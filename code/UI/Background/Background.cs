using Sandbox;
using Sandbox.UI;

namespace WiiTanks;

public class Background : Panel
{
	public Background()
	{
		AddClass( "bg" );
		StyleSheet.Load( "/UI/Background/Background.scss" );
	}

	[Event.Tick.Client]
	public void OnTick()
	{
		Style.BackgroundPositionX = RealTime.Now * -25f;
		Style.BackgroundPositionY = RealTime.Now * -25f;
		Style.BackgroundRepeat = BackgroundRepeat.Repeat;
	}
}
