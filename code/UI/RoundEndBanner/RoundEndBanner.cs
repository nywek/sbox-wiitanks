using System.Linq;
using Sandbox;
using Sandbox.UI;
using WiiTanks.Extensions;

namespace WiiTanks.UI;

[UseTemplate]
public class RoundEndBanner : Panel
{
	private Image AvatarPanel { get; set; }
	private BigBanner Banner { get; set; }

	public RoundEndBanner()
	{
		AvatarPanel.SetAvatar( 76561198024674549 );
		AddClass( "hidden" );
	}

	public void Display( Team? winningTeam, Client? client )
	{
		Log.Debug($"Display({winningTeam}, {client})");
		
		if ( winningTeam is not null )
		{
			var winners = CurrentRound.Participants.Count( p => p.Team == winningTeam );

			if ( winners == 1 )
			{
				var winner = CurrentRound.Participants.First( p => p.Team == winningTeam );
				AvatarPanel.SetAvatar( winner.Client );
				AvatarPanel.Style.Display = DisplayMode.Flex;

				Banner.BannerText.SetText( winner.Client.Name + " Won!" );
			}
			else
			{
				AvatarPanel.Style.Display = DisplayMode.None;
				Banner.BannerText.SetText( "Team " + winningTeam?.GetFriendlyName() + " Won!" );
			}
		}
		else
		{
			AvatarPanel.Style.Display = DisplayMode.None;
			Banner.BannerText.SetText( "Draw!" );
		}

		RemoveClass( "hidden" );
	}

	[Event( "round.won" )]
	public void OnRoundWon( Team? winningTeam )
	{
		if ( winningTeam is null )
		{
			Display( null, null );
		}
		else
		{
			if ( winningTeam?.GetClients().Count == 1 )
			{
				Display( winningTeam, winningTeam?.GetClients().FirstOrDefault() );
			}
			else
			{
				Display( winningTeam, null );
			}
		}
	}

	[Event( "round.statechange" )]
	public void OnRoundStateChanged( RoundState newState )
	{
		if ( newState == RoundState.Ended )
			AddClass( "hidden" );
	}
}
