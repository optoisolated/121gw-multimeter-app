using System.Diagnostics;
using Xamarin.Forms;

namespace App_121GW.BLE
{
	public class BLEDeviceSelector : GeneralView
	{
		private GeneralListView mDevices;
		private Loading		    Activity = new Loading("connecting");

		public delegate void DeviceConnected(IDeviceBLE pDevice);
		public event DeviceConnected Connected;

		public void Reset()
		{
			try
			{
				mClient.Reset();
			}
			catch
			{
				Debug.WriteLine("Error Caught : public void Reset()");
			}
		}

		private bool IsBusy
		{
			set
			{
				if (value)
				{
					Content = null;
					Activity.IsRunning = true;
					Content = Activity;
				}
				else
				{
					Content = null;
					Activity.IsRunning = false;
					Content = mDevices;
				}
			}
		}

		public IClientBLE mClient;
		public BLEDeviceSelector()
		{
			//Reset BLE
			mClient = null;
            mClient = Bluetooth.Client();
			mClient.DeviceConnected += MClient_DeviceConnected;

			// We can set data bindings to our supplied objects.
			var template = new DataTemplate(typeof(TextCell));
			template.SetBinding (TextCell.TextProperty,		    "Name");
			template.SetBinding (TextCell.DetailProperty,	    "Id");
			template.SetValue   (TextCell.TextColorProperty,	Globals.TextColor);
			template.SetValue   (TextCell.DetailColorProperty,  Globals.HighlightColor);

			//
			mDevices = new GeneralListView();
			mDevices.ItemTemplate	   = template;
			mDevices.ItemSelected	   += OnSelection;
			mDevices.ItemsSource		= mClient.ListDevices();

			//
			IsBusy = false;
		}

		public void RemoveDevices()
		{
			mClient.RemoveAll();
		}

		private void MClient_DeviceConnected(IDeviceBLE pDevice)
		{
			IsBusy = false;
			Connected?.Invoke(pDevice);
		}
		private void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			var item = (e.SelectedItem as IDeviceBLE);
			Connect(item);
		}

		private void Connect (IDeviceBLE Device)
		{
			if (Device == null)
				return;

			//Wait for device to appear
			if (mClient != null)
			{
				IsBusy = true;
				mClient.Connect(Device);
			}
		}
	}
}