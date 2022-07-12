namespace WiiTanks;

public class TileResolver
{
	public const int OUTER_SPACE = -1;
	public const int EMPTY = 0;
	public const int STATIC_WALL = 1;
	public const int DYNAMIC_WALL = 2;
	public const int ROOM = 3;

	public static bool IsValid(int tile)
	{
		return tile != OUTER_SPACE;
	}

	public static bool IsEmpty(int tile)
	{
		return tile == EMPTY;
	}

	public static bool IsWall(int tile)
	{
		return tile == STATIC_WALL || tile == DYNAMIC_WALL;
	}

	public static bool IsRoom(int tile)
	{
		return tile == ROOM;
	}

	public static bool IsBlocking(int tile)
	{
		return !IsValid(tile) || (!IsEmpty(tile) && !IsRoom(tile));
	}
}
