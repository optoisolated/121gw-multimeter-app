using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace App_121GW.BLE
{
    public class ServiceBLE : IServiceBLE
	{
		private GattDeviceService		mService;
		public ChangeEvent				ValueChanged	{ get; set; }
        public List<ICharacteristicBLE> Characteristics { get; private set; }

		private void CharacteristicsAquired(GattCharacteristicsResult pResult)
        {
            //Clear existing.
            Characteristics = null;
            Characteristics = new List<ICharacteristicBLE>();

			foreach (var item in pResult.Characteristics)
				Characteristics.Add(new CharacteristicBLE(item));

			foreach (var item in Characteristics)
				item.ValueChanged += ValueChanged;
		}
		void Build() => Task.Factory.StartNew(async ()=>CharacteristicsAquired(await mService.GetCharacteristicsAsync()));
		
		public void Remake()
		{
			foreach (var characteristic in Characteristics)
				characteristic.Remake();
		}

		public ServiceBLE(GattDeviceService pInput)
		{
			mService = pInput;
			Build();
		}
		~ServiceBLE()
		{
			Debug.WriteLine("Deregistering Service.");
			Remake();
			mService = null;
			Characteristics = null;
		}

		public string Id => mService.Uuid.ToString();
		public override string ToString() => Id;
	}
}
