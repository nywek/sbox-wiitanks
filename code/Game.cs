using Sandbox;
using System.Linq;

namespace WiiTanks;

public partial class TankGame : Sandbox.Game
{
	[Net] public Lobby Lobby { get; set; }
	[Net] public Round Round { get; set; }

	public TankGame()
	{
	}

	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();

		var arena = Entity.All.OfType<Arena>().First();
		Round = new Round( arena );

		Lobby = new Lobby();
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var tank = new Tank( client, Team.BLUE );
		var spawn = Round.Arena.FindSpawnPoints().First();

		tank.SpawnAtArena( Round.Arena, spawn.Position );
	}
}
