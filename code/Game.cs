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

		Host.AssertServer();

		Lobby = new Lobby();
	}

	[Event.Tick.Server]
	public void OnTick()
	{
		Round?.Tick();
	}

	[Event( "round.statechange" )]
	public void OnRoundEnded( RoundState newState )
	{
		if ( newState == RoundState.Ended )
		{
			// Begin a new round
			var newRound = Lobby.CreateRound();
			Round = newRound;
			newRound.Begin();
		}
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		if ( Round is null )
		{
			// As long as no round exists, just randomly spawn tanks

			var tank = new Tank( client, Team.BLUE );
			var randomArena = Entity.All.OfType<Arena>().First();
			var spawn = randomArena.FindSpawnPoints().First();

			tank.SpawnAtArena( randomArena, spawn.Position );

			tank.LifeState = LifeState.Alive;
		}

		Lobby.Join( client );

		if ( Lobby.IsFull )
		{
			// Begin round, kill all existing tanks
			foreach ( var tank in Entity.All.OfType<Tank>().ToList() )
			{
				tank.Delete();
			}

			Round = Lobby.CreateRound();

			Round.Begin();
		}
	}
}
