namespace WiiTanks.Geometry;

public class Room
{
	private int MinX { get; set; }
	private int MaxX { get; set; }
	private int MinY { get; set; }
	private int MaxY { get; set; }

	public Room(int x1, int x2, int y1, int y2)
	{
		SetBoundary(x1, x2, y1, y2);
	}

	public void SetBoundary(int x1, int x2, int y1, int y2)
	{
		if (x1 > x2)
		{
			MinX = x2;
			MaxX = x1;
		}
		else
		{
			MinX = x1;
			MaxX = x2;
		}

		if (y1 > y2)
		{
			MinY = y2;
			MaxY = y1;
		}
		else
		{
			MinY = y1;
			MaxY = y2;
		}
	}

	public int Width()
	{
		return MaxX - MinX;
	}

	public int Height()
	{
		return MaxY - MinY;
	}

	public int Area()
	{
		return Width() * Height();
	}

	public double CenterX()
	{
		return (MinX + MaxX) / 2.0;
	}

	public double CenterY()
	{
		return (MinY + MaxY) / 2.0;
	}

	public Point Center()
	{
		return new Point(CenterX(), CenterY());
	}

	public Point[] GetCorners()
	{
		Point[] corners = new Point[4];

		corners[0] = new Point(MinX, MinY);
		corners[1] = new Point(MaxX, MinY);
		corners[2] = new Point(MaxX, MaxY);
		corners[3] = new Point(MinX, MaxY);

		return corners;
	}

	public Point GetNearestCorner(Point point)
	{
		Point[] corners = GetCorners();
		return point.GetNearest(corners);
	}

	public Point[] GetEdgeMiddles()
	{
		Point[] corners = GetCorners();
		Point[] edgeMiddles = new Point[corners.Length];

		for (int i = 0; i < corners.Length; i++)
		{
			edgeMiddles[i] = corners[i].GetCenterPointTo(corners[(i + 1) % corners.Length]);
		}

		return edgeMiddles;
	}

	public Point GetNearestEdgeMiddle(Point point)
	{
		Point[] edgeMiddles = GetEdgeMiddles();
		return point.GetNearest(edgeMiddles);
	}

	public bool IsOverlapping(Point point)
	{
		return point.X >= MinX && point.X <= MaxX && point.Y >= MinY && point.Y <= MaxY;
	}

	public bool IsOverlapping(GridCoordinate coordinate)
	{
		return coordinate.X >= MinX
			&& coordinate.X <= MaxX
			&& coordinate.Y >= MinY
			&& coordinate.Y <= MaxY;
	}

	public bool IsValidOnGrid(Grid grid)
	{
		for (int x = MinX; x <= MaxX; x++)
		{
			for (int y = MinY; y <= MaxY; y++)
			{
				if (
					!grid.IsInGrid(x, y)
					|| grid.IsOnEdge(x, y)
					|| !TileResolver.IsEmpty(grid.GetTile(x, y))
				)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void StoreToGrid(Grid grid)
	{
		if (!IsValidOnGrid(grid))
		{
			return;
		}
		
		for (int x = MinX; x <= MaxX; x++)
		{
			for (int y = MinY; y <= MaxY; y++)
			{
				new GridCoordinate(grid, x, y).ApplyTile(TileResolver.ROOM);
			}
		}
	}
}
