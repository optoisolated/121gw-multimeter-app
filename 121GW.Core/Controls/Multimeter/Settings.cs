using App_121GW;
using App_121GW.BLE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace App_121GW
{
	public class Bluetooth121GWFilter : IBluetoothDeviceFilter
	{
		public bool IdAccepted(IDeviceBLE pInput) => true;
		public bool NameAccepted(IDeviceBLE pInput)
		{
			var name = pInput.Name;
			return name.Contains("121GW") || name.Contains("Bluegiga");
		}
	}

	public class Settings : AutoGrid
	{
		public delegate void AddBluetoothDevice(IDeviceBLE pDevice);

		public event AddBluetoothDevice AddDevice;
		private BluetoothDeviceSelector BluetoothSelectDevice = new BluetoothDeviceSelector(new Bluetooth121GWFilter());

		public Settings() : base()
		{
			//Setup connected event
			DefineGrid(1, 4);

			//Setup default display
			AutoAdd(new GeneralLabel { Text = "Select the 121GW to connect to:" });
			FormatCurrentRow(GridUnitType.Auto);

			AutoAdd(BluetoothSelectDevice);
			FormatCurrentRow(GridUnitType.Star);

			AutoAdd(new GeneralLabel { Text = "Build : " + Version.Build() });
			FormatCurrentRow(GridUnitType.Auto);

			AutoAdd(new GeneralButton("Refresh", async (o, e) => await BluetoothSelectDevice.Reset()));
			FormatCurrentRow(GridUnitType.Auto);

			BluetoothSelectDevice.Connected += Connected;
		}

		private void Connected(IDeviceBLE pDevice)
		{
			if (pDevice == null) return;
			AddDevice?.Invoke(pDevice);
		}
	}
}