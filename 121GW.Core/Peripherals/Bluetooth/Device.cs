using System.Collections.Generic;

namespace App_121GW.BLE
{
    public interface IDeviceBLE
    {
        event DeviceSetupComplete Ready;
        event ChangeEvent Change;

        string Id { get; }
        string Name { get; }
        bool Paired { get; }
        bool CanPair { get; }
        void Remake(object o);

        void Unregister();

        string ToString();
        List<IServiceBLE> Services { get; }
    }
}