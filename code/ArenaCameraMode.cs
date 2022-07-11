using Sandbox;

namespace WiiTanks;

public partial class ArenaCameraMode : CameraMode
{
	[Net] public Vector3 TargetPos { get; set; }= new(0, 0, 0);
	[Net] public Rotation TargetRot { get; set; } = Rotation.LookAt( Vector3.Down, Vector3.Forward );

	public void UpdateFrom( Arena arena )
	{
		var cameraEntity = arena.FindCamera();
		
		if ( cameraEntity is CameraEntity cam )
		{
			TargetPos = cam.Position;
			TargetRot = cam.Rotation;
		}
		else
		{
			// Find center of arena
			var centerPos = arena.CollisionBounds.Center + arena.Position;
			centerPos.z = arena.CollisionBounds.Maxs.z + arena.Position.z;

			TargetPos = centerPos;
			TargetRot = Rotation.LookAt( Vector3.Down, Vector3.Forward );
		}
	}
	
	public override void Update()
	{
		Position = TargetPos;//.WithZ( 300 );
		Rotation = TargetRot;
	}
}
