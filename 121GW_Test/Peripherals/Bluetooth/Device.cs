using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace App_121GW.BLE
{
    public class UnPairedDeviceBLE : IDeviceBLE
    {
        public IDevice mDevice;
        public ChangeEvent ValueChanged { get; set; }

		public string Id => mDevice.Id.ToString();
        public string Name => mDevice.Name;
		public bool Paired => (mDevice.State == Plugin.BLE.Abstractions.DeviceState.Connected);
        public bool CanPair => true;

        public UnPairedDeviceBLE(IDevice pDevice) { mDevice = pDevice; }
        public override string ToString() =>  Name + "\n" + Id;

        public void Remake(object o) => throw new NotImplementedException();
        public void Unregister() => throw new NotImplementedException();
		public void Dispose() => throw new NotImplementedException();

		public List<IServiceBLE> Services => null;
    }
    public class PairedDeviceBLE : IDeviceBLE
	{
		private IDevice mDevice;
		public ChangeEvent ValueChanged { get; set; }

		public List<IServiceBLE> Services { get; private set; }

		void AddServices(IList<IService> obj)
		{
			foreach (var item in obj)
			{
				Debug.WriteLine("Service adding : " + item.Name);
				Services.Add(new ServiceBLE(item));
			}
			foreach (var service in Services)
				service.ValueChanged += ValueChanged;
		}
		void Build() => Task.Factory.StartNew(async () => AddServices(await mDevice.GetServicesAsync()));

        public PairedDeviceBLE(IDevice pDevice)
        {
            Services = new List<IServiceBLE>();
            mDevice = pDevice;
            Build();
		}
		public void Remake(object o)
		{
			Debug.WriteLine("Remaking.");

			foreach (var se in Services) se.Remake();
			Services = null;
			Services = new List<IServiceBLE>();

			mDevice = null;
			mDevice = o as IDevice;

			Build();
		}
		public void Unregister() { }

		public string			Id => mDevice.Id.ToString();
		public override string	ToString() => Name + "\n" + Id;
		public string			Name => mDevice.Name;
		public bool				Paired => (mDevice.State == Plugin.BLE.Abstractions.DeviceState.Connected);
		public bool				CanPair => true;
	}
}