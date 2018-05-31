using System.Collections.Generic;

namespace App_121GW.BLE
{
    public interface IServiceBLE
    {
        event SetupComplete Ready;

        string Id { get; }
        string ToString();
        void Unregister();

        List<ICharacteristicBLE> Characteristics { get; }
    }
}
