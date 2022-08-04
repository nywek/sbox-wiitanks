using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace WiiTanks;

public partial class Lobby : BaseNetworkable
{
	public const int MaxSlots = 4;

	[Net] public IList<Slot> Slots { get; set; }
	[Net] public RoundOptions Options { get; set; }

	public bool IsFull => Slots.Count( s => s.Client is null ) <= 0;

	public void Init()
	{
		for ( int i = 0; i < MaxSlots; i++ )
		{
			Slots.Add( new Slot( i ) );
		}
		
		var randomArena = Entity.All.OfType<Arena>().First();
		
		Options = new RoundOptions
		{
			SelectedArenaId = randomArena.NetworkIdent,
			MinesEnabled = false,
			MaxAmmo = Tank.MaxAmmo,
			TankSpeed = Tank.DefaultSpeed,
			MissileBounces = Missile.MaxBounces
		};
	}

	public void Join( Client client )
	{
		int freeSlotIdx = Slots.First( s => s.Client is null ).Index;
		Join( client, freeSlotIdx );
	}

	public void Join( Client client, int slotIndex )
	{
		var slot = Slots.Where( s => s.Index == slotIndex ).First();
		if ( slot.IsOccupied ) { throw new Exception( "slot already occupied" ); }

		slot.Client = client;
	}

	public void Leave( Client client )
	{
		foreach ( var slot in Slots.Where( s => s.Client == client ) )
		{
			slot.Client = null;
		}
	}

	public override string ToString() => $"Lobby({Slots})";
}
