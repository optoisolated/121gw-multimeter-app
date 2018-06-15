using System.Threading.Tasks;
using Xamarin.Forms;

namespace App_121GW.BLE
{
	public class BluetoothDeviceSelector : GeneralView
	{
		public event DeviceConnected Connected;
        private IClientBLE		mClient; 
		private Loading			mActivity	= new Loading("connecting");
		private GeneralListView	mDevices	= new GeneralListView { ItemTemplate = DefaultTemplate() };

		private bool IsBusy
		{
			set
			{
				Content = null;
				if (value)
				{
					mActivity.Run();
					Content = mActivity;
				}
				else
				{
					mActivity.Stop();
					Content = mDevices;
				}
			}
		}

		static DataTemplate DefaultTemplate()
        {
            var template = new DataTemplate(typeof(TextCell));

            template.	SetBinding	(TextCell.TextProperty,			"Name"					);
            template.	SetBinding	(TextCell.DetailProperty,		"Id"					);
            template.	SetValue	(TextCell.TextColorProperty,	Globals.TextColor		);
            template.	SetValue	(TextCell.DetailColorProperty,	Globals.TextColor		);

            return template;
        }

		public async Task Reset() => await mClient?.Reset();

		public BluetoothDeviceSelector(IBluetoothDeviceFilter pFilter)
		{
			mClient = Bluetooth.Client(pFilter);
			IsBusy = false;
			mDevices.ItemSelected += async (o, e) =>
			{
				try
				{
					IsBusy = true;
					Connected?.Invoke(await mClient?.Connect((e.SelectedItem as IDeviceBLE)));
				}
				finally
				{
					IsBusy = false;
				}
			};
			mDevices.ItemsSource = mClient.VisibleDevices;
		}
        
    }
}