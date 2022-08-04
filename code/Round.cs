using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using WiiTanks.Extensions;

namespace WiiTanks;

public partial class Round : BaseNetworkable
{
	private const float PREPARING_DURATION = 4f;
	private const float ENDING_DURATION = 4f;

	[Net] public RoundState State { get; set; }
	[Net] public Arena Arena { get; set; }
	[Net] public IList<Participant> Participants { get; set; }
	[Net] public RoundOptions Options { get; set; } = new RoundOptions();

	// Serverside:
	private Dictionary<Client, Tank> TankMap = new();
	private TimeSince TimeSinceStateChanged;

	public Round() { }

	public Round( RoundOptions options )
	{
		State = RoundState.Initializing;
		Arena = Entity.FindByIndex( options.SelectedArenaId ) as Arena;
		Options = options;
	}

	private void SwitchState( RoundState newState )
	{
		State = newState;
		TimeSinceStateChanged = 0;
		Log.Info( $"New round state: {newState}" );
		Events.Broadcast( "round.statechange", newState );
	}
	
	public void UpdateFrom( Lobby lobby )
	{
		SwitchState( RoundState.Initializing );
		
		Participants.Clear();

		Arena = lobby.Options.SelectedArena;
		
		Options = lobby.Options;
		
		foreach (var slot in lobby.Slots.Where(s => s.IsOccupied))
		{
			Join( slot.Client, slot.Team );
		}
	}

	public void Join( Client client, Team team )
	{
		if ( Participants.Any( p => p.Client == client ) )
			throw new Exception( "client already joined this round" );

		var participant = new Participant( client, team );
		Participants.Add( participant );
	}

	public void Leave( Client client )
	{
		Participants.RemoveAll( p => p.Client == client );

		if ( TankMap[client] is Tank t )
		{
			t.Delete();
			TankMap.Remove( client );
		}
	}

	public void Begin()
	{
		// Find spawns for all participants
		Dictionary<Team, Queue<ArenaSpawn>> availableSpawns = Arena.FindSpawnPoints()
			.GroupBy( spawn => spawn.Team )
			.ToDictionary( g => g.Key, g => new Queue<ArenaSpawn>( g ) );

		Dictionary<Client, ArenaSpawn> spawnMap = new();

		foreach ( var participant in Participants )
		{
			if ( availableSpawns[participant.Team].Count <= 0 )
				throw new Exception( $"no spawns left for team: {participant.Team}" );

			spawnMap[participant.Client] = availableSpawns[participant.Team].Dequeue();
		}

		// Create tanks for each participant
		CleanupEntities();

		foreach ( var participant in Participants )
		{
			var tank = new Tank( participant.Client, participant.Team );
			var spawn = spawnMap[participant.Client];

			TankMap[participant.Client] = tank;

			tank.SpawnAtArena( Arena, spawn.Position );
		}

		// Set options
		Tank.MaxAmmo = Options.MaxAmmo;
		Tank.DefaultSpeed = Options.TankSpeed;
		Missile.MaxBounces = Options.MissileBounces;

		SwitchState( RoundState.Preparing );
	}

	public void Tick()
	{
		Log.Debug($"{this} has {Participants.Count} participants");
		
		if ( State == RoundState.Preparing && TimeSinceStateChanged > PREPARING_DURATION )
		{
			SwitchState( RoundState.Active );
			ActivateTanks();
		}

		if ( State == RoundState.Active )
		{
			// Check if only one team or no teams are alive
			List<Team> aliveTeams = TankMap.Values.GroupBy( t => t.Team )
				.Where( group => group.Any( t => t.LifeState != LifeState.Dead ) )
				.Select( group => group.Key )
				.ToList();
			
			if ( aliveTeams.Count <= 1 )
			{
				EndRound( aliveTeams.FirstOrDefault() );
			}
		}

		if ( State == RoundState.Ending && TimeSinceStateChanged > ENDING_DURATION )
		{
			// Round cleanup
			CleanupEntities();
			SwitchState( RoundState.Ended );
		}
	}

	public void ActivateTanks()
	{
		foreach ( var tank in TankMap.Values )
		{
			tank.LifeState = LifeState.Alive;
		}
	}

	public void EndRound( Team? winningTeam )
	{
		SwitchState( RoundState.Ending );
		Events.Broadcast( "round.won", winningTeam );
		Log.Info( $"Round has ended. Winning team: {winningTeam}" );
	}

	private void CleanupEntities()
	{
		foreach ( var entity in Entity.All.Where( e => e.Tags.Has( "ArenaEntity" ) ).ToList() )
		{
			entity.Delete();
		}
	}

	public override string ToString()
	{
		return $"Round({State}, {Participants.Count})";
	}
	
}
