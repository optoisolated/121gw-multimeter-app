using Xamarin.Forms;

namespace App_121GW
{
	public class GeneralLabel : Label
	{
		public GeneralLabel()
		{
			HorizontalOptions = LayoutOptions.CenterAndExpand;
			VerticalOptions = LayoutOptions.CenterAndExpand;
			TextColor = Globals.TextColor;
			FontSize = Globals.MajorFontSize;
		}
	}
}
