namespace WiiTanks.Geometry;

using System;

public struct GridCoordinate
{
	public Grid Grid { get; set; }
	public int X { get; set; }
	public int Y { get; set; }

	public GridCoordinate(Grid grid, int x, int y)
	{
		Grid = grid;
		X = x;
		Y = y;
	}

	public GridCoordinate(Grid grid, Point point)
	{
		Grid = grid;
		X = (int)Math.Round(point.X, MidpointRounding.AwayFromZero);
		Y = (int)Math.Round(point.Y, MidpointRounding.AwayFromZero);
	}

	public bool IsValid()
	{
		return Grid.IsInGrid(X, Y) && !Grid.IsOnEdge(X, Y);
	}

	public void ApplyTile(int tile = TileResolver.EMPTY)
	{
		if (IsValid())
		{
			Grid.Data[X, Y] = TileResolver.Apply(Grid.Data[X, Y], tile);
		}
	}

	public void OverrideTile(int tile)
	{
		if (IsValid())
		{
			Grid.Data[X, Y] = tile;
		}
	}
}
