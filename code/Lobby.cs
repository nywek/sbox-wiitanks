using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace WiiTanks;

public partial class Lobby : BaseNetworkable
{
	public const int MAX_SLOTS = 4;

	[Net] public IList<Slot> Slots { get; set; }

	public Lobby()
	{
		if ( Host.IsServer )
		{
			for ( int i = 0; i < MAX_SLOTS; i++ )
			{
				var slot = new Slot( i );
				Slots.Add( slot );
			}
		}
	}

	public bool IsFull => Slots.Count( s => s.Client is null ) <= 0;

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

	public Round CreateRound()
	{
		Host.AssertServer();
		var randomArena = Entity.All.OfType<Arena>().First();
		var round = new Round( randomArena );
		
		foreach (var slot in Slots.Where( s => s.IsOccupied ))
		{
			round.Join( slot.Client, slot.Team );
		}

		return round;
	}

	public override string ToString()
	{
		return $"Lobby({Slots})";
	}
}
