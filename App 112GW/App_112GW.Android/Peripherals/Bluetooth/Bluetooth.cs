using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(App_121GW.BLE.Android.Bluetooth))]
namespace App_121GW.BLE.Android
{
    class Bluetooth : IBluetooth
    {
        public IClientBLE Create() => new ClientBLE();
    }
}