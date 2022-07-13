namespace WiiTanks.Geometry;

using System;

public struct Point
{
	public double X { get; set; }
	public double Y { get; set; }

	public Point(double x, double y)
	{
		X = x;
		Y = y;
	}

	public Point(GridCoordinate coordinate) : this(coordinate.X, coordinate.Y) { }

	public double DistanceTo(Point other)
	{
		return Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2));
	}

	public Point GetCenterPointTo(Point other)
	{
		return CenterOf(this, other);
	}

	public static Point CenterOf(params Point[] points)
	{
		double x = 0.0;
		double y = 0.0;
		if (points.Length == 0)
		{
			return new Point(double.NaN, double.NaN);
		}

		foreach (var point in points)
		{
			x += point.X;
			y += point.Y;
		}

		return new Point(x / points.Length, y / points.Length);
	}

	public Point GetNearest(params Point[] points)
	{
		if (points.Length == 0)
		{
			return new Point(double.NaN, double.NaN);
		}

		Point nearest = points[0];

		foreach (var point in points)
		{
			if (DistanceTo(point) < DistanceTo(nearest))
			{
				nearest = point;
			}
		}

		return nearest;
	}

	public bool IsValidOnGrid(Grid grid)
	{
		return new GridCoordinate(grid, this).IsValid();
	}

	public void StoreToGrid(Grid grid, int tile = TileResolver.EMPTY)
	{
		GridCoordinate pos = new(grid, this);
		if (pos.IsValid())
		{
			pos.ApplyTile(tile);
		}
	}
}
