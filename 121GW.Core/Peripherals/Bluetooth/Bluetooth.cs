using System;
using Xamarin.Forms;

namespace App_121GW.BLE
{
    public delegate void SetupComplete();
    public delegate void VoidEvent();

    public delegate void ConnectedEvent(IDeviceBLE pDevice);
    public delegate void DeviceConnected(IDeviceBLE device);
    public delegate void DeviceSetupComplete(IDeviceBLE Device);
    public delegate void ChangeEvent(Object o, CharacteristicEvent v);

    public interface IBluetooth
    {
        IClientBLE Create(IBluetoothDeviceFilter pFilter);
    }

    class Bluetooth
    {
        public static IClientBLE Client(IBluetoothDeviceFilter pFilter) => DependencyService.Get<IBluetooth>().Create(pFilter);
    }
}