using Sandbox;
using Sandbox.UI;

namespace WiiTanks;

[UseTemplate]
public class LobbyMenu : Panel
{
	public void GoBack()
	{
		Hud.SwitchView( View.MainMenu );
	}

	public void BeginRound()
	{
		ConsoleSystem.Run( "lobby_begin_round" );
	}
}
