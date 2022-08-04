using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace WiiTanks.UI;

public class Countdown : Panel
{
	private List<Label> _labels = new();
	private int _labelIndex = 0;
	private TimeUntil _timeUntilNextLabel = 0;

	public Countdown()
	{
		AddClass("countdown");
		StyleSheet.Load( "/UI/Countdown/Countdown.scss" );

		_labels.Add( Add.Label( "3" ) );
		_labels.Add( Add.Label( "2" ) );
		_labels.Add( Add.Label( "1" ) );
		_labels.Add( Add.Label( "START!" ) );
		_labels.Add( Add.Label( "" ) );
		_labels.Add( Add.Label( "" ) );

		Style.Opacity = 0;
	}

	[Event( "round.statechange" )]
	public void BeginCountdown( RoundState newState )
	{
		if (newState != RoundState.Preparing)
			return;

		_labelIndex = 0;
		_timeUntilNextLabel = 0;

		Style.Opacity = 1;
	}

	public override void Tick()
	{
		if ( !_timeUntilNextLabel || _labelIndex >= _labels.Count )
			return;

		_labels[_labelIndex].AddClass( "count-in" );

		if ( _labelIndex > 0 )
		{
			_labels[_labelIndex - 1].RemoveClass( "count-in" );
			_labels[_labelIndex - 1].AddClass( "count-out" );
		}

		if ( _labelIndex > 1 )
		{
			_labels[_labelIndex - 2].RemoveClass( "count-out" );
		}

		_labelIndex++;
		_timeUntilNextLabel = 1;
	}
}
