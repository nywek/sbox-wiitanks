using System;
using System.Linq;
using Sandbox;
using Sandbox.UI;
using WiiTanks.Extensions;

namespace WiiTanks;

[UseTemplate]
public class SlotPanel : Panel
{
	private static int IndexCounter = 0;

	public Image AvatarImage { get; set; }
	public Label NameLabel { get; set; }
	public IconPanel SwitchTeamButton { get; set; }
	public IconPanel LeaveButton { get; set; }

	private Slot _slot;
	private bool _localUserHasSlot => CurrentLobby.Slots.Any( s => s.Client == Local.Client );

	public SlotPanel()
	{
		AvatarImage.SetAvatar( 76561198024674549 );
		
		Log.Info( $"Accessing slot: {IndexCounter} where total count = {CurrentLobby.Slots.Count}" );

		_slot = CurrentLobby.Slots[IndexCounter];

		Log.Debug( $"Before: {IndexCounter}" );
		IndexCounter = (IndexCounter + 1) % CurrentLobby.Slots.Count;
		Log.Debug( $"After: {IndexCounter}" );

		Log.Debug( $"Created LobbySlot for _slot: {_slot}" );

		Update();
	}

	protected override void OnClick( MousePanelEvent e )
	{
		if ( _slot.IsOccupied )
			return;

		if ( !_localUserHasSlot )
		{
			Commands.LobbyJoin( _slot.Index );
		}
		else if ( Local.Client.IsListenServerHost )
		{
			Commands.AddBotToLobby( _slot.Index );
		}
	}

	public void LeaveLobby()
	{
		if ( _slot.Client == Local.Client )
		{
			ConsoleSystem.Run( "lobby_leave" );
		}
		else if ( Local.Client.IsListenServerHost )
		{
			ConsoleSystem.Run( "lobby_kick", _slot.Index );
		}
	}

	public void SwapTeam()
	{
		if ( Local.Client.IsListenServerHost )
		{
			var nextTeam = _slot.Team switch
			{
				Team.RED => Team.BLUE,
				Team.BLUE => Team.GREEN,
				Team.GREEN => Team.YELLOW,
				Team.YELLOW => Team.RED,
				Team.GRAY => Team.RED,
				_ => throw new ArgumentOutOfRangeException()
			};

			ConsoleSystem.Run( "lobby_set_team", _slot.Index, nextTeam );
		}
	}

	[Event( "lobby.slot.updated" )]
	private void OnSlotUpdated( int slotIndex )
	{
		Update();
	}

	private void Update()
	{
		if ( _slot.IsOccupied )
		{
			AddClass( "disabled" );
			RemoveClass( "empty" );
			NameLabel.SetText( _slot.Client.Name );
			AvatarImage.SetAvatar( _slot.Client );

			switch ( _slot.Team )
			{
				case Team.RED:
					AddClass( "red" );
					RemoveClass( "blue" );
					RemoveClass( "green" );
					RemoveClass( "yellow" );
					break;
				case Team.BLUE:
					RemoveClass( "red" );
					AddClass( "blue" );
					RemoveClass( "green" );
					RemoveClass( "yellow" );
					break;
				case Team.GREEN:
					RemoveClass( "red" );
					RemoveClass( "blue" );
					AddClass( "green" );
					RemoveClass( "yellow" );
					break;
				case Team.YELLOW:
					RemoveClass( "red" );
					RemoveClass( "blue" );
					RemoveClass( "green" );
					AddClass( "yellow" );
					break;
			}

			//SetClass("disabled", Local.Client.IsListenServerHost && );
			SetClass( "can-leave", Local.Client.IsListenServerHost || _slot.Client == Local.Client );
			SetClass( "can-swap", Local.Client.IsListenServerHost );
		}
		else
		{
			SetClass( "disabled", _localUserHasSlot && !Local.Client.IsListenServerHost );
			AddClass( "empty" );
			NameLabel.SetText( "" );
			AvatarImage.SetTexture( null );
		}
	}
}
