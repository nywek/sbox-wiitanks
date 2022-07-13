namespace WiiTanks.Geometry;

using System;

public struct Line
{
	private const double STORE_TO_GRID_STEP_SIZE = 0.5;

	public Point From { get; set; }
	public Point To { get; set; }

	public Line(Point from, Point to)
	{
		From = from;
		To = to;
	}

	public void StoreToGrid(Grid grid, int tile = TileResolver.EMPTY)
	{
		double dx = To.X - From.X;
		double dy = To.Y - From.Y;
		double distance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
		dx = dx / distance * STORE_TO_GRID_STEP_SIZE;
		dy = dy / distance * STORE_TO_GRID_STEP_SIZE;

		// TODO: Draw better line; this has overhead, because many Points are stored multiple times.
		double x = From.X;
		double y = From.Y;
		while (
			Math.Abs(To.X - x) > STORE_TO_GRID_STEP_SIZE
			|| Math.Abs(To.Y - y) > STORE_TO_GRID_STEP_SIZE
		)
		{
			new Point(x, y).StoreToGrid(grid, tile);
			x += dx;
			y += dy;
		}
	}
}
