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
		public event SetupComplete			Ready;

		private ChangeEvent					mEvent;
		private volatile IService			mService;

		public List<ICharacteristicBLE>		Characteristics { get; }
		public string						Id => mService.Id.ToString();

		void TriggerReady() => Ready?.Invoke();
		public override string ToString() => Id;

		private int Uninitialised = 0;
		private void ItemReady()
		{
			if (--Uninitialised == 0)
				TriggerReady();
		}
		private void AddCharacteristics(IList<ICharacteristic> obj)
		{
			Uninitialised = obj.Count;
			foreach (var item in obj)
			{
				Debug.WriteLine("Characteristic adding : " + item.Name);
				Characteristics.Add(new CharacteristicBLE(item, ItemReady, mEvent));
			}
		}
		private async void Build()
		{
			var characteristics = await mService.GetCharacteristicsAsync();
			AddCharacteristics(characteristics);
		}

		public ServiceBLE(IService pInput, SetupComplete ready, ChangeEvent pEvent)
		{
			Characteristics = new List<ICharacteristicBLE>();
			Ready			+=  ready;
			mService		=   pInput;
			mEvent			=   pEvent;

			Task.Factory.StartNew(Build);
		}

		public void Remake() => throw new NotImplementedException();
		public void Unregister() => throw new NotImplementedException();
	}
}