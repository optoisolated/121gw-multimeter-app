using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace App_121GW
{
	public class GeneralPage : ContentPage
	{
		public GeneralPage(string pTitle, View pContent)
		{
			Title = pTitle;
			Content = pContent;
			Padding = Globals.Padding;
			BackgroundColor = Globals.BackgroundColor;
			On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
		}
	}
}
