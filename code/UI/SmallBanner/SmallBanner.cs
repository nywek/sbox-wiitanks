using Sandbox.Html;
using Sandbox.UI;

namespace WiiTanks;

[UseTemplate]
public class SmallBanner : Panel
{
	public Label BannerText { get; set; }

	public override bool OnTemplateElement( INode element )
	{
		if ( HasChildren )
		{
			BannerText.SetText( element.InnerHtml );
		}

		return base.OnTemplateElement( element );
	}
}
