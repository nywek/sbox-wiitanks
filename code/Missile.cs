using Sandbox;

namespace WiiTanks;

public partial class Missile : ModelEntity
{
	[ConVar.ServerAttribute( "missile_speed", Min = 0 )]
	public static float Speed { get; set; } = 300f;

	[ConVar.ServerAttribute( "missile_max_bounces", Min = 0 )]
	public static int MaxBounces { get; set; } = 1;

	[Net] public Tank? Source { get; set; }
	[Net] public Vector3 Direction { get; set; }

	private int Bounces;

	public Missile() { Transmit = TransmitType.Always; }

	public void Spawn( Tank source )
	{
		SetModel( "models/missile/missile.vmdl" );

		Source = source;

		Position = Source.Head.GetAttachment( "cannon" )?.Position ?? Source.Position;
		Rotation = Source.Head.Rotation;
		Direction = Vector3.Forward * Rotation;

		PlaySpawnSound();
		
		Tags.Add( "ArenaEntity" );
	}

	public override void ClientSpawn()
	{
		Particles.Create( "particles/missile_flames.vpcf", this, "end" );
	}

	[Event.Tick.Server]
	public void OnServerTick()
	{
		var newPos = Position + (Direction * Speed * Time.Delta);
		var tr = Trace.Ray( Position, newPos )
			.Ignore( Source?.Body )
			.WithoutTags( "dead" )
			.WithAnyTags( "ArenaEntity", "solid" )
			.UseHitboxes( true )
			.Run();

		if ( !tr.Hit )
		{
			Position = newPos;
			return;
		}

		// Check what the missile hit
		if ( tr.Entity is WorldEntity || tr.Entity is Box )
		{
			Bounces++;

			if ( Bounces > MaxBounces )
			{
				DeleteAsync( 0.1f );
			}
			else
			{
				var newDirection = Vector3.Reflect( Direction, tr.Normal );
				Direction = newDirection;
				Rotation = Rotation.LookAt( newDirection );
				PlayBounceEffect();
			}
		}
		else if ( tr.Entity is Tank t )
		{
			t.Kill();
			DeleteAsync( 0.1f );
		}
	}

	protected override void OnDestroy()
	{
		PlayExplodeEffect();
	}
}
