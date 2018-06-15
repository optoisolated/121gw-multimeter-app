using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace App_121GW.BLE
{
    public class ServiceBLE : IServiceBLE
    {
        private volatile IService mService;

		public ChangeEvent ValueChanged { get; set; }
		public List<ICharacteristicBLE> Characteristics { get; private set; }
		
		private void AddCharacteristics(IList<ICharacteristic> obj)
		{
			foreach (var item in obj)
				Characteristics.Add(new CharacteristicBLE(item));

			foreach (var ch in Characteristics)
				ch.ValueChanged += ValueChanged;
		}
		private void Build()
        {
			Task.Factory.StartNew(async () => AddCharacteristics(await mService.GetCharacteristicsAsync()) );
        }
        public ServiceBLE(IService pInput)
        {
            Characteristics = new List<ICharacteristicBLE>();
            mService = pInput;
            Build();
		}
		public void Remake()
		{
			Debug.WriteLine("Service remaking...");
			foreach (var ch in Characteristics) ch.Remake();
			Characteristics = new List<ICharacteristicBLE>();

			mService.Dispose();
			mService = null;

			Build();
		}

		public void Unregister() => throw new NotImplementedException();
		public string Id => mService.Id.ToString();
		public override string ToString() => Id;
	}
} 