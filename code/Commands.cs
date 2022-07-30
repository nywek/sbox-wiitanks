using Sandbox;

namespace WiiTanks;

public class Commands
{
	public static TankGame Game => Sandbox.Game.Current as TankGame;

	[ConCmd.Server("testcmd")]
	public static void TestCmd()
	{
		Log.Info("TestCmd()");

		if (Game.Lobby is null)
		{
			Game.Lobby = new Lobby();
		}

		Log.Info($"Lobby: {Game.Lobby}");

		foreach (var lobbySlot in Game.Lobby.Slots)
		{
			Log.Info($"- {lobbySlot}");
		}
	}

	[ConCmd.Server("testa")]
	public static void testa()
	{
		Log.Info("a()");

		var bot = new Bot();
		Game.Lobby.Join(bot.Client);
	}

	[ConCmd.Server("arena")]
	public static void Arena()
	{
		Log.Info("Generate Arena");

		var grid = new ArenaGrid();
		grid.Debug();
	}
}
