namespace WiiTanks.Validator;

using WiiTanks.Geometry;
using System.Collections.Generic;

public class ArenaValidator
{
    public static bool IsValid(Grid grid)
    {
        int openSpaces = 0;

        for (int x = 0; x < grid.Width(); x++)
        {
            for (int y = 0; y < grid.Height(); y++)
            {
                if (!TileResolver.IsValid(grid.Data[x, y]) || TileResolver.IsEmpty(grid.Data[x, y]))
                {
                    return false;
                }
                if (grid.IsOnEdge(x, y) && !TileResolver.IsBlocking(grid.Data[x, y]))
                {
                    return false;
                }

                if (!grid.IsOnEdge(x, y) && grid.IsOpenSpace(x, y))
                {
                    openSpaces++;
                }
            }
        }
        Log.Error(
            "Playable area: "
                + openSpaces
                + " out of "
                + (grid.Width() - 2) * (grid.Height() - 2)
                + " => "
                + (openSpaces * 100.0 / ((grid.Width() - 2) * (grid.Height() - 2)))
                + "%"
        );

        var unseen = new bool[grid.Width(), grid.Height()];

        Point startPoint = new(-1, -1); // Invalid
        for (int x = 0; x < grid.Width(); x++)
        {
            for (int y = 0; y < grid.Height(); y++)
            {
                if (TileResolver.IsOpen(grid.GetTile(x, y)))
                {
                    unseen[x, y] = true;
                    if (!startPoint.IsValidOnGrid(grid))
                    {
                        startPoint = new Point(x, y);
                    }
                }
            }
        }

        unseen = FloodUnseenGrid(unseen, startPoint);
        for (int x = 0; x < unseen.GetLength(0); x++)
        {
            for (int y = 0; y < unseen.GetLength(1); y++)
            {
                if (unseen[x, y])
                {
                    Log.Info("Unreachable rooms detected!");
                    return false;
                }
            }
        }

        return openSpaces >= (grid.Width() - 2) * (grid.Height() - 2) * 0.25;
    }

    private static bool[,] FloodUnseenGrid(bool[,] unseen, Point from)
    {
        int x = (int)from.X;
        int y = (int)from.Y;
        if (x >= 0 && x < unseen.GetLength(0) && y >= 0 && y < unseen.GetLength(1) && unseen[x, y])
        {
            unseen[x, y] = false;
            FloodUnseenGrid(unseen, new Point(x - 1, y));
            FloodUnseenGrid(unseen, new Point(x + 1, y));
            FloodUnseenGrid(unseen, new Point(x, y - 1));
            FloodUnseenGrid(unseen, new Point(x, y + 1));
        }
        return unseen;
    }
}
