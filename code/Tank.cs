using Sandbox;

namespace WiiTanks;

public partial class Tank : Entity
{
	[ConVar.ServerAttribute("tank_speed", Min = 0f)]
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

		var bbox = new BBox(
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

	private Vector3 mousePos;
	
	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		//var bbox = new BBox(
		//	new(-27f, -23f, 20.5136f),
		//	new(27f, 23f, 75)
		//);
		//bbox *= 0.5f;
		//DebugOverlay.Box( Position, bbox.Mins, bbox.Maxs, Color.Blue );
		DebugOverlay.Box( Body, Color.Blue );
		
		if ( cl.IsBot )
		{
			Position = Position.WithZ( 90 );
			return;
		}

		if ( Host.IsServer && Input.Pressed( InputButton.PrimaryAttack ) )
		{
			var missile = new Missile();
			missile.Spawn( this );

			PlayShootEffect();

			//var missileCam = new MissileCameraMode();
			//missileCam.TargetMissile = missile;
			//Components.RemoveAny<ArenaCameraMode>();
			//Components.Add( missileCam );
		}
		
		if ( cl.Components.Get<CameraMode>() is not null || Components.Get<ArenaCameraMode>() is null )
		{
			// Client is using some other camera (dev cam?)
			return;
		}

		var camRot = Components.Get<ArenaCameraMode>().TargetRot;
		var movement = (Input.Forward * Vector3.Up * camRot) + (Input.Left * Vector3.Left * camRot);

		if ( !movement.IsNearZeroLength )
		{
			TargetRotation = Rotation.LookAt( movement, Vector3.Up );
		}
		
		Body.Rotation = Rotation.Lerp( Body.Rotation, TargetRotation, 0.2f );
		Head.Rotation = Rotation.LookAt( Input.Cursor.Origin.WithZ( 0 ) - Head.Position.WithZ( 0 ) );

		Velocity = movement.Normal * (Input.Down( InputButton.Run ) ? 1000f : DefaultSpeed);
		
		DebugOverlay.Sphere( Input.Cursor.Origin, 5f, Color.Red, 0f, false );

		var moveHelper = new MoveHelper( Position, Velocity );
		moveHelper.Trace = moveHelper.Trace.Ignore( Body );
		moveHelper.Trace = moveHelper.Trace.Size( Body.CollisionBounds );

		if ( moveHelper.TryMove( Time.Delta ) > 0 )
		{
			Position = moveHelper.Position;
		}
	}

	public override void BuildInput( InputBuilder inputBuilder )
	{
		base.BuildInput( inputBuilder );

		var cannonHeight = Head.GetAttachment( "cannon" )?.Position.z ?? 0f; //103.3159f);
		Vector3 direction = Screen.GetDirection( Mouse.Position );
		float distance = (cannonHeight - Input.Position.z) / direction.z;
		var mousePos = Input.Position + (distance * direction);
		
		inputBuilder.Cursor.Origin = mousePos;
	}

	[ClientRpc]
	private void PlayShootEffect()
	{
		Particles.Create( "particles/tank_shoot.vpcf", Head, "cannon" );
		Particles.Create( "particles/shoot_smoke.vpcf", Head, "cannon" );
	}

	public void Explode()
	{
		PlayExplosionEffect();
		DeleteAsync( 0.1f );
	}

	[ClientRpc]
	public void PlayExplosionEffect()
	{
		Particles.Create( "particles/tank_explosion/tank_explosion.vpcf", Position );
	}
}
