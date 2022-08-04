using Sandbox;
using Sandbox.UI;

namespace WiiTanks.UI;

public partial class Crosshair : Panel
{
	public Crosshair()
	{
		AddClass( "crosshair" );
		StyleSheet.Load( "/UI/Crosshair/Crosshair.scss" );
	}

	public override void Tick()
	{
		if ( Local.Pawn is not Tank tank || CurrentRound is null )
		{
			Style.Opacity = 0;
			return;
		}

		Style.Opacity = 1;

		var ammo = tank.Ammo;

		if ( tank.LifeState == LifeState.Dead || CurrentRound.State == RoundState.Preparing )
		{
			ammo = 0;
		}

		for ( int i = 0; i < Tank.MaxAmmo; i++ )
		{
			SetClass( $"ammo-{i}", ammo == i );
		}
		
		var position = Mouse.Position;

		Style.Left = Length.Fraction( position.x / Screen.Width );
		Style.Top = Length.Fraction( position.y / Screen.Height );

		var transform = new PanelTransform();
		transform.AddTranslateX( Length.Fraction( -0.5f ) );
		transform.AddTranslateY( Length.Fraction( -0.5f ) );
		Style.Transform = transform;
	}

}
