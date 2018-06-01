using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App_121GW.BLE
{
	public class BluetoothDeviceSelector : GeneralView
	{
		public event DeviceConnected Connected;

		public async Task	Reset()		=>	await mClient?.Reset();
        IClientBLE			mClient		=	Bluetooth.Client();
		GeneralListView		mDevices	=	new GeneralListView { ItemTemplate = DefaultTemplate() };
		Loading				mActivity	=	new Loading("connecting");

		bool IsBusy
		{
			set
			{
				mActivity.IsRunning = value;

				Content = null;
				if ( value )	Content = mActivity;
				else			Content = mDevices;
			}
		}

		static DataTemplate DefaultTemplate()
        {
            var template = new DataTemplate(typeof(TextCell));

            template.	SetBinding	(TextCell.TextProperty,			"Name"					);
            template.	SetBinding	(TextCell.DetailProperty,		"Id"					);
            template.	SetValue	(TextCell.TextColorProperty,	Globals.TextColor		);
            template.	SetValue	(TextCell.DetailColorProperty,	Globals.HighlightColor	);

            return template;
        }

		public BluetoothDeviceSelector()
		{
			mDevices.ItemSelected += async (o, e) =>
			{
				IsBusy = true;
				try		{ Connected?.Invoke(await mClient?.Connect((e.SelectedItem as IDeviceBLE))); }
				catch	{ }
				IsBusy = false;
			};
			mDevices.ItemsSource = mClient.VisibleDevices;
			IsBusy = false;
		}
        
    }
}