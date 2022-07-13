namespace WiiTanks;

public class TileResolver
{
	public const int INVALID = -1;
	public const int EMPTY = 0;
	public const int STATIC_WALL = 1;
	public const int DYNAMIC_WALL = 2;
	public const int ROOM = 4;
	public const int PATHWAY = 8;

	private const int OPEN = EMPTY | ROOM | PATHWAY;
	private const int BLOCKING = STATIC_WALL | DYNAMIC_WALL;

	public static bool IsValid(int tile)
	{
		return tile != INVALID;
	}

	public static bool IsEmpty(int tile)
	{
		return IsValid(tile) && tile == EMPTY;
	}

	public static bool IsStaticWall(int tile)
	{
		return IsValid(tile) && (tile & STATIC_WALL) != 0;
	}

	public static bool IsDynamicWall(int tile)
	{
		return IsValid(tile) && (tile & DYNAMIC_WALL) != 0;
	}

	public static bool IsRoom(int tile)
	{
		return IsValid(tile) && (tile & ROOM) != 0;
	}

	public static bool IsPathway(int tile)
	{
		return IsValid(tile) && (tile & PATHWAY) != 0;
	}

	public static bool IsWall(int tile)
	{
		return IsStaticWall(tile) || IsDynamicWall(tile);
	}

	public static bool IsOpen(int tile)
	{
		return IsValid(tile) && (tile & OPEN) != 0;
	}

	public static bool IsBlocking(int tile)
	{
		return !IsValid(tile) || (tile & BLOCKING) != 0;
	}

	public static int ApplyOpenBlockingUnsafe(int source, int tile)
	{
		if (!IsValid(source) || !IsValid(tile))
		{
			return INVALID;
		}
		return source | tile;
	}

	public static int Apply(int source, int tile)
	{
		int appliedTile = ApplyOpenBlockingUnsafe(source, tile);
		if (!IsValid(appliedTile))
		{
			return INVALID;
		}
		// Result would be in open and blocking state at the same time, but applying tile is not
		if (!(IsOpen(tile) && IsBlocking(tile)) && IsOpen(appliedTile) && IsBlocking(appliedTile))
		{
			// Remove the blocking tiles, when applying tile is in open state
			if (IsOpen(tile))
			{
				appliedTile &= ~BLOCKING;
			}
			// Remove the open tiles, when applying tile is in blocking state
			if (IsBlocking(tile))
			{
				appliedTile &= ~OPEN;
			}
		}
		return appliedTile;
	}
}
