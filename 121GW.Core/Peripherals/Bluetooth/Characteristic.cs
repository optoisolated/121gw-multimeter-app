using System;
using System.Text;
using System.Diagnostics;

namespace App_121GW.BLE
{
    public class CharacteristicEvent : EventArgs
    {
        public byte[] Bytes;
		override public string ToString() => Encoding.UTF8.GetString(Bytes);
		public CharacteristicEvent(byte[] pNewValue)
		{
			Bytes = pNewValue;

			Debug.WriteLine(ToString());
		}
    }
    public interface ICharacteristicBLE
	{
		event ChangeEvent ValueChanged;

		string Id { get; }
        string Description { get; }

        void Send(string pInput);
        void Send(byte[] pInput);

        void Remake();
	}
}