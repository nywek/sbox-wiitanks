namespace WiiTanks.Geometry;

using System.Collections.Generic;

public class Pathway
{
	private Room fromRoom;
	public Room From
	{
		get { return fromRoom; }
		set
		{
			fromRoom = value;
			Update();
		}
	}

	private Room toRoom;
	public Room To
	{
		get { return toRoom; }
		set
		{
			toRoom = value;
			Update();
		}
	}

	public Point MiddlePoint { get; private set; }
	public Point StartPoint { get; private set; }
	public Point EndPoint { get; private set; }

	public Pathway(Room from, Room to)
	{
		From = from;
		To = to;
	}

	private void Update()
	{
		if (From == null || To == null)
		{
			return;
		}
		MiddlePoint = Point.CenterOf(From.Center(), To.Center());
		StartPoint = Point.CenterOf(
			From.GetNearestEdgeMiddle(MiddlePoint),
			From.GetNearestCorner(MiddlePoint)
		);
		EndPoint = Point.CenterOf(
			To.GetNearestEdgeMiddle(MiddlePoint),
			To.GetNearestCorner(MiddlePoint)
		);
	}

	public double Distance()
	{
		return StartPoint.DistanceTo(EndPoint);
	}

	public void StoreToGrid(Grid grid)
	{
		List<Line> lines =
			new()
			{
				new Line(From.Center(), StartPoint),
				new Line(StartPoint, MiddlePoint),
				new Line(MiddlePoint, EndPoint),
				new Line(EndPoint, To.Center()),
				new Line(MiddlePoint, From.GetNearestCorner(MiddlePoint)),
				new Line(MiddlePoint, To.GetNearestCorner(MiddlePoint)),
				new Line(From.GetNearestCorner(MiddlePoint), To.GetNearestCorner(MiddlePoint)),
				new Line(
					From.GetNearestEdgeMiddle(MiddlePoint),
					To.GetNearestEdgeMiddle(MiddlePoint)
				),
				new Line(From.GetNearestEdgeMiddle(MiddlePoint), To.GetNearestCorner(MiddlePoint)),
				new Line(From.GetNearestCorner(MiddlePoint), To.GetNearestEdgeMiddle(MiddlePoint))
			};

		lines.ForEach(line => line.StoreToGrid(grid, TileResolver.PATHWAY));
	}
}
