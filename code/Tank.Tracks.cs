using System.Linq;
using Sandbox;

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
		var decalDef = ResourceLibrary.Get<DecalDefinition>( "decals/tracks.decal" );
		var worldEntity = Entity.All.OfType<WorldEntity>().First();
		Rotation rot = Body.Rotation;

		Decal.Place( decalDef, worldEntity, -1, pos, rot );
	}
}
