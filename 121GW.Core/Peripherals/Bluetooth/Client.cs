using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace App_121GW.BLE
{
    public interface IClientBLE
    {
        event ConnectedEvent DeviceConnected;

        //Default functions
        void Start();
        void Stop();
        void Rescan();
        void Reset();
        void RemoveAll();

        //Does not return a usable device, it must be paired first
        ObservableCollection<IDeviceBLE> ListDevices();
        void Connect(IDeviceBLE pInput);
    }
    public abstract class AClientBLE
    {
        public volatile ObservableCollection<IDeviceBLE> mVisibleDevices = new ObservableCollection<IDeviceBLE>();
        public volatile ObservableCollection<IDeviceBLE> mConnectedDevices = null;

        private Mutex mut = new Mutex();
        public event ConnectedEvent DeviceConnected;

        public void RemoveAll()
        {
            mVisibleDevices.Clear();
            if (mConnectedDevices != null)
                mConnectedDevices.Clear();
        }

        public void RemoveDevice(string pId)
        {
            foreach (var dev in mVisibleDevices)
                if (dev.Id == pId)
                    mVisibleDevices.Remove(dev);
        }
        public void RemoveDevice(IDeviceBLE pInput)
        {
            RemoveDevice(pInput.Id);
        }

        public void TriggerDeviceConnected(IDeviceBLE pInput)
        {
            MutexBlock(() =>
            {
                Debug.WriteLine("Finished connecting to : " + pInput.Id);
                if (mConnectedDevices != null)
                    mConnectedDevices.Add(pInput);
                RemoveDevice(pInput);
            });

            Globals.RunMainThread(() =>
            {
                DeviceConnected?.Invoke(pInput);
            });
        }

        public void MutexBlock(Action Function, string tag = "")
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
            try
            {
                GetMutex(tag);
                Function();
                ReleaseMutex(tag);
            }
            catch
            {
                ReleaseMutex(tag);
            }
        }

        public bool AddUniqueItem(IDeviceBLE pInput)
        {
            if (pInput != null)
            {
                try
                {
                    bool add = true;
                    foreach (var device in mVisibleDevices)
                        if (device.Id == pInput.Id)
                            add = false;

                    if (add)
                        if (pInput.Name != null)
                            if (pInput.Name.Length != 0)
                                mVisibleDevices.Add(pInput);

                    return add;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error Caught : public bool AddUniqueItem(IDeviceBLE pInput)");
                    Debug.WriteLine(e);
                }
            }
            return false;
        }
        public ObservableCollection<IDeviceBLE> ListDevices()
        {
            if (mVisibleDevices == null)
                return new ObservableCollection<IDeviceBLE>();
            return mVisibleDevices;
        }
    }
}