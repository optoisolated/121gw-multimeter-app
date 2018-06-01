using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace App_121GW.BLE
{
    public class ServiceBLE : IServiceBLE
    {
        public event SetupComplete Ready;
        void TriggerReady()
        {
            Ready?.Invoke();
        }

        private ChangeEvent mEvent;
        private volatile IService mService;
        public List<ICharacteristicBLE> Characteristics { get; }
        public string Id => mService.Id.ToString();
        public override string ToString() => Id;

        private void Build()
        {
            mService.GetCharacteristicsAsync().ContinueWith((obj) => { AddCharacteristics(obj); });
        }


        private int Uninitialised = 0;
        private void ItemReady()
        {
            --Uninitialised;
            if (Uninitialised == 0)
                TriggerReady();
        }
        private void AddCharacteristics(Task<IList<ICharacteristic>> obj)
        {
            Uninitialised = obj.Result.Count;
            foreach (var item in obj.Result)
            {
                Debug.WriteLine("Characteristic adding : " + item.Name);
                Characteristics.Add(new CharacteristicBLE(item, ItemReady, mEvent));
            }
        }

        public void Remake()
        {
            throw new NotImplementedException();
        }

        public void Unregister()
        {
            throw new NotImplementedException();
        }

        public ServiceBLE(IService pInput, SetupComplete ready, ChangeEvent pEvent)
        {
            Characteristics = new List<ICharacteristicBLE>();
            Ready += ready;
            mService = pInput;
            mEvent = pEvent;

            Build();
        }
    }
}