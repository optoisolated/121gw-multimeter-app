using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading.Tasks;

namespace App_121GW.BLE
{
    public class ServiceBLE : IServiceBLE
	{
		private GattDeviceService mService;

		public ChangeEvent ValueChanged	{ get; set; }
		public List<ICharacteristicBLE> Characteristics { get; private set; } = new List<ICharacteristicBLE>();

		private void CharacteristicsAquired(GattCharacteristicsResult pResult)
        {
			//Clear existing
			Characteristics = null;
			Characteristics = new List<ICharacteristicBLE>();
			var characteristics = pResult.Characteristics;

			//Build list
			foreach (var item in characteristics)
				Characteristics.Add(new CharacteristicBLE(item));

			foreach (var item in Characteristics)
				item.ValueChanged += ValueChanged;
		}
		
		private async void Build() => CharacteristicsAquired(await mService.GetCharacteristicsAsync());
		public void Remake()
		{
			Debug.WriteLine("Service remaking");
			foreach (var characteristic in Characteristics)
				characteristic.Remake();
			Characteristics = new List<ICharacteristicBLE>();

			Task.Factory.StartNew(Build);
		}
		public ServiceBLE(GattDeviceService pInput)
		{
			mService = pInput;
			Task.Factory.StartNew(Build);
		}
		~ServiceBLE()
		{
			Debug.WriteLine("De-registering service");
			Remake();
			mService.Dispose();
			mService = null;
			Characteristics = null;
		}

		public string Id => mService.Uuid.ToString();
		public override string ToString() => Id;
	}
}
