namespace WiiTanks;

using WiiTanks.Geometry;
using WiiTanks.Validator;
using System;
using System.Collections.Generic;
using Sandbox;

public class ArenaGrid : Grid
{
	public Vector3 Origin { get; set; }
	public List<Box> Boxes { get; set; }
	public ArenaGrid(Vector3 origin) : base()
	{
		Origin = origin;
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
		while (tries < 1000 && rooms.Count < Area() * 3/8)
		{
			tries++;

			double roomTargetArea = Math.Sqrt(Area()) / 4;
			int x1 = Rand.Int(1, Width() - 1);
			int x2 = x1 + Rand.Int(0, 1);
//			int x2 = x1 + (int) (Math.Sqrt(Area()) * Rand.Double(-0.75f, 0.75f));
			int y1 = Rand.Int(1, Height() - 1);
			int y2 = y1 + Rand.Int(0, 1);
//			int y2 = y1 + (int)Math.Floor(roomTargetArea / Math.Abs(x2 - x1) + Rand.Int(-1, 1));

			var room = new Room(x1, x2, y1, y2);

			if (
/*				room.Area() > 3
				&& room.Area() <= roomTargetArea * 1.75
				&& (Math.Max(room.Width(), room.Height()) / Math.Min(room.Width(), room.Height())) < 4
				&& */room.IsValidOnGrid(this)
			)
			{
				rooms.Add(room);
				room.StoreToGrid(this);
			}
		}

/*		int smoothness = Rand.Int(1, 3);
		int roughness = Rand.Int(0, 1);*/

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
			int maxPathways = Rand.Int(1, 3);
			for (int i = 0; i < maxPathways; i++)
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

		int diagonals = RemoveDiagonalWalls();
		int singleOpens = RemoveSingleSpaces();

		List<Vector2> singleWalls = GetSingleWalls();
		Log.Info("Single Walls: " + singleWalls);
		int singleWallsAllowed = Rand.Int(0, 5);
		int removeSingles = singleWalls.Count - singleWallsAllowed;
		for(int i = 0; i < removeSingles; i++)
		{
			int index = Rand.Int(0, singleWalls.Count - 1);
			Log.Info("Removing index: " + index + " (" + singleWalls.Count + ")");
			new Point(singleWalls[index].x, singleWalls[index].y).StoreToGrid(this, TileResolver.PATHWAY);
			singleWalls.RemoveAt(index);
		}

		Log.Info("singleOpens removed: " + singleOpens + " diagonals removed: " + diagonals + " single walls removed: " + removeSingles + " single walls allowed: " + singleWallsAllowed);

		// TODO: Remove bad practice, maybe infinite generation of Arena
		// Still unlikely to generate invalid arenas more than 5 times in a row
		if (!ArenaValidator.IsValid(this))
		{
			Log.Info("Regenerate because an invalid arena was generated...");
			GenerateNewArena();
		}
	}

	public List<Vector2> GetSingleWalls()
	{
		List<Vector2> singleWalls = new();

		for ( int h = 0; h < Height(); h++ )
		{
			for ( int w = 0; w < Width(); w++ )
			{
				if( TileResolver.IsBlocking(GetTile(w, h)) )
				{
					if( IsInGrid(w - 1, h) && IsInGrid(w + 1, h) && IsInGrid(w, h - 1) && IsInGrid(w, h + 1)
						&& !IsOnEdge(w - 1, h) && !IsOnEdge(w + 1, h) && !IsOnEdge(w, h - 1) && !IsOnEdge(w, h + 1)
						&& !TileResolver.IsBlocking(GetTile(w - 1, h)) && !TileResolver.IsBlocking(GetTile(w + 1, h)) && !TileResolver.IsBlocking(GetTile(w, h - 1)) && !TileResolver.IsBlocking(GetTile(w, h + 1))
						&& !TileResolver.IsBlocking(GetTile(w - 1, h - 1)) && !TileResolver.IsBlocking(GetTile(w - 1, h + 1)) && !TileResolver.IsBlocking(GetTile(w + 1, h - 1)) && !TileResolver.IsBlocking(GetTile(w + 1, h + 1))
					)
					{
						Log.Info("Single blocking: " + w + ", " + h);
						singleWalls.Add(new Vector2(w, h));
					}
				}
			}
		}

		return singleWalls;
	}

	public int RemoveSingleSpaces()
	{
		int spaces = 0;
		for ( int h = 0; h < Height(); h++ )
		{
			for ( int w = 0; w < Width(); w++ )
			{
				if( !TileResolver.IsBlocking(GetTile(w, h)) )
				{
					if( IsInGrid(w - 1, h) && IsInGrid(w + 1, h) && IsInGrid(w, h - 1) && IsInGrid(w, h + 1)
						&& TileResolver.IsBlocking(GetTile(w - 1, h)) && TileResolver.IsBlocking(GetTile(w + 1, h)) && TileResolver.IsBlocking(GetTile(w, h - 1)) && TileResolver.IsBlocking(GetTile(w, h + 1))
					)
					{
						Log.Info("Remove single open: " + w + ", " + h);
						new Point(w, h).StoreToGrid(this, TileResolver.DYNAMIC_WALL);
					}
				}
			}
		}

		return spaces;
	}

	public int RemoveDiagonalWalls()
	{
		int diagonals = 0;

		for ( int h = 0; h < Height(); h++ )
		{
			for ( int w = 0; w < Width(); w++ )
			{
				if( TileResolver.IsBlocking(GetTile(w, h)) )
				{
					if( IsInGrid(w - 1, h) && IsInGrid(w + 1, h) && IsInGrid(w, h - 1) && IsInGrid(w, h + 1)
						&& !IsOnEdge(w - 1, h) && !IsOnEdge(w + 1, h) && !IsOnEdge(w, h - 1) && !IsOnEdge(w, h + 1)
						&& !TileResolver.IsBlocking(GetTile(w - 1, h)) && !TileResolver.IsBlocking(GetTile(w + 1, h)) && !TileResolver.IsBlocking(GetTile(w, h - 1)) && !TileResolver.IsBlocking(GetTile(w, h + 1))
						&& (TileResolver.IsBlocking(GetTile(w - 1, h - 1)) || TileResolver.IsBlocking(GetTile(w - 1, h + 1)) || TileResolver.IsBlocking(GetTile(w + 1, h - 1)) || TileResolver.IsBlocking(GetTile(w + 1, h + 1)))
					)
					{
						if(GetSurroundingBlockingTiles(w, h, 2) <= 12) {
							new Point(w, h).StoreToGrid(this, TileResolver.PATHWAY);
						} else {
							int dw = Rand.Int(0, 1);
							int dh = 1 - dw;
							new Point(w - dw, h - dh).StoreToGrid(this, TileResolver.DYNAMIC_WALL);
							new Point(w + dw, h + dh).StoreToGrid(this, TileResolver.DYNAMIC_WALL);
						}
						diagonals++;
					}
				}
			}
		}

		return diagonals;
	}

	public void PlaceEntities()
	{
		for ( int h = 0; h < Height(); h++ )
		{
			for ( int w = 0; w < Width(); w++ )
			{
				if(TileResolver.IsBlocking(GetTile(w, h))) 
				{
					var box = new Box();
					box.Material = Material.Load("materials/arena/wall.vmat");
					box.Size = 64;
					box.Position = Origin + new Vector3(w * 64, h * 64, 0);
					box.Tags.Add( "ArenaEntity" );
					Boxes.Add(box);
				}
			}
		}
	}
}
