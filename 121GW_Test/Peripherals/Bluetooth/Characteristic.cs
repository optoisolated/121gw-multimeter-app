using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace App_121GW.BLE
{
	public class CharacteristicBLE : ICharacteristicBLE
	{
        public volatile ICharacteristic mCharacteristic;
        public event ChangeEvent ValueChanged;

        public bool Send(string pInput) => Send(Encoding.UTF8.GetBytes(pInput));
        public bool Send(byte[] pInput)
		{
			Task.Factory.StartNew(async () => await mCharacteristic.WriteAsync(pInput));
			return true;
		}

        //Event that is called when the value of the characteristic is changed
        private void CharacteristicEvent_ValueChanged(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs args)
        {
			Debug.WriteLine("Value changed");
            ValueChanged?.Invoke(sender, new CharacteristicEvent(args.Characteristic.Value));
        }

        public CharacteristicBLE(ICharacteristic pInput)
        {
            mCharacteristic = pInput;

			if (mCharacteristic.CanUpdate)
				Task.Factory.StartNew(async () => 
				{
					await mCharacteristic.StartUpdatesAsync();
					mCharacteristic.ValueUpdated += CharacteristicEvent_ValueChanged;
				});
		}

		public void Remake()
		{
			Debug.WriteLine("Characteristic remaking");

			if (mCharacteristic.CanUpdate)
				mCharacteristic.ValueUpdated -= CharacteristicEvent_ValueChanged;

			mCharacteristic = null;
			ValueChanged = null;
		}

		public string Id => mCharacteristic.Id.ToString();
		public string Description => mCharacteristic.Name;
	}
}