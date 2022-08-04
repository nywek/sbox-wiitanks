using Sandbox;
using Sandbox.UI;

namespace WiiTanks.UI;

[UseTemplate]
public class LobbyOptionsPanel : Panel
{
	public ToggleBox MinesToggle { get; set; }
	public Slider MaxAmmoSlider { get; set; }
	public Label MaxAmmoNumber { get; set; }
	public Slider TankSpeedSlider { get; set; }
	public Label TankSpeedNumber { get; set; }
	public Slider BounceSlider { get; set; }
	public Label BounceNumber { get; set; }

	public LobbyOptionsPanel()
	{
		MinesToggle.Checked = false;
		MaxAmmoSlider.Value = Tank.MaxAmmo;
		TankSpeedSlider.Value = Tank.DefaultSpeed;
		BounceSlider.Value = Missile.MaxBounces;

		if ( Local.Client.IsListenServerHost )
		{
			MinesToggle.AddEventListener( "onchange", e => OnOptionsChangedLocally() );
			MaxAmmoSlider.AddEventListener( "onchange", e => OnOptionsChangedLocally() );
			TankSpeedSlider.AddEventListener( "onchange", e => OnOptionsChangedLocally() );
			BounceSlider.AddEventListener( "onchange", e => OnOptionsChangedLocally() );
		}
	}

	public override void Tick()
	{
		MaxAmmoNumber.SetText( MaxAmmoSlider.Value.ToString() );
		TankSpeedNumber.SetText( TankSpeedSlider.Value.ToString() );
		BounceNumber.SetText( BounceSlider.Value.ToString() );
	}

	private void OnOptionsChangedLocally()
	{
		Commands.UpdateLobbyOptions( new RoundOptions
		{
			MaxAmmo = MaxAmmoSlider.Value.FloorToInt(),
			MinesEnabled = MinesToggle.Checked,
			MissileBounces = BounceSlider.Value.FloorToInt(),
			SelectedArenaId = CurrentLobby.Options.SelectedArenaId,
			TankSpeed = TankSpeedSlider.Value
		} );
	}

	[Event( "lobby.options.updated" )]
	private void OnOptionsUpdated()
	{
		if ( Local.Client.IsListenServerHost )
			return;

		MinesToggle.Checked = CurrentLobby.Options.MinesEnabled;
		MaxAmmoSlider.Value = CurrentLobby.Options.MaxAmmo;
		TankSpeedSlider.Value = CurrentLobby.Options.TankSpeed;
		BounceSlider.Value = CurrentLobby.Options.MissileBounces;
	}

	public void SelectSomeArena()
	{
		if ( !Local.Client.IsListenServerHost )
			return;

		Hud.SwitchView( View.ArenaSelection );
	}
}
