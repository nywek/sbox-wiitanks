using Sandbox;

namespace WiiTanks;

public partial class Participant : BaseNetworkable
{
	[Net] public Client Client { get; set; }
	[Net] public Team Team { get; set; }

	public Participant() { }
	
	public Participant( Client client, Team team )
	{
		Client = client;
		Team = team;
	}

}
