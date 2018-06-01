using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using Plugin.BLE.Abstractions.EventArgs;
using System.Collections.ObjectModel;

namespace App_121GW.BLE
{
    public class ClientBLE : AClientBLE, IClientBLE
    {
        private volatile IBluetoothLE mDevice;
        private volatile IAdapter mAdapter;

        private void DeviceWatcher_Added(object sender, DeviceEventArgs args)
        {
            if (args.Device.Name == string.Empty)
                return;

            Debug.WriteLine("Device discovered.");

            var devices = mAdapter.DiscoveredDevices;
            foreach (var item in devices)
            {
                Debug.WriteLine(item.Name + " " + item.Id);
                AddUniqueItem(new UnPairedDeviceBLE(item));
            }
        }

        public async Task Start()
        {
            await mAdapter.StartScanningForDevicesAsync();
        }
        public async Task Stop()
        {
            await mAdapter.StopScanningForDevicesAsync();
        }
        public async Task Rescan()
        {
            await mAdapter.StartScanningForDevicesAsync();
        }
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
                return new PairedDeviceBLE((pInput as UnPairedDeviceBLE).mDevice, TriggerDeviceConnected);
            }
            return null;
        }

        public ClientBLE()
        {
            //Setup bluetoth basic adapter
            mDevice = CrossBluetoothLE.Current;
            mAdapter = CrossBluetoothLE.Current.Adapter;

            mAdapter.ScanTimeoutElapsed += async (sender, args) => await Rescan();
            mAdapter.ScanTimeout = int.MaxValue;

            //Add debug state change indications
            mDevice.StateChanged += async (s, e) => 
            {
                Debug.WriteLine("The bluetooth state changed to " + e.NewState.ToString());
                if (e.NewState == BluetoothState.TurningOn || e.NewState == BluetoothState.On)
                {
                    await Reset();
                    if (mDevice.IsOn && mDevice.IsAvailable)
                        mAdapter.DeviceDiscovered += DeviceWatcher_Added;
                }
            };

            //
            mAdapter.DeviceConnectionLost += DeviceConnection_Lost;
        }




        private async void DeviceConnection_Lost(object sender, DeviceErrorEventArgs e)
        {
            string disconnect_Id = e.Device.Id.ToString();
            Debug.WriteLine("DeviceConnection_Lost : " + disconnect_Id);
            foreach (var item in mConnectedDevices)
                if (item.Id == disconnect_Id)
                {
                    Debug.WriteLine(item.Id);
                    await mAdapter.DisconnectDeviceAsync(e.Device);

                    while (e.Device.State != Plugin.BLE.Abstractions.DeviceState.Connected)
                    {
                        await mAdapter.ConnectToDeviceAsync(e.Device);
                        Debug.WriteLine("Reconnected : " + item.Id);
                        if (e.Device.State == Plugin.BLE.Abstractions.DeviceState.Connected)
                            item.Remake(e.Device);
                    }
                }
        }

        ~ClientBLE()
        {
            Debug.WriteLine("Deconstructing ClientBLE!");
            try
            {
                Stop().Wait();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}