using System.Collections.Generic;

namespace Sandbox;

public partial class Box : ModelEntity
{
	[Net] public Material Material { get; set; }
	[Net] public float Size { get; set; }

	public override void ClientSpawn()
	{
		base.ClientSpawn();
		Create();
	}

	public override void Spawn()
	{
		base.Spawn();
		Create();
	}

	public void Create()
	{
		// ...
		var mesh = new Mesh( Material );
		BuildMesh( mesh, Size );

		var model = Model.Builder
			.AddMesh( mesh )
			.AddCollisionBox( Size / 2 )
			.Create();

		Model = model;

		SetupPhysicsFromModel(PhysicsMotionType.Static);
	}

	[Event.Tick.Client]
	public void DebugBoundingBox()
	{
		DebugOverlay.Box(this, Color.Red);
	}

	private void BuildMesh( Mesh mesh, float Size )
	{
		var Origin = Position;

		var positions = new Vector3[]
	{
				new Vector3(-0.5f, -0.5f, 0.5f) * Size,
				new Vector3(-0.5f, 0.5f, 0.5f) * Size,
				new Vector3(0.5f, 0.5f, 0.5f) * Size,
				new Vector3(0.5f, -0.5f, 0.5f) * Size,
				new Vector3(-0.5f, -0.5f, -0.5f) * Size,
				new Vector3(-0.5f, 0.5f, -0.5f) * Size,
				new Vector3(0.5f, 0.5f, -0.5f) * Size,
				new Vector3(0.5f, -0.5f, -0.5f) * Size,
	};

		var faceIndices = new int[]
		{
				0, 1, 2, 3,
				7, 6, 5, 4,
				0, 4, 5, 1,
				1, 5, 6, 2,
				2, 6, 7, 3,
				3, 7, 4, 0,
		};

		var uAxis = new Vector3[]
		{
				Vector3.Forward,
				Vector3.Left,
				Vector3.Left,
				Vector3.Forward,
				Vector3.Right,
				Vector3.Backward,
		};

		var vAxis = new Vector3[]
		{
				Vector3.Left,
				Vector3.Forward,
				Vector3.Down,
				Vector3.Down,
				Vector3.Down,
				Vector3.Down,
		};

		List<SimpleVertex> verts = new();
		List<int> indices = new();

		for ( var i = 0; i < 6; ++i )
		{
			var tangent = uAxis[i];
			var binormal = vAxis[i];
			var normal = Vector3.Cross( tangent, binormal );

			for ( var j = 0; j < 4; ++j )
			{
				var vertexIndex = faceIndices[(i * 4) + j];
				var pos = positions[vertexIndex];

				verts.Add( new SimpleVertex()
				{
					position = pos,
					normal = normal,
					tangent = tangent,
					texcoord = Planar( (Origin + pos) / 32, uAxis[i], vAxis[i] )
				} );
			}

			indices.Add( i * 4 + 0 );
			indices.Add( i * 4 + 2 );
			indices.Add( i * 4 + 1 );
			indices.Add( i * 4 + 2 );
			indices.Add( i * 4 + 0 );
			indices.Add( i * 4 + 3 );
		}

		mesh.CreateVertexBuffer<SimpleVertex>( verts.Count, SimpleVertex.Layout, verts.ToArray() );
		mesh.CreateIndexBuffer( indices.Count, indices.ToArray() );
	}

	protected static Vector2 Planar( Vector3 pos, Vector3 uAxis, Vector3 vAxis )
	{
		return new Vector2()
		{
			x = Vector3.Dot( uAxis, pos ),
			y = Vector3.Dot( vAxis, pos )
		};
	}
}
