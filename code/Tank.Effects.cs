using Sandbox;

namespace WiiTanks;

public partial class Tank
{
	[ClientRpc]
	public void PlayExplosionEffect()
	{
		Particles.Create( "particles/tank_explosion/tank_explosion.vpcf", Position );
	}

	[ClientRpc]
	private void PlayShootEffect()
	{
		Particles.Create( "particles/tank_shoot.vpcf", Head, "cannon" );
		Particles.Create( "particles/shoot_smoke.vpcf", Head, "cannon" );
	}
}
