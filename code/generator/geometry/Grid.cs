namespace WiiTanks;

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
		for (int x = 0; x < Data.GetLength(0); x++)
		{
			for (int y = 0; y < Data.GetLength(1); y++)
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

	public int Area()
	{
		return Data.Length;
	}

	public bool IsInGrid(int x, int y)
	{
		return x >= 0 && y >= 0 && x < Data.GetLength(0) && y < Data.GetLength(1);
	}

	public bool IsOnEdge(int x, int y)
	{
		return x == 0 || y == 0 || x == Data.GetLength(0) - 1 || y == Data.GetLength(1) - 1;
	}

	public int GetTile(int x, int y)
	{
		if (IsInGrid(x, y))
		{
			return Data[x, y];
		}
		return TileResolver.OUTER_SPACE;
	}

	public int GetSurroundingBlockingTiles(int x, int y, int deltaSize = 1)
	{
		int tiles = 0;

		for (int dx = -deltaSize; dx <= deltaSize; dx++)
		{
			for (int dy = -deltaSize; dy <= deltaSize; dy++)
			{
				if (!(dx == 0 && dy == 0) && TileResolver.IsBlocking(GetTile(x + dx, y + dy)))
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
			for (int x = 0; x < Data.GetLength(0); x++)
			{
				for (int y = 0; y < Data.GetLength(1); y++)
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
						Data[x, y] = TileResolver.STATIC_WALL;
					}
					if (surrounding < threshold - roughness)
					{
						Data[x, y] = TileResolver.EMPTY;
					}
				}
			}
		}
	}
}
