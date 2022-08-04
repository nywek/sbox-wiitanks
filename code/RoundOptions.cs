using System.Text.Json.Serialization;
using Sandbox;

namespace WiiTanks;

public record struct RoundOptions
{
	public int SelectedArenaId { get; set; }
	public bool MinesEnabled { get; set; }
	public int MaxAmmo { get; set; }
	public float TankSpeed { get; set; }
	public int MissileBounces { get; set; }

	[JsonIgnore]
	public Arena SelectedArena => Entity.FindByIndex( SelectedArenaId ) as Arena;
}
