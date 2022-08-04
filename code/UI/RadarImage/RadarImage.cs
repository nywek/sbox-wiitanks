using Sandbox.UI;

namespace WiiTanks;

public class RadarImage : Image
{
	private Arena _arena;

	public Arena Arena
	{
		get
		{
			return _arena;
		}
		set
		{
			_arena = value;
		}
	}

	public RadarImage()
	{
		SetTexture( "/ui/arena_radar.png" );
		Style.Width = Length.Pixels( 177 );
		Style.Height = Length.Pixels( 102 );
	}
}
