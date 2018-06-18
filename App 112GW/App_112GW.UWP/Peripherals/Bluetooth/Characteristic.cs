using System;
using Windows.Security.Cryptography;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Diagnostics;
using System.Threading.Tasks;

namespace App_121GW.BLE
{
	public class CharacteristicBLE : ICharacteristicBLE
	{
		private GattCharacteristic mCharacteristic;
		public event ChangeEvent ValueChanged;

		void TriggerChange(GattCharacteristic sender, CharacteristicEvent ChangeEvent)
		{
			ValueChanged?.Invoke(sender, ChangeEvent);
		}

		public bool Send(string pInput)
		{
			Task.Factory.StartNew(async () => await mCharacteristic.WriteValueAsync(CryptographicBuffer.ConvertStringToBinary(pInput, BinaryStringEncoding.Utf8)));
			return true;
		}
		public bool Send(byte[] pInput)
		{
			Task.Factory.StartNew(async () => await mCharacteristic.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(pInput)));
            return true;
		}

		//Event that is called when the value of the characteristic is changed
		private void CharacteristicEvent_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{
			Debug.WriteLine("Value changed");
			CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out byte[] data);
			TriggerChange(sender, new CharacteristicEvent(data));
		}
		async void Build()
		{
			int properties = (int)mCharacteristic.CharacteristicProperties;
            int indicate_mask = (int)GattCharacteristicProperties.Indicate;
			if ((properties & indicate_mask) != 0)
			{
				Debug.WriteLine("Setting up Indicate.");
                var obj2 = await mCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);
				mCharacteristic.ValueChanged += CharacteristicEvent_ValueChanged;
			}
		}
		public void Remake()
		{
			if (mCharacteristic == null) return;

			try { mCharacteristic.ValueChanged -= CharacteristicEvent_ValueChanged; }
			catch { }
		}

		public CharacteristicBLE(GattCharacteristic pInput)
		{
			mCharacteristic = pInput;
			Build();
		}
		~CharacteristicBLE()
		{
			Debug.WriteLine("Destructing Characteristic.");
			Remake();
			ValueChanged = null;
			mCharacteristic = null;
		}

		public string Id => mCharacteristic.Uuid.ToString();
		public string Description => mCharacteristic.UserDescription;
	}
}
