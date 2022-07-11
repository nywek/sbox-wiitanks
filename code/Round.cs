using System.Collections.Generic;
using Sandbox;

namespace WiiTanks;

public partial class Round : BaseNetworkable
{
	[Net] public RoundState State { get; set; }
	[Net] public Arena Arena { get; set; }
	[Net] public IList<Participant> Participants { get; set; }

	public Round() { }
	
	public Round(Arena arena)
	{
		State = RoundState.INITIALIZING;
		Arena = arena;
	}

	public void Join( Client client, Team team )
	{
		var participant = new Participant( client, team );
		Participants.Add( participant );
	}

	public void Leave( Client client )
	{
		// TODO
		Participants.RemoveAll( p => p.Client == client );
	}

	public void Begin()
	{
		
	}

	public override string ToString()
	{
		return $"Round({State})";
	}
}
