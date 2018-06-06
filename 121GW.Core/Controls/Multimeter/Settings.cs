using App_121GW;
using App_121GW.BLE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace App_121GW
{
	public class Settings : AutoGrid
	{
		public delegate void AddBluetoothDevice(IDeviceBLE pDevice);
		public event AddBluetoothDevice AddDevice;

		private BluetoothDeviceSelector BluetoothSelectDevice = new BluetoothDeviceSelector();
		private GeneralButton ButtonLeft;
		private GeneralButton ButtonRight;

		public Settings ()
		{
			ButtonLeft  = new GeneralButton("", ButtonLeft_Clicked);
			ButtonRight = new GeneralButton("", ButtonRight_Clicked);

			//Setup connected event
			DefineGrid(2, 2);

			//Setup default display
			AutoAdd(BluetoothSelectDevice, 2);
			FormatCurrentRow(GridUnitType.Star);
			
			AutoAdd(ButtonLeft);
			AutoAdd(ButtonRight);
			FormatCurrentRow(GridUnitType.Auto);
			
			ClearRightButton();
			ClearLeftButton();

			SetLeftButton("Refresh", RefreshDevices);
			BluetoothSelectDevice.Connected += Connected;
		}

		private async void RefreshDevices(object sender, EventArgs e)
		{
			await BluetoothSelectDevice.Reset();
		}
		private void Connected(IDeviceBLE pDevice)
		{
			if (pDevice == null) return;
			AddDevice?.Invoke(pDevice);
		}

		private event EventHandler RightButtonEvent;
		private event EventHandler LeftButtonEvent;

		private void ButtonRight_Clicked(object sender, EventArgs e) => RightButtonEvent?.Invoke(sender, e);
		private void ButtonLeft_Clicked(object sender, EventArgs e) => LeftButtonEvent?.Invoke(sender, e);
		
		private string LeftButtonText
		{
			set
			{
				ButtonLeft.Text = value;
			}
		}
		private string RightButtonText
		{
			set
			{
				ButtonRight.Text = value;
			}
		}

		public void SetLeftButton(string LeftText, EventHandler LeftEvent)
		{
			LeftButtonEvent = null;
			LeftButtonEvent += LeftEvent;
			LeftButtonText = LeftText;
			ButtonLeft.IsVisible = true;
		}
		public void SetRightButton(string RightText, EventHandler RightEvent)
		{
			RightButtonEvent = null;
			RightButtonEvent += RightEvent;
			RightButtonText = RightText;
			ButtonRight.IsVisible = true;
		}
		public void ClearRightButton()
		{
			RightButtonEvent = null;
			ButtonRight.IsVisible = false;
		}
		public void ClearLeftButton()
		{
			LeftButtonEvent = null;
			ButtonLeft.IsVisible = false;
		}
	}
}