namespace WiiTanks.Validator;

using WiiTanks.Geometry;

public class ArenaValidator
{
	// TODO: Check for non accessable paths in the grid, maybe unconnected rooms
	public static bool IsValid(Grid grid)
	{
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
			}
		}
		return true;
	}
}
