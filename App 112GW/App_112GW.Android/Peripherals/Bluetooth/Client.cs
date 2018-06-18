using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Android.Bluetooth;

using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace App_121GW.BLE
{
	public class ClientBLE : AClientBLE, IClientBLE
	{
        private volatile IBluetoothLE mDevice;
        private volatile IAdapter mAdapter;

        private void DeviceWatcher_Added(object sender, DeviceEventArgs args)
        {
            if (args.Device.Name == string.Empty) return;
			
            var devices = mAdapter.DiscoveredDevices;
            foreach (var item in devices)
            {
                Debug.WriteLine(item.Name + " " + item.Id);
                AddUniqueItem(new UnPairedDeviceBLE(item));
            }
        }

        public async Task Start() => await mAdapter.StartScanningForDevicesAsync();
        public async Task Stop() => await mAdapter.StopScanningForDevicesAsync();
        public async Task Rescan() => await mAdapter.StartScanningForDevicesAsync();
        public async Task Reset()
        {
            await Stop();
            await Start();
        }

        public async Task<IDeviceBLE> Connect(IDeviceBLE pInput)
        {
            if (pInput == null) return null;

            var inputType = pInput.GetType();
            var searchType = typeof(UnPairedDeviceBLE);

            if (inputType == searchType)
            {
                //Pair if the device is able to pair
                Debug.WriteLine("Connecting to new device.");
                await mAdapter.ConnectToDeviceAsync((pInput as UnPairedDeviceBLE).mDevice);

                Debug.WriteLine("Stopping scanning.");
                await mAdapter.StopScanningForDevicesAsync();

                Debug.WriteLine("Connection Complete.");
                return new PairedDeviceBLE((pInput as UnPairedDeviceBLE).mDevice);
            }
            return null;
        }

		public ClientBLE(IBluetoothDeviceFilter pFilter) : base(pFilter)
		{
			//Setup bluetoth basic adapter
			mDevice	 = CrossBluetoothLE.Current;
			mAdapter = CrossBluetoothLE.Current.Adapter;
			mAdapter.ScanTimeoutElapsed += async (s, e ) => await Rescan();
			mAdapter.ScanTimeout = int.MaxValue;

			//Add debug state change indications
			mDevice.StateChanged += async (s, e) =>
			{
				Debug.WriteLine($"The bluetooth state changed to " + e.NewState.ToString());
				if (e.NewState == BluetoothState.TurningOn || e.NewState == BluetoothState.On)
					await Reset();
			};

			//
			if (mDevice.IsOn && mDevice.IsAvailable)
				mAdapter.DeviceDiscovered += DeviceWatcher_Added;
			mAdapter.DeviceConnectionLost += DeviceConnection_Lost;

			Task.Factory.StartNew(Start);
		}

		private async void DeviceConnection_Lost (object sender, DeviceErrorEventArgs e)
		{
			string disconnect_Id = e.Device.Id.ToString();
			Debug.WriteLine( "DeviceConnection_Lost." );
			Debug.WriteLine( disconnect_Id );
			foreach (var item in ConnectedDevices)
				if (item.Id == disconnect_Id)
				{
					Debug.WriteLine(item.Id);
					await mAdapter.DisconnectDeviceAsync(e.Device).ContinueWith((temp) =>
					{
						while (e.Device.State != Plugin.BLE.Abstractions.DeviceState.Connected)
						{
							mAdapter.ConnectToDeviceAsync(e.Device).ContinueWith((obj) =>
							{
								Debug.WriteLine("Reconnected, maybe.");
								if (e.Device.State == Plugin.BLE.Abstractions.DeviceState.Connected)
									item.Remake(e.Device);
							}).Wait();
						}
					});
				}
		}

        ~ClientBLE() => Stop().Wait();
	}
}