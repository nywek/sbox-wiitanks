using Sandbox;

namespace WiiTanks;

public partial class ArenaCameraMode : CameraMode
{
	[Net] public Vector3 TargetPos { get; set; } = new(0, 0, 0);
	[Net] public Rotation TargetRot { get; set; } = Rotation.LookAt( Vector3.Down, Vector3.Forward );

	private int _currentArenaIdent = -1;

	public ArenaCameraMode()
	{
		WatchCurrentLobbyArena();
	}

	public override void Update()
	{
		Position = Position.LerpTo(TargetPos, 0.05f);
		Rotation = Rotation.Slerp( Rotation, TargetRot, 0.05f );
	}

	[Event( "lobby.options.updated" )]
	public void OnOptionsUpdated()
	{
		WatchCurrentLobbyArena();
	}

	private void WatchCurrentLobbyArena()
	{
		if ( Host.IsClient )
			return;

		var arena = CurrentLobby.Options.SelectedArena;
		
		if ( _currentArenaIdent != arena.NetworkIdent )
			WatchArena( arena );
	}

	private void WatchArena( Arena arena )
	{
		_currentArenaIdent = arena.NetworkIdent;
		
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
}
