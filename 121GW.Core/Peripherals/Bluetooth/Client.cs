using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace App_121GW.BLE
{
    public interface IClientBLE
    {
        ObservableCollection<IDeviceBLE> VisibleDevices { get; }

        //Default functions
        Task Start();
        Task Stop();
        Task Rescan();
        Task Reset();

        Task<IDeviceBLE> Connect(IDeviceBLE pInput);
    }
    public abstract class AClientBLE
    {
        static ObservableCollection<IDeviceBLE> mVisibleDevices = new ObservableCollection<IDeviceBLE>();
		protected static ObservableCollection<IDeviceBLE> mConnectedDevices = new ObservableCollection<IDeviceBLE>();

		public ObservableCollection<IDeviceBLE> VisibleDevices => mVisibleDevices;

        public void RemoveDevice(IDeviceBLE pInput)
        {
            var pId = pInput.Id;
            foreach (var dev in mVisibleDevices)
                if (dev.Id == pId)
                    mVisibleDevices.Remove(dev);
        }

        public void TriggerDeviceConnected(IDeviceBLE pInput)
        {
            MutexBlock(() =>
            {
                Debug.WriteLine("Finished connecting to : " + pInput.Id);
                mConnectedDevices?.Add(pInput);
                RemoveDevice(pInput);
            });
        }

        private Mutex mut = new Mutex();
        void MutexBlock(Action Function, string tag = "")
        {
            void GetMutex(string ltag = "")
            {
                Debug.WriteLine(ltag + " : Waiting");
                mut.WaitOne();
                Debug.WriteLine(ltag + " : Started");
            }
            void ReleaseMutex(string ltag = "")
            {
                Debug.WriteLine(ltag + " : Done");
                mut.ReleaseMutex();
                Debug.WriteLine(ltag + " : Released");
			}
			GetMutex(tag);
			try
            {
                Function();
            }
            catch{}
			ReleaseMutex(tag);
		}

        public void AddUniqueItem(IDeviceBLE pInput)
        {
            if (pInput != null && pInput?.Name != null)
            {
                try
				{
					MutexBlock(() =>
					{
						bool add = true;
						foreach (var device in mVisibleDevices)
							if (device.Id == pInput.Id)
								add = false;

						if (add && (pInput.Name.Length > 0))
							mVisibleDevices.Add(pInput);
					}, "Adding");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error Caught : public bool AddUniqueItem(IDeviceBLE pInput)");
                    Debug.WriteLine(e);
                }
            }
        }

		public void UpdateItem(Action<IDeviceBLE> pAction)
		{
			MutexBlock(() =>
			{
				foreach (var item in mVisibleDevices)
					pAction(item);
			}, "Updating");
		}
	}
}