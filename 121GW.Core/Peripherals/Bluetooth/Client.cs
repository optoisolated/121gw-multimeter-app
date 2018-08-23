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
		void Clear();

        Task<IDeviceBLE> Connect(IDeviceBLE pInput);
	}
    public abstract class AClientBLE
    {
		private readonly IBluetoothDeviceFilter mFilter;
		public ObservableCollection<IDeviceBLE> ConnectedDevices { get; private set; } = new ObservableCollection<IDeviceBLE>();
		public ObservableCollection<IDeviceBLE> VisibleDevices { get; private set; } = new ObservableCollection<IDeviceBLE>();


		public void Clear()
		{
			VisibleDevices.Clear();
		}

		public IDeviceBLE DeviceConnected( IDeviceBLE pInput )
		{
			MutexBlock(() => ConnectedDevices?.Add(pInput));
			return pInput;
		}

        private Mutex mut = new Mutex();
        void MutexBlock( Action Function, string tag = "" )
		{
			void GetMutex() => mut.WaitOne();
            void ReleaseMutex() => mut.ReleaseMutex();
			GetMutex();
			try
            {
                Function();
            }
            catch{}
			ReleaseMutex();
		}

        public void AddUniqueItem( IDeviceBLE pInput )
		{
			if ( pInput != null && pInput?.Name != null )
            {
                try
				{
					MutexBlock(() =>
					{
						bool add = true;
						foreach (var device in VisibleDevices)
							if (device.Id == pInput.Id)
								add = false;

						if (add && (pInput.Name.Length > 0))
							if (mFilter.IdAccepted(pInput) && mFilter.NameAccepted(pInput))
							{
								foreach (var item in ConnectedDevices)
									if (item.Id.GetHashCode() == pInput.Id.GetHashCode())
										add = false;

								if (add) VisibleDevices.Add(pInput);
							}
					}, "Adding");
                }
                catch ( Exception e )
                {
                    Debug.WriteLine("Error Caught : public bool AddUniqueItem(IDeviceBLE pInput)");
                    Debug.WriteLine(e);
                }
            }
        }

		public void UpdateItem( Action<IDeviceBLE> pAction )
		{
			MutexBlock(() =>
			{
				foreach (var item in VisibleDevices) pAction(item);
			}, "Updating");
		}

		public AClientBLE(IBluetoothDeviceFilter pFilter)
		{
			mFilter = pFilter;
		}
	}
}