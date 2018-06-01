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

		private event EventHandler _LeftButtonEvent;
		private event EventHandler _RightButtonEvent;

		private event EventHandler LeftButtonEvent
		{
			add
			{
				_LeftButtonEvent += value;
			}
			remove
			{
				_LeftButtonEvent -= value;
			}
		}
		private event EventHandler RightButtonEvent
		{
			add
			{
				_RightButtonEvent += value;
			}
			remove
			{
				_RightButtonEvent -= value;
			}
		}

		private void ButtonRight_Clicked(object sender, EventArgs e)
		{
			_RightButtonEvent?.Invoke(sender, e);
		}
		private void ButtonLeft_Clicked(object sender, EventArgs e)
		{
			_LeftButtonEvent?.Invoke(sender, e);
		}
		
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
			_LeftButtonEvent = null;
			LeftButtonEvent += LeftEvent;
			LeftButtonText = LeftText;
			ButtonLeft.IsVisible = true;
		}
		public void SetRightButton(string RightText, EventHandler RightEvent)
		{
			_RightButtonEvent = null;
			RightButtonEvent += RightEvent;
			RightButtonText = RightText;
			ButtonRight.IsVisible = true;
		}

		public void ClearRightButton()
		{
			_RightButtonEvent = null;
			ButtonRight.IsVisible = false;
		}
		public void ClearLeftButton()
		{
			_LeftButtonEvent = null;
			ButtonLeft.IsVisible = false;
		}
	}
}