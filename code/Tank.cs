using Sandbox;

namespace WiiTanks;

public partial class Tank : Entity
{
	[ConVar.ServerAttribute( "tank_speed", Min = 0f )]
	public static float DefaultSpeed { get; set; } = 200f;

	[Net] public Team Team { get; set; }
	[Net] public ModelEntity Body { get; set; }
	[Net] public ModelEntity Head { get; set; }
	[Net, Predicted] public Rotation TargetRotation { get; set; }

	public Tank()
	{
		Transmit = TransmitType.Always;
	}

	public Tank( Client client, Team team )
	{
		Transmit = TransmitType.Always;
		client.Pawn = this;
		Team = team;
	}

	public override void Spawn()
	{
		Body = new ModelEntity();
		Body.SetParent( this );
		Body.SetModel( "models/tank/tank_body.vmdl" );
		Body.Owner = this;
		Body.Tags.Add( "TankBody" );

		var bbox = new BBox( // Temporarily use bbox
			new(-27f, -23f, 0.5136f),
			new(27f, 23f, 75)
		);

		Body.SetupPhysicsFromOBB( PhysicsMotionType.Static, bbox.Mins, bbox.Maxs );

		Head = new ModelEntity();
		Head.SetParent( this );
		Head.SetModel( "models/tank/tank_head.vmdl" );
		Head.Owner = this;
		Head.Tags.Add( "TankHead" );
	}

	public void SpawnAtArena( Arena arena, Vector3 pos )
	{
		Components.GetOrCreate<ArenaCameraMode>().UpdateFrom( arena );
		Position = pos;
		ResetInterpolation();
	}

	public override void Simulate( Client cl )
	{
		if ( Host.IsServer && Input.Pressed( InputButton.PrimaryAttack ) )
		{
			var missile = new Missile();
			missile.Spawn( this );

			PlayShootEffect();
		}

		SimulateMovement( cl );
	}

	public override void BuildInput( InputBuilder inputBuilder )
	{
		base.BuildInput( inputBuilder );

		var cannonHeight = Head.GetAttachment( "cannon" )?.Position.z ?? 0f; // 103.3159f
		Vector3 direction = Screen.GetDirection( Mouse.Position );
		float distance = (cannonHeight - Input.Position.z) / direction.z;
		var mousePos = Input.Position + (distance * direction);

		inputBuilder.Cursor.Origin = mousePos;
	}

	protected override void OnDestroy()
	{
		PlayExplosionEffect();
	}
}
