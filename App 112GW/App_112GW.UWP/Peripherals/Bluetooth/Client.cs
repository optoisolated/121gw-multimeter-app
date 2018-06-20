using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace App_121GW.BLE
{
    class ClientBLE : AClientBLE, IClientBLE
	{
		private DeviceWatcher mDeviceWatcher;
		private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
		{
			if (sender != mDeviceWatcher)	return;
			if (args?.Name == string.Empty)	return;
				
			Debug.WriteLine(args?.Name);
			AddUniqueItem(new UnPairedDeviceBLE(args));
		}
		private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
		{
			if (sender != mDeviceWatcher) return;

			var removed_id = args?.Id;
			UpdateItem((item) =>
			{
				if (item?.Id == removed_id) (item as UnPairedDeviceBLE).Information.Update(args);
			});
		}

		private async Task<IDeviceBLE> ConnectionComplete( UnPairedDeviceBLE input )
		{
			var obj = await BluetoothLEDevice.FromIdAsync(input.Information.Id);
			if (obj == null) return null;

			Debug.WriteLine("Connection complete");
			return new PairedDeviceBLE(obj);
		}
		public async Task<IDeviceBLE> Connect(IDeviceBLE pInput)
		{
            if (pInput == null)
                return null;

            //Stops if running....
            await Stop();
			if (pInput.GetType() == typeof(UnPairedDeviceBLE))
            {
			    Debug.WriteLine("Connecting to : " + pInput.Id);
				return await ConnectionComplete(pInput as UnPairedDeviceBLE);
            }
            return null;
		}

		public async Task Start()
		{
			await Task.Run(() =>
			{
				bool stopped = false;
                stopped |= mDeviceWatcher.Status == DeviceWatcherStatus.Created;
                stopped |= mDeviceWatcher.Status == DeviceWatcherStatus.Stopped;
				stopped |= mDeviceWatcher.Status == DeviceWatcherStatus.Aborted;
				if (stopped) mDeviceWatcher.Start();
			});
		}
		public async Task Stop()
		{
			await Task.Run(() =>
			{
				bool running = false;
				running |= mDeviceWatcher.Status == DeviceWatcherStatus.Started;
				running |= mDeviceWatcher.Status == DeviceWatcherStatus.EnumerationCompleted;
				if (running) mDeviceWatcher.Stop();
			});
		}
		public async Task Reset()
		{
            await Stop();
            await Start();
        }
        public async Task Rescan() => await Reset();

		public ClientBLE(IBluetoothDeviceFilter pFilter) : base(pFilter)
		{
			//Get all devices paired and not
			var query = "(" + BluetoothLEDevice.GetDeviceSelectorFromPairingState( true  ) + ") OR (" + BluetoothLEDevice.GetDeviceSelectorFromPairingState( false ) + ")";

			//Create device watcher
			mDeviceWatcher = DeviceInformation.CreateWatcher(query, new string[]{ "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" }, DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            mDeviceWatcher.Added	+=  DeviceWatcher_Added;
			mDeviceWatcher.Updated  +=  DeviceWatcher_Updated;

			// Start the watcher, it should actually start...
			mDeviceWatcher.Start();
		}
		~ClientBLE()
		{
			if ( mDeviceWatcher != null )
			{
				// Unregister the event handlers.
				mDeviceWatcher.Added	-=  DeviceWatcher_Added;
				mDeviceWatcher.Updated  -=  DeviceWatcher_Updated;

				// Stop the watcher, it should actually stop...
				Stop().Wait();
			}
		}
    }
}