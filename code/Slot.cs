using Sandbox;

namespace WiiTanks;

public partial class Slot : BaseNetworkable
{
	[Net] public int Index { get; set; }

	[Net, Change( nameof(OnSlotChanged) )] public Client Client { get; set; }

	[Net, Change( nameof(OnSlotChanged) )] public Team Team { get; set; }

	public bool IsOccupied => Client is not null;

	public Slot() { }

	public Slot( int index )
	{
		Index = index;
		Team = (Team)index;
	}

	private void OnSlotChanged()
	{
		Event.Run( "lobby.slot.updated", Index );
	}

	public override string ToString()
	{
		return $"Slot({Index}, {Team}, {Client})";
	}
}
