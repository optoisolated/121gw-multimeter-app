using System;
using System.Text;
using System.Diagnostics;

namespace App_121GW.BLE
{
    public class CharacteristicEvent : EventArgs
    {
        public string NewValue;
        public byte[] Bytes;

        public CharacteristicEvent(byte[] pNewValue)
        {
            Bytes = pNewValue;
            NewValue = Encoding.UTF8.GetString(Bytes);

			Debug.WriteLine("CE : " + NewValue);
        }
    }
    public interface ICharacteristicBLE
	{
		event ChangeEvent ValueChanged;

		string Id { get; }
        string Description { get; }

        bool Send(string pInput);
        bool Send(byte[] pInput);

        void Remake();
	}
}