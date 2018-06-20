using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace App_121GW.BLE
{
    public class UnPairedDeviceBLE : IDeviceBLE
	{
		public volatile DeviceInformation Information;
		public event DeviceSetupComplete Ready;
		public ChangeEvent ValueChanged { get; set; }

		public string Id => Information.Id;
		public string Name => Information.Name;
		public bool Paired => Information.Pairing.IsPaired;
		public bool CanPair => Information.Pairing.CanPair;

		public UnPairedDeviceBLE(DeviceInformation pInput) { Information = pInput; }
		public override string ToString() => Name + "\n" + Id;

        public void Remake(object o) => throw new NotImplementedException();
		public void Unregister() => throw new NotImplementedException();
		public void Dispose() => throw new NotImplementedException();

		public List<IServiceBLE> Services => null;
	}

    public class PairedDeviceBLE : IDeviceBLE
	{
		private BluetoothLEDevice	mDevice;
		public ChangeEvent			ValueChanged	{ get; set; }
		public List<IServiceBLE>	Services		{ get; private set; }

		private void ServicesAquired(GattDeviceServicesResult pResult)
		{
			var services = pResult.Services;
			foreach (var service in services)
				Services.Add(new ServiceBLE(service));
			foreach (var service in Services)
				service.ValueChanged += ValueChanged;

			mDevice.ConnectionStatusChanged += MDevice_ConnectionStatusChanged;
		}
		private void Build() => Task.Factory.StartNew(async () => ServicesAquired(await mDevice.GetGattServicesAsync()));

		public void Unregister()
		{
			if (mDevice != null)
			{
				mDevice.ConnectionStatusChanged -= MDevice_ConnectionStatusChanged;

				if (Services != null)
					foreach (var service in Services)
						service.Remake();

				Services = null;
				Services = new List<IServiceBLE>();
			}
		}
		void ConnectionComplete(BluetoothLEDevice pResult) => Remake(mDevice = pResult);

		async void MDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
		{
			if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
			{
				Unregister();
                ConnectionComplete(await BluetoothLEDevice.FromIdAsync(sender.DeviceId));
			}
			else ConnectionComplete(sender);
		}
		public void Remake(object o)
		{
			Unregister();
			Build();
		}
		public PairedDeviceBLE(BluetoothLEDevice pInput)
		{
			Services = new List<IServiceBLE>();
			mDevice = pInput;
			Build();
		}
		~PairedDeviceBLE()
		{
			mDevice = null;
			Unregister();
			Services = null;
		}

		public string Id => mDevice.DeviceId;
		public string Name => mDevice.Name;
		public override string ToString() => Name + "\n" + Id;
		public bool Paired => mDevice.DeviceInformation.Pairing.IsPaired;
		public bool CanPair => mDevice.DeviceInformation.Pairing.CanPair;
	}
}
