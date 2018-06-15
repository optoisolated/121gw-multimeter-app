namespace App_121GW.BLE
{
	public interface IBluetoothDeviceFilter
	{
		bool NameAccepted(IDeviceBLE pDevice);
		bool IdAccepted(IDeviceBLE pDevice);
	}
}
