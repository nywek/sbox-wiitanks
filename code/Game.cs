using System.Linq;
using Sandbox;

namespace WiiTanks;

public partial class TankGame : Sandbox.Game
{
	// TODOs
	// Arenas
	// Spectator list
	// AI
	// Scoreboard
	// Disable some main menu buttons

	[Net] public Hud Hud { get; set; }
	[Net] public Lobby Lobby { get; set; }
	[Net] public Round Round { get; set; }

	public TankGame()
	{
		if ( IsServer )
		{
			Hud = new Hud();

			Lobby = new Lobby();

			Round = new Round();
		}
	}

	public override void PostLevelLoaded()
	{
		Lobby.Init();
	}

	[Event.Tick.Server]
	public void OnServerTick()
	{
		Round?.Tick();
	}

	[Event( "round.statechange" )]
	public void OnRoundEnded( RoundState newState )
	{
		if ( newState == RoundState.Ended )
		{
			// Go back to lobby
			Hud.SwitchView( View.Lobby );
			Round = new Round();
		}
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		client.Components.Create<ArenaCameraMode>();

		if ( Round is null )
		{
			// As long as no round exists, just randomly spawn tanks

			var tank = new Tank( client, Team.BLUE );
			var arena = Lobby.Options.SelectedArena;
			var spawn = arena.FindSpawnPoints().First();

			tank.SpawnAtArena( arena, spawn.Position );

			tank.LifeState = LifeState.Alive;
		}
	}
}
