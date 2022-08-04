using System.Linq;
using Sandbox.Html;
using Sandbox.UI;

namespace WiiTanks;

[UseTemplate]
public class MenuOption : Panel
{
	public string OptionText
	{
		get
		{
			return InnerLabel.Text;
		}
		set
		{
			InnerLabel.SetText(value);
		}
	}

	private Label InnerLabel => Children.OfType<Label>().First();

	public override bool OnTemplateElement( INode element )
	{
		if ( HasChildren )
		{
			OptionText = element.InnerHtml;
		}
		
		return base.OnTemplateElement( element );
	}
}
