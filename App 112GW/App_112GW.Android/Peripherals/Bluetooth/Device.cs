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
		public event DeviceSetupComplete Ready;
		public ChangeEvent ValueChanged { get; set; }
		public IDevice mDevice;

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
		public ChangeEvent ValueChanged{ get; set; }
		private void InvokeChange(object o, CharacteristicEvent v) => ValueChanged?.Invoke(o, v);

		private IDevice mDevice;

		public string Id	=> mDevice.Id.ToString();
		public string Name	=> mDevice.Name;
		public bool Paired	=> (mDevice.State == Plugin.BLE.Abstractions.DeviceState.Connected);
		public bool CanPair	=> true;

		private void AddServices(IList<IService> obj)
		{
			foreach (var item in obj)
			{
				Debug.WriteLine("Service adding : " + item.Name);
				Services.Add(new ServiceBLE(item, InvokeChange));
			}
		}
		private void Build()
		{
			Task.Factory.StartNew(async () => AddServices(await mDevice.GetServicesAsync()));
		}
		public PairedDeviceBLE(IDevice pDevice)
		{
			Services = new List<IServiceBLE>();
			mDevice = pDevice;
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