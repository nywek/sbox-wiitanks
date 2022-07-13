using Sandbox;

namespace WiiTanks;

public partial class Tank
{
	public void SimulateMovement( Client cl )
	{
		if ( cl.Components.Get<CameraMode>() is not null || Components.Get<ArenaCameraMode>() is null )
		{
			// Client is using some other camera (dev cam?) => disable all movement
			return;
		}

		var camRot = Components.Get<ArenaCameraMode>().TargetRot;
		var movement = (Input.Forward * Vector3.Up * camRot) + (Input.Left * Vector3.Left * camRot);

		if ( !movement.IsNearZeroLength )
		{
			TargetRotation = Rotation.LookAt( movement, Vector3.Up );
		}

		Body.Rotation = Rotation.Lerp( Body.Rotation, TargetRotation, 0.2f );
		Head.Rotation = Rotation.LookAt( Input.Cursor.Origin.WithZ( 0 ) - Head.Position.WithZ( 0 ) );

		Velocity = movement.Normal * (Input.Down( InputButton.Run ) ? 1000f : DefaultSpeed);

		DebugOverlay.Sphere( Input.Cursor.Origin, 5f, Color.Red, 0f, false );

		var moveHelper = new MoveHelper( Position, Velocity );
		moveHelper.Trace = moveHelper.Trace.Ignore( Body );
		moveHelper.Trace = moveHelper.Trace.Size( Body.CollisionBounds );

		if ( moveHelper.TryMove( Time.Delta ) > 0 )
		{
			Position = moveHelper.Position;
		}
	}
}
