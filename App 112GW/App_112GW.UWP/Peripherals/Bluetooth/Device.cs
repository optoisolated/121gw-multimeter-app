using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace rMultiplatform.BLE
{

    #region unpaired
    public class UnPairedDeviceBLE : IDeviceBLE
	{
		public volatile DeviceInformation Information;
		public event DeviceSetupComplete Ready;
		public event ChangeEvent Change;

		public string Id => Information.Id;
		public string Name => Information.Name;
		public bool Paired => Information.Pairing.IsPaired;
		public bool CanPair => Information.Pairing.CanPair;

		public UnPairedDeviceBLE(DeviceInformation pInput) { Information = pInput; }
		public override string ToString() => Name + "\n" + Id;

        public void Remake(object o)
		{
			throw new NotImplementedException();
		}
		public void Unregister()
		{
			throw new NotImplementedException();
		}

		public List<IServiceBLE> Services => null;
	}
    #endregion


    public class PairedDeviceBLE : IDeviceBLE
	{
		private BluetoothLEDevice mDevice;
		private List<IServiceBLE> mServices;
		public event DeviceSetupComplete Ready;
		public event ChangeEvent Change;
		void TriggerReady()
		{
			mDevice.ConnectionStatusChanged += MDevice_ConnectionStatusChanged;
			Ready?.Invoke(this);
			Ready = null;
		}

		private void                InvokeChange(object o, CharacteristicEvent v) => Change?.Invoke(o, v);
        public  void                Unregister()    => Deregister();
        public  string              Id              => mDevice.DeviceId;
		public  string              Name            => mDevice.Name;
        public override string      ToString()      => Name + "\n" + Id;
		public  bool                Paired          => mDevice.DeviceInformation.Pairing.IsPaired;
		public  bool                CanPair         => mDevice.DeviceInformation.Pairing.CanPair;
		public  List<IServiceBLE>   Services        => mServices;

        private int Uninitialised = 0;
		private void ItemReady()
		{
			--Uninitialised;
			Debug.WriteLine("Service count remaining = " + Uninitialised.ToString());
			if (Uninitialised == 0) TriggerReady();
		}
		private async void Build()
		{
            var obj = await mDevice.GetGattServicesAsync();
            ServicesAquired(obj);
		}
		private void ServicesAquired(GattDeviceServicesResult result)
		{
			Debug.WriteLine("Services Aquired.");
			var services = result.Services;
			Uninitialised = services.Count;
			foreach (var service in services)
				mServices.Add(new ServiceBLE(service, ItemReady, InvokeChange));
		}
		public void Remake(object o)
		{
			Deregister();
			Build();
		}

		void Deregister()
		{
			if (mDevice != null)
			{
				mDevice.ConnectionStatusChanged -= MDevice_ConnectionStatusChanged;

				if (mServices != null)
					foreach (var service in mServices)
						service.Unregister();

				mServices = null;
				mServices = new List<IServiceBLE>();
			}
		}

		async void MDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
		{
			mDevice = sender;
			Deregister();
			if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
			{
				Debug.WriteLine("Reconnecting...");
                var rslt = await BluetoothLEDevice.FromIdAsync(sender.DeviceId);
                ConnectionComplete(rslt);
			}
			else
				ConnectionComplete(sender);
			Debug.WriteLine("MDevice_ConnectionStatusChanged end.");
		}
		void ConnectionComplete(BluetoothLEDevice result)
		{
			Debug.WriteLine("Connection complete start.");
			mDevice = result;
			Remake(result);
			Debug.WriteLine("Connection complete end.");
		}


		public PairedDeviceBLE(BluetoothLEDevice pInput, DeviceSetupComplete pReady)
		{
			mServices = new List<IServiceBLE>();
			Ready = pReady;
			mDevice = pInput;
			Build();
		}
		~PairedDeviceBLE()
		{
			Debug.WriteLine("Deregistering Service.");
			mDevice.Dispose();
			mDevice = null;
			Deregister();
			mServices = null;
		}
	}
}
