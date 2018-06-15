using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace App_121GW.BLE
{
    public class ServiceBLE : IServiceBLE
	{
		private GattDeviceService mService;
		public event SetupComplete Ready;
		void TriggerReady()
		{
			Debug.WriteLine("Service ready.");
			Ready?.Invoke();
		}
        public List<ICharacteristicBLE> Characteristics { get; private set; }

        public string Id => mService.Uuid.ToString();
		public override string ToString() => Id;

		async void Build(ChangeEvent pEvent)
		{
            var arg = await mService.GetCharacteristicsAsync();
			CharacteristicsAquired(arg, pEvent);
		}
		private int Uninitialised = 0;
		private void ItemReady()
		{
			--Uninitialised;
			if (Uninitialised == 0)
				TriggerReady();
		}
		private void CharacteristicsAquired(GattCharacteristicsResult result, ChangeEvent pEvent)
        {
            //Clear existing.
            Characteristics = null;
            Characteristics = new List<ICharacteristicBLE>();

            //Build list
            var characteristics = result.Characteristics;
			Uninitialised = characteristics.Count;
			foreach (var item in result.Characteristics)
				Characteristics.Add(new CharacteristicBLE(item, ItemReady, pEvent));
		}
		
		public void Remake()
		{
			foreach (var characteristic in Characteristics)
				characteristic.Remake();
		}

		public ServiceBLE(GattDeviceService pInput, SetupComplete ready, ChangeEvent pEvent)
		{
			Ready	   = ready;
			mService	= pInput;
			Build(pEvent);
		}
		~ServiceBLE()
		{
			Debug.WriteLine("Deregistering Service.");
			Remake();
			mService.Dispose();
			mService = null;
			Characteristics = null;
		}
	}
}
