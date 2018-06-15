using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace App_121GW.BLE
{
	public class UnPairedDeviceBLE : IDeviceBLE
	{
		public IDevice						mDevice;
		public event DeviceSetupComplete	Ready;
		public event ChangeEvent			Change;

		public string	Id							=> mDevice.Id.ToString();
		public string	Name						=> mDevice.Name;
		public bool		Paired						=> (mDevice.State == Plugin.BLE.Abstractions.DeviceState.Connected);
		public bool		CanPair						=> true;
		public UnPairedDeviceBLE(IDevice pDevice)	=> mDevice = pDevice;
		public override string ToString()			=> Name + "\n" + Id;
		public void Remake(object o)				=> throw new NotImplementedException();
		public void Unregister()					=> throw new NotImplementedException();
		public List<IServiceBLE> Services			=> null;
	}
	public class PairedDeviceBLE : IDeviceBLE
	{
		public event DeviceSetupComplete Ready;
		public event ChangeEvent Change;
		private void InvokeChange(object o, CharacteristicEvent v) => Change?.Invoke(o, v);

		private IDevice mDevice;

		public string Id	=> mDevice.Id.ToString();
		public string Name	=> mDevice.Name;
		public bool Paired	=> (mDevice.State == Plugin.BLE.Abstractions.DeviceState.Connected);
		public bool CanPair	=> true;

		private int UninitialisedServices = 0;
		private void ServiceReady()
		{
			if (--UninitialisedServices == 0)
			{
				Ready?.Invoke(this);
				Ready = null;
			}
		}
		private void AddServices(IList<IService> obj)
		{
			UninitialisedServices = obj.Count;
			foreach (var item in obj)
			{
				Debug.WriteLine("Service adding : " + item.Name);
				Services.Add(new ServiceBLE(item, ServiceReady, InvokeChange));
			}
		}
		private void Build()
		{
			Task.Factory.StartNew(async () => AddServices(await mDevice.GetServicesAsync()));
		}
		public PairedDeviceBLE(IDevice pDevice, DeviceSetupComplete ready)
		{
			Services = new List<IServiceBLE>();
			mDevice = pDevice;
			Ready = ready;
			Build();
		}
		public override string ToString() => Name + "\n" + Id;

		public void Remake(object o)
		{
			Debug.WriteLine("Remaking.");
			var dev = o as IDevice;
			mDevice = null;
			mDevice = dev;
			Services = null;
			Services = new List<IServiceBLE>();
			Build();
		}

		public void Unregister() {}
		public List<IServiceBLE> Services { get; private set; }
	}
}