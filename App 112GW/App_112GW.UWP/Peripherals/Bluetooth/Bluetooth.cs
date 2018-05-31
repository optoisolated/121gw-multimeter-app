using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(App_121GW.BLE.UWP.Bluetooth))]
namespace App_121GW.BLE.UWP
{
    class Bluetooth : IBluetooth
    {
        public IClientBLE Create() => new ClientBLE();
    }
}
