using Sandbox;
using Sandbox.UI;
using WiiTanks.Extensions;
using WiiTanks.UI;

namespace WiiTanks;

public partial class Hud : HudEntity<RootPanel>
{
	[Net] public View CurrentView { get; set; } = View.MainMenu;

	// Clientside:
	private bool Initialized = false;
	private Background Background;
	private MainMenu MainMenu;
	private LobbyMenu LobbyMenu;
	private TestMenu TestMenu;
	private ArenaSelectionMenu ArenaSelectionMenu;

	public Hud()
	{
		if ( !IsClient ) { return; }

		RootPanel.StyleSheet.Load( "/UI/style.scss" );
	}

	[Event.Tick.Client]
	private void OnTick()
	{
		if ( !Initialized )
		{
			Init();
		}

		if ( CurrentRound is not null && CurrentRound.State == RoundState.Active )
		{
			if ( Input.Down( InputButton.Score ) )
			{
				LobbyMenu.Show();
			}
			else
			{
				LobbyMenu.Hide();
			}
		}
		
	}

	private void Init()
	{
		if ( Initialized ) { return; }
		Initialized = true;
		
		RootPanel.DeleteChildren();
		
		Background = RootPanel.AddChild<Background>();

		MainMenu = RootPanel.AddChild<MainMenu>();

		LobbyMenu = RootPanel.AddChild<LobbyMenu>();

		TestMenu = RootPanel.AddChild<TestMenu>();

		ArenaSelectionMenu = RootPanel.AddChild<ArenaSelectionMenu>();

		RootPanel.AddChild<ChatBox>();

		RootPanel.AddChild<Crosshair>();

		RootPanel.AddChild<CrosshairLine>();
		
		RootPanel.AddChild<Countdown>();

		RootPanel.AddChild<RoundEndBanner>();

		SwitchViewOnClient( (int)CurrentView );
	}

	[ConCmd.Server]
	public static void SwitchView( View newView )
	{
		Log.Debug($"Switching view to: {newView}");
		Host.AssertServer();
		CurrentHud.CurrentView = newView;
		CurrentHud.SwitchViewOnClient( (int)newView );
	}

	[ClientRpc]
	public void SwitchViewOnClient( int newViewId )
	{
		Show( (View)newViewId );
	}

	private void Show( View view )
	{
		switch ( view )
		{
			case View.TestMenu:
				Background.Show();
				TestMenu.Show();
				MainMenu.Hide();
				LobbyMenu.Hide();
				ArenaSelectionMenu.Hide();
				break;
			case View.MainMenu:
				Background.Show();
				TestMenu.Hide();
				MainMenu.Show();
				LobbyMenu.Hide();
				ArenaSelectionMenu.Hide();
				break;
			case View.Lobby:
				Background.Hide();
				TestMenu.Hide();
				MainMenu.Hide();
				LobbyMenu.Show();
				ArenaSelectionMenu.Hide();
				break;
			case View.Ingame:
			case View.ArenaEditor:
				Background.Hide();
				TestMenu.Hide();
				MainMenu.Hide();
				LobbyMenu.Hide();
				ArenaSelectionMenu.Hide();
				break;
			case View.ArenaSelection:
				Background.Hide();
				TestMenu.Hide();
				MainMenu.Hide();
				LobbyMenu.Hide();
				ArenaSelectionMenu.Show();
				break;
		}
	}

	/*[Event.Hotload]
	public void OnHotload()
	{
		if ( !IsClient ) return;
		InitLater();
	}

	private async void InitLater()
	{
		await Task.Delay( 5000 );
		Initialized = false;
	}*/
}
