using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace WiiTanks.Extensions;

public static class Extensions
{
	public static void Debug( this Logger logger, string msg )
	{
		logger.Info( $"[{Realm}]: {msg}" );
	}

	public static void Show( this Panel panel )
	{
		panel.RemoveClass( "hidden" );
	}

	public static void Hide( this Panel panel )
	{
		panel.AddClass( "hidden" );
	}

	public static string GetMaterialGroup( this Team team ) => team switch
	{
		Team.BLUE => "Blue",
		Team.RED => "Red",
		Team.GREEN => "Green",
		Team.YELLOW => "Yellow",
		Team.GRAY => "Gray",
		_ => throw new ArgumentOutOfRangeException( nameof(team), team, null )
	};

	public static string GetFriendlyName( this Team team ) => team switch
	{
		Team.BLUE => "Blue",
		Team.RED => "Red",
		Team.GREEN => "Green",
		Team.YELLOW => "Yellow",
		Team.GRAY => "Gray",
		_ => throw new ArgumentOutOfRangeException( nameof(team), team, null )
	};

	public static void SetAvatar( this Image image, long steamId )
	{
		image.SetTexture( $"avatar:{steamId}" );
	}

	public static void SetAvatar( this Image image, Client client )
	{
		image.SetAvatar( client.PlayerId );
	}

	public static List<Participant> GetParticipants(this Team team)
	{
		return CurrentRound.Participants.Where( p => p.Team == team ).ToList();
	}

	public static List<Client> GetClients( this Team team )
	{
		return CurrentRound.Participants.Where( p => p.Team == team )
			.Select( p => p.Client )
			.ToList();
	}

	public static List<Tank> GetTanks( this Team team )
	{
		return Entity.All.OfType<Tank>()
			.Where( t => t.Team == team )
			.ToList();
	}

	public static bool IsInDevCam( this Client client )
	{
		return client.Components.Get<DevCamera>() != null;
	}
	
}

public class NamespaceExtensions
{
	public static Hud CurrentHud => (TankGame.Current as TankGame).Hud;

	public static Lobby CurrentLobby => (TankGame.Current as TankGame).Lobby;

	public static Round CurrentRound => (TankGame.Current as TankGame).Round;
	
	public static string Realm => Host.IsServer ? "Server" : "Client";
	
}
