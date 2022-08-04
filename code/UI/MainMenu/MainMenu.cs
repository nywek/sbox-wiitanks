using Sandbox.UI;

namespace WiiTanks;

[UseTemplate]
public class MainMenu : Panel
{
	private BigBanner SoloBanner { get; set; }
	private BigBanner LobbyBanner { get; set; }
	private BigBanner EditorBanner { get; set; }

	public MainMenu()
	{
	}

	public void PlaySolo()
	{
		//Log.Debug( "PLAY SOLO" );
	}

	public void PlayLobby()
	{
		Hud.SwitchView( View.Lobby );
	}

	public void PlayEditor()
	{
		//Hud.SwitchView( View.ArenaEditor );
	}
}
