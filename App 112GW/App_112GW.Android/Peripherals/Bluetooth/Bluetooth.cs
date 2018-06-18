using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(App_121GW.BLE.Droid.Bluetooth))]
namespace App_121GW.BLE.Droid
{
    class Bluetooth : IBluetooth
    {
        public IClientBLE Create(IBluetoothDeviceFilter pFilter) => new ClientBLE(pFilter);
    }
}