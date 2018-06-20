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

		public string Id	=> mDevice.Id.ToString();
        public string Name	=> mDevice.Name;
		public bool Paired	=> (mDevice.State == Plugin.BLE.Abstractions.DeviceState.Connected);
        public bool CanPair => true;

        public override string ToString()	=>  Name + "\n" + Id;

        public void Remake(object o)		=> throw new NotImplementedException();
        public void Unregister()			=> throw new NotImplementedException();
		public void Dispose()				=> throw new NotImplementedException();

		public List<IServiceBLE> Services	=> null;

		public UnPairedDeviceBLE(IDevice pDevice) { mDevice = pDevice; }
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class PairedDeviceBLE : IDeviceBLE
	{
		private IDevice mDevice;
		public ChangeEvent ValueChanged { get; set; } = null;
		public List<IServiceBLE> Services { get; set; } = new List<IServiceBLE>();

		void AddServices(IList<IService> pServices)
		{
			Debug.WriteLine("Services Aquired");
			foreach (var item in pServices)
				Services.Add(new ServiceBLE(item));

			foreach (var service in Services)
				service.ValueChanged += (o, e) => ValueChanged?.Invoke(o, e);
		}
		void Build() => Task.Factory.StartNew( async () => AddServices( await mDevice.GetServicesAsync() ) );

		public void Remake(object o)
		{
			mDevice = null;
			mDevice = o as IDevice;

			Debug.WriteLine("Remaking");

			foreach (var se in Services) se.Remake();
			Services = null;
			Services = new List<IServiceBLE>();

			Build();
		}
		public void Unregister() { }

		public PairedDeviceBLE( IDevice pDevice )
		{
			mDevice = pDevice;
			Build();
		}
		~PairedDeviceBLE()
		{
		}

		public string Id => mDevice.Id.ToString();
		public string Name => mDevice.Name;
		public bool Paired => (mDevice.State == Plugin.BLE.Abstractions.DeviceState.Connected);
		public bool CanPair => true;

		public override string ToString() => Name + "\n" + Id;
	}
}