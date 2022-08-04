using Sandbox.UI;

namespace WiiTanks;

[UseTemplate]
public class ToggleBox : Panel
{
	public bool Checked
	{
		get => HasClass( "checked" );
		set
		{
			if (value == HasClass("checked"))
				return;

			SetClass( "checked", value );
			CreateEvent( "onchange" );
		}
	}

	protected override void OnClick( MousePanelEvent e )
	{
		Checked = !Checked;
	}
}
