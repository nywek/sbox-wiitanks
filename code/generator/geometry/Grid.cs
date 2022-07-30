namespace WiiTanks.Geometry;

public class Grid
{
	public int[,] Data { get; private set; }

	public Grid(
		int width = ArenaGridConfiguration.ARENA_WIDTH,
		int height = ArenaGridConfiguration.ARENA_HEIGHT
	)
	{
		Data = new int[width, height];
		Reset();
	}

	public void Reset()
	{
		for (int x = 0; x < Width(); x++)
		{
			for (int y = 0; y < Height(); y++)
			{
				if (IsOnEdge(x, y))
				{
					Data[x, y] = TileResolver.STATIC_WALL;
				}
				else
				{
					Data[x, y] = TileResolver.EMPTY;
				}
			}
		}
	}

	public int Width()
	{
		return Data.GetLength(0);
	}

	public int Height()
	{
		return Data.GetLength(1);
	}

	public int Area()
	{
		return Data.Length;
	}

	public bool IsInGrid(int x, int y)
	{
		return x >= 0 && y >= 0 && x < Width() && y < Height();
	}

	public bool IsOnEdge(int x, int y)
	{
		return x == 0 || y == 0 || x == Width() - 1 || y == Height() - 1;
	}

	public int GetTile(int x, int y)
	{
		if (IsInGrid(x, y))
		{
			return Data[x, y];
		}
		return TileResolver.INVALID;
	}

	public int GetSurroundingBlockingTiles(int x, int y, int deltaSize = 1)
	{
		int tiles = 0;

		for (int dx = -deltaSize; dx <= deltaSize; dx++)
		{
			for (int dy = -deltaSize; dy <= deltaSize; dy++)
			{
				if (
					!(dx == 0 && dy == 0)
					&& (
						TileResolver.IsEmpty(GetTile(x + dx, y + dy))
						|| TileResolver.IsBlocking(GetTile(x + dx, y + dy))
					)
				)
				{
					tiles++;
				}
			}
		}
		return tiles;
	}

	public void Smooth(int iterations = 1, int roughness = 0, int deltaSize = 1)
	{
		for (int iteration = 0; iteration < iterations; iteration++)
		{
			for (int x = 0; x < Width(); x++)
			{
				for (int y = 0; y < Height(); y++)
				{
					if (IsOnEdge(x, y))
					{
						continue;
					}

					// Threshold
					// 1: 3x3 => 4 of 8 neighbours
					// 2: 5x5 => 12 of 24 neighbours
					// 3: 7x7 => 24 of 48 neighbours
					int threshold = ((deltaSize * 2 + 1) * (deltaSize * 2 + 1) - 1) / 2;

					int surrounding = GetSurroundingBlockingTiles(x, y);
					if (surrounding > threshold + roughness)
					{
						new Point(x, y).StoreToGrid(this, TileResolver.DYNAMIC_WALL);
					}
					if (surrounding < threshold - roughness)
					{
						new Point(x, y).StoreToGrid(this, TileResolver.PATHWAY);
					}
				}
			}
		}
	}

	public void Debug()
	{
		Log.Info("x/y | 00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 ");
		Log.Info("    + ----------------------------------------------------------------------------- ");
		for (int h = 0; h < Height(); h++)
		{
			var line = "";
			for (int w = 0; w < Width(); w++)
			{
				line += (Data[w, h] < 10 ? "0" : "") + Data[w, h] + " ";
			}
			Log.Info((h < 10 ? " 0" : " ") + h + " | " + line);
		}
		Log.Info("");
	}
}
