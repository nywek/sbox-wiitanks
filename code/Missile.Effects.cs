using Sandbox;

namespace WiiTanks;

public partial class Missile
{
	[ClientRpc]
	public void PlayExplodeEffect()
	{
		Particles.Create( "particles/missile_explosion/missile_explosion.vpcf", Position );
		PlaySound( "sounds/missile_impact.sound" );
	}

	[ClientRpc]
	public void PlayBounceEffect()
	{
		PlaySound( "sounds/missile_bounce.sound" );
	}

	[ClientRpc]
	public void PlaySpawnSound()
	{
		PlaySound( "sounds/tank_shoot.sound" );
	}
}
