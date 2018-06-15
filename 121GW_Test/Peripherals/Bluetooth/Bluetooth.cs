using Xamarin.Forms;

[assembly: Dependency(typeof(App_121GW.BLE.IOS.Bluetooth))]
namespace App_121GW.BLE.IOS
{
    class Bluetooth : IBluetooth
	{
		public IClientBLE Create(IBluetoothDeviceFilter pFilter) => new ClientBLE(pFilter);
	}
}