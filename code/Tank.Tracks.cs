using System.Diagnostics.Tracing;
using System.Linq;
using Sandbox;
using Sandbox.Internal;

namespace WiiTanks;

public partial class Tank
{
	private Vector3 lastLeftTrackPos = Vector3.Zero;
	private Vector3 lastRightTrackPos = Vector3.Zero;
	private const float maxDecalDistance = 10;
	
	[Event.Tick.Client]
	public void OnClientTick()
	{
		var pos = Body.GetAttachment( "left_track_source" )?.Position ?? lastLeftTrackPos;

		if ( lastLeftTrackPos.Distance( pos ) > maxDecalDistance )
		{
			PlaceTrack( pos );
			lastLeftTrackPos = pos;
		}

		pos = Body.GetAttachment( "right_track_source" )?.Position ?? lastRightTrackPos;

		if ( lastRightTrackPos.Distance( pos ) > maxDecalDistance )
		{
			PlaceTrack( pos );
			lastRightTrackPos = pos;
		}
	}

	private void PlaceTrack( Vector3 pos )
	{
		Material material = Material.Load( "materials/tracks/tracks.vmat" );

		var worldEntity = Entity.All.OfType<WorldEntity>().First();
		//WorldEntity worldEntity = PhysicsWorld.WorldBody?.Entity as WorldEntity;

		Rotation rot = Body.Rotation;
		Vector3 scale = new Vector3( 10, 8, 5 );

		Decals.Place( material, worldEntity, -1, pos, scale, rot );
	}
}
