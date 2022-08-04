using Sandbox;
using Sandbox.UI;

namespace WiiTanks;

[UseTemplate]
public class ArenaOption : Panel
{
	private RadarImage RadarImage { get; set; }

	private Arena Arena => RadarImage.Arena;
	
	public ArenaOption() { }

	public ArenaOption( Arena arena )
	{
		RadarImage.Arena = arena;
		SetClass( "selected", arena.NetworkIdent == CurrentLobby.Options.SelectedArenaId );
		SetClass( "disabled", !Local.Client.IsListenServerHost );
	}

	protected override void OnClick( MousePanelEvent e )
	{
		if ( !Local.Client.IsListenServerHost || Arena is null )
			return;

		Commands.UpdateLobbyOptions( CurrentLobby.Options with { SelectedArenaId = Arena.NetworkIdent } );
	}

	[Event( "lobby.options.updated" )]
	private void OnOptionsUpdated()
	{
		if (Arena is null)
			return;
		
		SetClass( "selected", Arena.NetworkIdent == CurrentLobby.Options.SelectedArenaId );
		SetClass( "disabled", !Local.Client.IsListenServerHost );
	}
}
