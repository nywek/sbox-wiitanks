using Sandbox;

namespace WiiTanks;

public partial class MissileCameraMode : CameraMode
{
	[Net] public Missile TargetMissile { get; set; }
	
	public override void Update()
	{
		if ( TargetMissile is null ) { return; }

		Position = TargetMissile.Position.WithZ( TargetMissile.Position.z + 400 );
		Rotation = TargetMissile.Rotation * Rotation.LookAt( Vector3.Down );
	}
}
