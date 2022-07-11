using Sandbox;
using SandboxEditor;

namespace WiiTanks;

[Library("wiitanks_arenaspawn", Group = "WiiTanks", Title = "Arena Spawnpoint"), HammerEntity]
[Title("Arena Spawnpoint")]
[Model(Model = "models/tank/tank.vmdl")]
public class ArenaSpawn : Entity
{
	[Property]
	public Team Team { get; set; }
}
