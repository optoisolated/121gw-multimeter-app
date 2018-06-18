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
		public List<ICharacteristicBLE> Characteristics { get; private set; } = new List<ICharacteristicBLE>();

		private void AddCharacteristics(IList<ICharacteristic> obj)
		{
			foreach (var item in obj)
				Characteristics.Add(new CharacteristicBLE(item));

			foreach (var item in Characteristics)
				item.ValueChanged += ValueChanged;
		}

		private async void Build() => AddCharacteristics(await mService.GetCharacteristicsAsync());
		public void Remake()
		{
			Debug.WriteLine("Service remaking");
			foreach (var characteristic in Characteristics)
				characteristic.Remake();
			Characteristics = new List<ICharacteristicBLE>();

			mService.Dispose();
			mService = null;

			Task.Factory.StartNew(Build);
		}

		public ServiceBLE(IService pInput)
		{
			mService = pInput;
			Task.Factory.StartNew(Build);
		}
		~ServiceBLE() { }

		public string Id => mService.Id.ToString();
		public override string ToString() => Id;
	}
}