using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace WiiTanks.UI;

public class CrosshairLine : Panel
{
	private const float LineGap = 75f;

	private readonly List<Image> _dots = new();

	public CrosshairLine()
	{
		StyleSheet.Load( "/UI/Crosshair/CrosshairLine.scss" );
	}

	[Event.Frame]
	public void Update()
	{
		if ( Local.Pawn is not Tank tank || CurrentRound is null )
		{
			DeleteLine();
			Style.Opacity = 0;
			return;
		}

		if ( Local.Client.Components.Get<ArenaCameraMode>() is null )
		{
			DeleteLine();
			Style.Opacity = 0;
			return;
		}

		if ( tank.LifeState == LifeState.Dead )
		{
			DeleteLine();
			Style.Opacity = 0;
			return;
		}

		Style.Opacity = 1;
		DisplayLine( FromFractionToPixels( GetCannonPos( tank ) ), Mouse.Position );
	}

	public void DisplayLine( Vector2 from, Vector2 to )
	{
		var dir = (to - from).Normal;
		var dist = (to - from).Length;

		int dotCount = (int)(dist / LineGap);
		var realGap = dist / dotCount;

		DeleteDots( Math.Max( 0, dotCount - 1 ) ); // delete any unnecessary dots

		for ( int i = 1; i < dotCount; i++ )
		{
			var len = realGap * i;
			var pos = from + (dir * len);
			DisplayDot( pos, i - 1 );
		}
	}

	public void DisplayDot( Vector2 pos, int index )
	{
		Image img;

		if ( _dots.Count <= index )
		{
			img = Add.Image( "/ui/crosshair_dot.png", "crosshair-dot" );
			_dots.Insert( index, img );
		}
		else
		{
			img = _dots[index];
		}

		// Position div
		img.Style.Position = PositionMode.Absolute;
		img.Style.Left = Length.Fraction( pos.x / Screen.Width );
		img.Style.Top = Length.Fraction( pos.y / Screen.Height );

		// Center div
		var transform = new PanelTransform();
		transform.AddTranslateX( Length.Fraction( -0.5f ) );
		transform.AddTranslateY( Length.Fraction( -0.5f ) );
		img.Style.Transform = transform;
	}

	public void DeleteDots( int count )
	{
		if ( _dots.Count <= count ) { return; }

		for ( int i = _dots.Count - 1; i >= count; i-- )
		{
			_dots[i].Delete( true );
			_dots.RemoveAt( i );
		}
	}

	public void DeleteLine()
	{
		foreach ( var dot in _dots )
		{
			dot.Delete( true );
		}

		_dots.Clear();
	}

	private Vector2 FromFractionToPixels( Vector2 pos )
	{
		return new Vector2( pos.x * Screen.Width, pos.y * Screen.Height );
	}

	private Vector2 GetCannonPos( Tank tank )
	{
		var cannonWorldPos = tank.Head.GetAttachment( "cannon" )?.Position ?? Vector3.Zero;
		var cannonScreenPos = cannonWorldPos.ToScreen();
		var cannonPos = new Vector2( cannonScreenPos.x, cannonScreenPos.y );
		
		return cannonPos;
	}
}
