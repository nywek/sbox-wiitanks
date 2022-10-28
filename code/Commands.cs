using System;
using System.Linq;
using System.Text.Json;
using Sandbox;
using WiiTanks.Extensions;

namespace WiiTanks;

public class Commands
{
	public static TankGame Game => Sandbox.Game.Current as TankGame ?? throw new InvalidOperationException();

	[ConCmd.Server( "lobby_join" )]
	public static void LobbyJoin( int slotIndex )
	{
		CurrentLobby.Join( ConsoleSystem.Caller, slotIndex );
	}

	[ConCmd.Server( "lobby_leave" )]
	public static void LobbyLeave()
	{
		CurrentLobby.Leave( ConsoleSystem.Caller );
	}

	[ConCmd.Server( "lobby_kick" )]
	public static void LobbyKick( int slotIndex )
	{
		var client = CurrentLobby.Slots[slotIndex].Client;
		CurrentLobby.Leave( client );
	}

	[ConCmd.Server( "lobby_begin_round" )]
	public static void LobbyBeginRound()
	{
		if ( !ConsoleSystem.Caller.IsListenServerHost )
			return;

		CurrentRound.UpdateFrom( CurrentLobby );
		//var round = CurrentLobby.CreateRound();
		//Game.Round = round;
		Hud.SwitchView( View.Ingame );
		CurrentRound.Begin();
	}

	[ConCmd.Server( "switch_view" )]
	public static void SwitchView( string viewString )
	{
		switch ( viewString )
		{
			case "main":
				Hud.SwitchView( View.MainMenu );
				break;
			case "lobby":
				Hud.SwitchView( View.Lobby );
				break;
			case "ingame":
				Hud.SwitchView( View.Ingame );
				break;
			case "arenaselection":
				Hud.SwitchView( View.ArenaSelection );
				break;
			case "test":
				Hud.SwitchView( View.TestMenu );
				break;
			case "editor":
				Hud.SwitchView( View.ArenaEditor );
				break;
		}
	}

	[ConCmd.Server( "lobby_set_team" )]
	public static void SwapTeam( int slotIndex, Team newTeam )
	{
		if ( !ConsoleSystem.Caller.IsListenServerHost )
			return;

		CurrentLobby.Slots[slotIndex].Team = newTeam;
	}

	public static void UpdateLobbyOptions( RoundOptions options )
	{
		UpdateLobbyOptions( JsonSerializer.Serialize( options ) );
	}

	[ConCmd.Server]
	private static void UpdateLobbyOptions( string optionsJson )
	{
		var options = JsonSerializer.Deserialize<RoundOptions>( optionsJson );

		if ( !ConsoleSystem.Caller.IsListenServerHost )
			return;

		CurrentLobby.Options = options;
		Events.Broadcast( "lobby.options.updated" );
	}

	[ConCmd.Server]
	public static void AddBotToLobby( int slotIndex )
	{
		var bot = Client.All.FirstOrDefault( c => c.IsBot && CurrentLobby.Slots.All( s => s.Client != c ) );
		
		if ( bot is null )
		{
			bot = new Bot().Client;
		}

		CurrentLobby.Join( bot, slotIndex );
	}
}
