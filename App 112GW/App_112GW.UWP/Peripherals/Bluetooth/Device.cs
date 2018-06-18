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
		public ChangeEvent ValueChanged { get; set; }

		public string Id	=> Information.Id;
		public string Name	=> Information.Name;
		public bool Paired	=> Information.Pairing.IsPaired;
		public bool CanPair => Information.Pairing.CanPair;

		public override string ToString()	=> Name + "\n" + Id;

        public void Remake(object o)		=> throw new NotImplementedException();
		public void Unregister()			=> throw new NotImplementedException();
		public void Dispose()				=> throw new NotImplementedException();

		public List<IServiceBLE> Services	=> null;

		public UnPairedDeviceBLE(DeviceInformation pInput) { Information = pInput; }
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class PairedDeviceBLE : IDeviceBLE
	{
		async void MDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
		{
			Debug.WriteLine("Reconnecting");
			if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
				Remake(await BluetoothLEDevice.FromIdAsync(sender.DeviceId));
		}

		private BluetoothLEDevice mDevice;
		public ChangeEvent ValueChanged { get; set; } = null;
		public List<IServiceBLE> Services { get; private set; } = new List<IServiceBLE>();

		void AddServices(IReadOnlyList<GattDeviceService> pServices)
		{
			Debug.WriteLine("Services Aquired");
			foreach (var item in pServices)
				Services.Add(new ServiceBLE(item));

			foreach (var service in Services)
				service.ValueChanged += ValueChanged;

			mDevice.ConnectionStatusChanged += MDevice_ConnectionStatusChanged;
		}
		void Build() => Task.Factory.StartNew( async() =>
		{
			if (mDevice == null) return;
			var services = (await mDevice?.GetGattServicesAsync())?.Services;
			if (services == null) return;

			AddServices(services);
		});

		public void Remake( object o )
		{
			try
			{
				//Unregisters old system
				Unregister();

				mDevice = null;
				mDevice = o as BluetoothLEDevice;

				Debug.WriteLine("Remaking");

				foreach (var se in Services) se.Remake();
				Services = null;
				Services = new List<IServiceBLE>();

				//Builds new system
				Build();
			}
			catch { }
		}
		public void Unregister()
		{
			if ( mDevice != null )
				mDevice.ConnectionStatusChanged -= MDevice_ConnectionStatusChanged;
		}

		public PairedDeviceBLE( BluetoothLEDevice pInput )
		{
			mDevice = pInput;
			Build();
		}
		~PairedDeviceBLE()
		{
			Debug.WriteLine("Deregistering Service.");
			mDevice.Dispose();
			mDevice = null;
			Unregister();
			Services = null;
		}

		public string Id => mDevice.DeviceId;
		public string Name => mDevice.Name;
		public bool Paired => mDevice.DeviceInformation.Pairing.IsPaired;
		public bool CanPair => mDevice.DeviceInformation.Pairing.CanPair;

		public override string ToString() => Name + "\n" + Id;
	}
}
