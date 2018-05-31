using System;
using System.Text;

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
        }
    }
    public interface ICharacteristicBLE
    {
        event SetupComplete Ready;

        string Id { get; }
        string Description { get; }

        bool Send(string pInput);
        bool Send(byte[] pInput);

        event ChangeEvent ValueChanged;

        void Unregister();
    }
}