using Sandbox;

namespace WiiTanks.Extensions;

public static partial class Events
{
	public static void Broadcast( string name )
	{
		ClientEvent( To.Everyone, name );
		Event.Run( name );
	}

	[ClientRpc]
	public static void ClientEvent( string name ) { Event.Run( name ); }

	// ----------------------------------------------------------------------------------------------------------------

	public static void Broadcast( string name, RoundState arg )
	{
		ClientEvent( To.Everyone, name, arg );
		Event.Run( name, arg );
	}

	[ClientRpc]
	public static void ClientEvent( string name, RoundState arg ) { Event.Run( name, arg ); }

	// ----------------------------------------------------------------------------------------------------------------

	public static void Broadcast( string name, int arg )
	{
		ClientEvent( To.Everyone, name, arg );
		Event.Run( name, arg );
	}

	[ClientRpc]
	internal static void ClientEvent( string name, int arg ) { Event.Run( name, arg ); }
	
	// ----------------------------------------------------------------------------------------------------------------

	public static void Broadcast( string name, Team? arg )
	{
		ClientEventTeam( To.Everyone, name, arg is null ? -1 : (int) arg );
		Event.Run( name, arg );
	}

	[ClientRpc]
	public static void ClientEventTeam( string name, int arg )
	{
		Team? team = arg < 0 ? null : (Team)arg;
		Event.Run( name, team );
	}
}
