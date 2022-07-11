using Sandbox;

namespace WiiTanks;

public partial class Missile : ModelEntity
{
	public static float SPEED { get; set; } = 300f;

	[Net] public Tank Source { get; set; }
	[Net] public Vector3 Direction { get; set; }

	public Missile() { Transmit = TransmitType.Always; }

	public void Spawn( Tank source )
	{
		SetModel( "models/missile/missile.vmdl" );

		Source = source;

		Position = Source.Head.GetAttachment( "cannon" )?.Position ?? Source.Position;
		Rotation = Source.Head.Rotation;
		Direction = Vector3.Forward * Rotation;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
		//Particles.Create( "particles/missile_trail.vpcf", this, "end" );
		Particles.Create( "particles/missile_flames.vpcf", this, "end" );
		//Particles.Create( "particles/shoot_particle.vpcf", this, "end" );
		//Particles.Create( "particles/tank_shoot.vpcf", this, "end" );
	}

	[Event.Tick.Server]
	public void OnServerTick()
	{
		var newPos = Position + (Direction * SPEED * Time.Delta);
		var tr = Trace.Ray( Position, newPos )
			.Ignore( Source.Body )
			.UseHitboxes( true )
			.Run();
		
		if ( !tr.Hit )
		{
			Position = newPos;
			return;
		}

		// Check what the missile hit
		if ( tr.Entity is WorldEntity )
		{
			Explode();
			
			// bounce
			//var newDirection = Vector3.Reflect( Direction, tr.Normal );
			//Direction = newDirection;
			//Rotation = Rotation.LookAt( newDirection );
		}
		else if ( tr.Entity is ModelEntity )
		{
			if ( tr.Entity.Tags.Has( "TankBody" ) )
			{
				Tank tank = tr.Entity.Parent as Tank;
				Log.Info( $"Found tank: {tank}" );
				tank.Explode();
			}
			// TODO
			Explode();
		}
		else
		{
			Log.Info( $"TR: {tr}" );
		}
	}

	public void Explode()
	{
		DeleteAsync( 0.1f );
		PlayExplodeEffect();
	}

	[ClientRpc]
	public void PlayExplodeEffect()
	{
		Particles.Create( "particles/missile_explosion/missile_explosion.vpcf", Position );
	}
}
