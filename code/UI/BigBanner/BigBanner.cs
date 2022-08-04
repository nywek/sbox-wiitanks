using Sandbox.Html;
using Sandbox.UI;

namespace WiiTanks;

[UseTemplate]
public class BigBanner : Panel
{
	public enum Color { Blue, Red }

	public Label BannerText { get; set; }

	public Color BannerColor
	{
		get
		{
			return HasClass( "blue" ) ? Color.Blue : Color.Red;
		}
		set
		{
			SetClass( "blue", value == Color.Blue );
			SetClass( "red", value == Color.Red );
		}
	}

	public override bool OnTemplateElement( INode element )
	{
		if ( HasChildren )
		{
			BannerText.SetText( element.InnerHtml );
		}

		return base.OnTemplateElement( element );
	}
}
