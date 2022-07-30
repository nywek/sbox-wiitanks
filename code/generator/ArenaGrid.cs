namespace WiiTanks;

using WiiTanks.Geometry;
using WiiTanks.Validator;
using System;
using System.Collections.Generic;
using Sandbox;

public class ArenaGrid : Grid
{
	public List<Box> Boxes { get; set; }
	public ArenaGrid() : base()
	{
		Boxes = new();
		GenerateNewArena();
	}

	public void GenerateNewArena()
	{
		Reset();

		Boxes.ForEach(box => {
			box.Delete();
		});
		Boxes.Clear();

		List<Room> rooms = new();
		List<Pathway> pathways = new();

		int tries = 0;
		while (tries < 1000 && rooms.Count < Math.Sqrt(Area()) / 3)
		{
			tries++;

			double roomTargetArea = Math.Sqrt(Area()) * Random.Shared.NextDouble() - 0.5;
			int x1 = Rand.Int(1, Width() - 1);
			int x2 = Rand.Int(1, Width() - 1);
			int y1 = Rand.Int(1, Height() - 1);
			int y2 = Rand.Int(1, Height() - 1);

			var room = new Room(x1, x2, y1, y2);

			if (
				room.Area() > 3
				&& room.Area() >= roomTargetArea / 2
				&& room.Area() <= roomTargetArea * 2
				&& room.IsValidOnGrid(this)
			)
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

		pathways.ForEach(pathway =>
		{
			pathway.StoreToGrid(this);
		});

		// Update empty spaces to walls
		for ( int h = 0; h < Height(); h++ )
		{
			for ( int w = 0; w < Width(); w++ )
			{
				if (TileResolver.IsEmpty(GetTile(w, h)))
				{
					new Point(w, h).StoreToGrid(this, TileResolver.DYNAMIC_WALL);
				}
			}
		}

		Smooth();

		// TODO: Remove bad practice, maybe infinite generation of Arena
		// Still unlikely to generate invalid arenas more than 5 times in a row
		if (!ArenaValidator.IsValid(this))
		{
			Log.Info("Regenerate because an invalid arena was generated...");
			GenerateNewArena();
		}
	}

	public void PlaceEntities(Vector3 origin)
	{
		for ( int h = 0; h < Height(); h++ )
		{
			for ( int w = 0; w < Width(); w++ )
			{
				if(TileResolver.IsBlocking(GetTile(w, h))) 
				{
					var box = new Box();
					box.Material = Material.Load("materials/dev/reflectivity_30.vmat");
					box.Size = 64;
					box.Position = origin + new Vector3(w * 64, h * 64, 0);
//					box.Tags.Add("arena_wall");
					Boxes.Add(box);
				}
			}
		}

	}
}
