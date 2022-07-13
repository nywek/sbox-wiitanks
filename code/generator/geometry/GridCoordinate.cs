namespace WiiTanks.Geometry;

using System;

public struct GridCoordinate
{
	public int X { get; set; }
	public int Y { get; set; }

	public GridCoordinate(int x, int y)
	{
		X = x;
		Y = y;
	}

	public GridCoordinate(Point point)
	{
		X = (int)Math.Round(point.X, MidpointRounding.AwayFromZero);
		Y = (int)Math.Round(point.Y, MidpointRounding.AwayFromZero);
	}
}
