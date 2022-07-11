using System.Collections.Generic;
using System.Linq;
using Sandbox;
using SandboxEditor;

namespace WiiTanks;

[Library("wiitanks_arena", Group = "WiiTanks", Title = "Arena"), HammerEntity, Solid]
[AutoApplyMaterial("materials/tools/toolsinvisible.vmat")]
[VisGroup( VisGroup.Logic )]
[Title("Arena")]
public partial class Arena : BrushEntity
{
	public override void Spawn()
	{
		base.Spawn();
		Solid = false;
	}

	[Event.Tick]
	public void OnTick()
	{
		DebugOverlay.Box( Position, Rotation, CollisionBounds.Mins, CollisionBounds.Maxs, Color.Blue );
	}

	public IEnumerable<ArenaSpawn> FindSpawnPoints()
	{
		foreach (var arenaSpawn in Entity.All.OfType<ArenaSpawn>())
		{
			var relativePos = arenaSpawn.Position - Position;
			var relativeBBox = BBox.FromPositionAndSize( relativePos, 1 );

			if ( CollisionBounds.Contains( relativeBBox ) )
			{
				yield return arenaSpawn;
			}
		}
	}

	public CameraEntity? FindCamera()
	{
		foreach (var cam in Entity.All.OfType<CameraEntity>())
		{
			var relativePos = cam.Position - Position;
			var relativeBBox = BBox.FromPositionAndSize( relativePos, 1 );

			if ( CollisionBounds.Contains( relativeBBox ) )
			{
				return cam;
			}
		}

		return null;
	}
	
}
