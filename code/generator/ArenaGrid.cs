namespace WiiTanks;

using WiiTanks.Geometry;
using WiiTanks.Validator;
using System;
using System.Collections.Generic;
using Sandbox;

public class ArenaGrid : Grid
{
	public ArenaGrid()
	{
		GenerateNewArena();
	}

	public void GenerateNewArena()
	{
		Reset();

		List<Room> rooms = new();
		List<Pathway> pathways = new();

		int tries = 0;
		while (tries < 1000 && rooms.Count < Math.Sqrt(Area()) / 3)
		{
			tries++;

			double roomTargetArea = Math.Sqrt(Area()) * Random.Shared.NextDouble() - 0.5;
			int x1 = Rand.Int(1, Width() - 1);
			int x2 = Rand.Int(x1 + 1, Width() - 1);
			int y1 = Rand.Int(1, Height() - 1);
			int y2 = (int)Math.Floor(roomTargetArea / (x2 - x1) + y1);

			var room = new Room(x1, x2, y1, y2);

			if (room.Area() > 3 && room.IsValidOnGrid(this))
			{
				rooms.Add(room);
				room.StoreToGrid(this);
			}
		}

		rooms.ForEach(room =>
		{
			List<Pathway> roomPathways = new();
			rooms.ForEach(existingRoom =>
			{
				if (room != existingRoom)
				{
					roomPathways.Add(new Pathway(room, existingRoom));
				}
			});
			roomPathways.Sort((a, b) => (int)(a.Distance() - b.Distance()));
			for (int i = 0; i < 3; i++)
			{
				pathways.Add(roomPathways[i]);
			}
		});

		for (int i = 0; i < 5; i++)
		{
			pathways.ForEach(pathway =>
			{
				pathway.StoreToGrid(this);
			});
			Smooth();
		}

		// TODO: Remove bad practice, maybe infinite generation of Arena
		// Still unlikely to generate invalid arenas more than 5 times in a row
		if (!ArenaValidator.IsValid(this))
		{
			GenerateNewArena();
		}
	}
}
