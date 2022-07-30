using Sandbox;

namespace WiiTanks;

public partial class Tank : ModelEntity
{
	[ConVar.ServerAttribute( "tank_speed", Min = 0f )]
	public static float DefaultSpeed { get; set; } = 200f;

	[ConVar.ServerAttribute( "tank_ammo", Min = 0 )]
	public static int MaxAmmo { get; set; } = 5;

	[ConVar.ServerAttribute( "tank_reload_speed", Min = 0 )]
	public static float ReloadSpeed = 1f;

	private static Color RespawningColor = new Color( 1f, 1f, 1f, 0.25f );
	private static Color AliveColor = Color.White;
	private static BBox HitboxBounds = new BBox( new(-27f, -23f, 0.5136f), new(27f, 23f, 75) );
	private static BBox MoveBounds = new BBox( new(-16.869f, -16.869f, 0.5136f), new(16.869f, 16.689f, 75) );

	[Net] public Team Team { get; set; }
	[Net] public ModelEntity Body { get; set; }
	[Net] public ModelEntity Head { get; set; }
	[Net, Predicted] public Rotation TargetRotation { get; set; }
	[Net] public int Ammo { get; set; }
	private TimeSince TimeSinceLastAmmoReplenish { get; set; }

	public Tank()
	{
		Transmit = TransmitType.Always;
	}

	public Tank( Client client, Team team )
	{
		Transmit = TransmitType.Always;
		client.Pawn = this;
		Team = team;
		LifeState = LifeState.Respawning;
	}

	public override void Spawn()
	{
		Body = new ModelEntity();
		Body.SetParent( this );
		Body.SetModel( "models/tank/tank_body.vmdl" );
		Body.Owner = this;

		Head = new ModelEntity();
		Head.SetParent( this );
		Head.SetModel( "models/tank/tank_head.vmdl" );
		Head.Owner = this;

		SetupPhysicsFromOBB( PhysicsMotionType.Static, HitboxBounds.Mins, HitboxBounds.Maxs );

		Tags.Add( "ArenaEntity" );

		Ammo = MaxAmmo;
	}

	public void SpawnAtArena( Arena arena, Vector3 pos )
	{
		Components.GetOrCreate<ArenaCameraMode>().UpdateFrom( arena );
		Position = pos;
		ResetInterpolation();
	}

	public override void Simulate( Client cl )
	{
		if ( Host.IsServer && LifeState == LifeState.Alive && Input.Pressed( InputButton.PrimaryAttack ) && Ammo > 0 )
		{
			Ammo--;
			TimeSinceLastAmmoReplenish = 0;

			var missile = new Missile();
			missile.Spawn( this );

			PlayShootEffect();
		}

		if ( Ammo < MaxAmmo && TimeSinceLastAmmoReplenish > ReloadSpeed )
		{
			TimeSinceLastAmmoReplenish = 0;
			Ammo++;
		}

		if ( LifeState == LifeState.Respawning )
		{
			Head.RenderColor = RespawningColor;
			Body.RenderColor = RespawningColor;
		}
		else if ( LifeState == LifeState.Alive )
		{
			Head.RenderColor = AliveColor;
			Body.RenderColor = AliveColor;
		}
		else if ( LifeState == LifeState.Dead )
		{
			Head.RenderColor = Color.Transparent;
			Body.RenderColor = Color.Transparent;
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

	public void Kill()
	{
		Host.AssertServer();
		PlayExplosionEffect();
		LifeState = LifeState.Dead;
		Tags.Add( "dead" );
	}
}
