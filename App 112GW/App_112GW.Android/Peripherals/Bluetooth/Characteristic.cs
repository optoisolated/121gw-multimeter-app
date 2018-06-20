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
	public class CharacteristicBLE : ICharacteristicBLE
	{
		public event ChangeEvent ValueChanged;
		public volatile ICharacteristic mCharacteristic;

		public string Id => mCharacteristic.Id.ToString();
		public string Description => mCharacteristic.Name;
		public void Send(string pInput) => Send(Encoding.UTF8.GetBytes(pInput));

		public void Send(byte[] pInput)
		{
			try
			{
				Task.Factory.StartNew(async () => await mCharacteristic.WriteAsync(pInput));
			}
			catch (Exception e)
			{
				Debug.WriteLine("Failed to Send.");
				Debug.WriteLine(e);
			}
		}

		//Event that is called when the value of the characteristic is changed
		private void CharacteristicEvent_ValueChanged(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs args)
		{
			var buffer = args.Characteristic.Value;
			var charEvent = new CharacteristicEvent(buffer);
			ValueChanged?.Invoke(sender, charEvent);
		}

		public CharacteristicBLE(ICharacteristic pInput, ChangeEvent pEvent)
		{
			ValueChanged += pEvent;
			mCharacteristic = pInput;
			mCharacteristic.ValueUpdated +=  CharacteristicEvent_ValueChanged;

			if (mCharacteristic.CanUpdate)
				Task.Factory.StartNew(async () => await mCharacteristic.StartUpdatesAsync());
		}

		public void Remake() => throw new NotImplementedException();
		public void Unregister() => throw new NotImplementedException();
	}
}