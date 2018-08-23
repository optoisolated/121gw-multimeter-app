using App_121GW.BLE;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using System.Diagnostics;

using System.Collections;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.Xaml;

namespace App_121GW
{
	public partial class MainPage : GeneralTabbedPage
	{
		private Settings SettingsView = new Settings();
        private MathChart MathChart = new MathChart();

        private void AddPage(string Title, View Content, string Icon = null) => Children.Add(new GeneralPage(Title, Content, Icon));

		private void AddDevice(MultimeterPage Device)
		{
			On<Windows>().SetHeaderIconsEnabled(true);
			On<Windows>().EnableHeaderIcons();
			On<Windows>().SetHeaderIconsSize(new Size(32, 32));
			MathChart.AddDevice(Device);
			
			if (Children.Count >= 2)
				if (MathChart.Devices.Count == 2)
					Children.Insert(1, new GeneralPage("Maths", MathChart));
        }
		private void Button_AddDevice(IDeviceBLE pDevice)
		{
			Globals.RunMainThread(() =>
			{
				var dev = new MultimeterPage(pDevice);

				AddDevice(dev);
				Children.Add(dev);
				CurrentPage = Children[Children.Count - 1];
			});
		}


		protected override void InvalidateMeasure()
		{
			base.InvalidateMeasure();
			On<iOS>().SetUseSafeArea(true);
			var safe = On<iOS>().SafeAreaInsets();
			Padding = new Thickness(0, safe.Top, 0, 0);
			Debug.WriteLine(safe.ToString());
		}

		public MainPage()
		{
			On<Android>().SetIsSwipePagingEnabled(false);

            BackgroundColor = Globals.BackgroundColor;
			SettingsView.AddDevice += Button_AddDevice;

            AddPage("Settings", SettingsView);
		}
    }
}