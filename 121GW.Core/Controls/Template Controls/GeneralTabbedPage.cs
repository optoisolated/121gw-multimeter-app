using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace App_121GW
{
	public class GeneralTabbedPage : Xamarin.Forms.TabbedPage
	{
		public GeneralTabbedPage()
		{
			Padding			= Globals.Padding;
			BarTextColor	= Globals.TextColor;
			BackgroundColor = Globals.BackgroundColor;

			On<Windows>().SetHeaderIconsEnabled(true);
			On<Windows>().SetHeaderIconsSize(new Size(32, 32));
			On<Windows>().EnableHeaderIcons();

			base.PagesChanged += GeneralTabbedPage_PagesChanged;
		}

		private void GeneralTabbedPage_PagesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			On<Windows>().DisableHeaderIcons();
			On<Windows>().EnableHeaderIcons();
		}
	}
}
