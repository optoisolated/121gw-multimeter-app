using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using App_121GW.BLE;

namespace App_121GW.BLE
{
	public class ServiceBLE : IServiceBLE
	{
		private volatile IService mService;
		public ChangeEvent ValueChanged { get; set; } 

		public List<ICharacteristicBLE>		Characteristics { get; }
		public string						Id => mService.Id.ToString();
		public override string				ToString() => Id;

		private void AddCharacteristics(IList<ICharacteristic> obj)
		{
			foreach (var item in obj)
			{
				Debug.WriteLine("Characteristic adding : " + item.Name);
				Characteristics.Add(new CharacteristicBLE(item, ValueChanged));
			}
		}
		private async void Build()
		{
			var characteristics = await mService.GetCharacteristicsAsync();
			AddCharacteristics(characteristics);
		}

		public ServiceBLE(IService pInput, ChangeEvent pEvent)
		{
			Characteristics = new List<ICharacteristicBLE>();
			mService		=   pInput;
			ValueChanged	=   pEvent;

			Task.Factory.StartNew(Build);
		}

		public void Remake() => throw new NotImplementedException();
		public void Unregister() => throw new NotImplementedException();
	}
}