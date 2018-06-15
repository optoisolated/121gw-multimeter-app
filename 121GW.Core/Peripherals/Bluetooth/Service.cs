using System.Collections.Generic;

namespace App_121GW.BLE
{
    public interface IServiceBLE
	{
		ChangeEvent ValueChanged { get; set; }

		string Id { get; }
        string ToString();

		void Remake();

        List<ICharacteristicBLE> Characteristics { get; }
	}
}
