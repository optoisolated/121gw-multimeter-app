﻿using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text;
using Android.Bluetooth;
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

		private static int index = 0;
		private void DeviceWatcher_Added(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs args)
		{
			int indexer = index++;
			if (args.Device.Name == string.Empty || mVisibleDevices == null)
				return;


            MutexBlock(() =>
			{
				mVisibleDevices.Clear();
				var devices = mAdapter.DiscoveredDevices;
				foreach (var item in devices)
				{
					Debug.WriteLine(item.Name + " " + item.Id);
					AddUniqueItem(new UnPairedDeviceBLE(item));
				}
			}, (indexer.ToString() + " Adding"));
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

		IDeviceBLE ConnectingDevice = null;
		PairedDeviceBLE Device = null;
		private void ConnectionComplete(Task obj)
		{
			Debug.WriteLine("Connection Complete.");
			Device = new PairedDeviceBLE((ConnectingDevice as UnPairedDeviceBLE).mDevice, 
			(dev) => {
				TriggerDeviceConnected(dev);
			});
		}
		private void StopScanning(Task obj)
		{
			Debug.WriteLine("Stopping scanning.");
			mAdapter.StopScanningForDevicesAsync().ContinueWith(ConnectionComplete);
		}
		public void Connect(IDeviceBLE pInput)
		{
			if (pInput == null)
				return;

			var inputType = pInput.GetType();
			var searchType = typeof(UnPairedDeviceBLE);

			Device = null;
			if (inputType == searchType)
			{
				//Pair if the device is able to pair
				ConnectingDevice = pInput;
				Debug.WriteLine("Connecting to new device.");
				mAdapter.ConnectToDeviceAsync((ConnectingDevice as UnPairedDeviceBLE).mDevice).ContinueWith(StopScanning);
			}
		}

		public ClientBLE()
		{
			mConnectedDevices = new ObservableCollection<IDeviceBLE>();

			//Setup bluetoth basic adapter
			mDevice	 = CrossBluetoothLE.Current;
			mAdapter	= CrossBluetoothLE.Current.Adapter;
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
		}

		private async void DeviceConnection_Lost (object sender, DeviceErrorEventArgs e)
		{
			string disconnect_Id = e.Device.Id.ToString();
			Debug.WriteLine( "DeviceConnection_Lost." );
			Debug.WriteLine( disconnect_Id );
			foreach (var item in mConnectedDevices)
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

        Task IClientBLE.Connect(IDeviceBLE pInput)
        {
            throw new NotImplementedException();
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