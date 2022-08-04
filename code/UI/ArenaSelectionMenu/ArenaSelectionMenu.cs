using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace WiiTanks;

[UseTemplate]
public class ArenaSelectionMenu : Panel
{
	private Panel ArenaContainer { get; set; }

	public ArenaSelectionMenu()
	{
		ArenaContainer.DeleteChildren();
		
		foreach ( var arena in Entity.All.OfType<Arena>() )
		{
			ArenaContainer.AddChild( new ArenaOption( arena ) );
		}
	}

	[Event.Hotload]
	private void OnHotload()
	{
		ArenaContainer.DeleteChildren();
		
		foreach ( var arena in Entity.All.OfType<Arena>() )
		{
			ArenaContainer.AddChild( new ArenaOption( arena ) );
		}
	}

	public void GoBack()
	{
		Hud.SwitchView( View.Lobby );
	}
}
