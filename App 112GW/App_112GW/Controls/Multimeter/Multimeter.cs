﻿using System;
using System.Threading;
using App_112GW;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;

namespace rMultiplatform
{
	public partial class Multimeter : AutoGrid
	{
        public event EventHandler   IdChanged;

		public BLE.IDeviceBLE	    mDevice;
		public SmartChart		    Chart;
		public SmartChartMenu	    ChartMenu;
		public SmartChartLogger	    Logger = new SmartChartLogger(10, SmartChartLogger.LoggerMode.Rescaling);

		public enum ActiveItem
		{
			Screen,
			FullscreenPlot
		}

		private ActiveItem _Item = ActiveItem.Screen;
		public ActiveItem Item
		{
			set
			{
				_Item = value;
				switch (Item)
				{
					case ActiveItem.Screen:
						RestoreItems();
						break;
					case ActiveItem.FullscreenPlot:
						MaximiseItem(Chart);    break;
				}
			}
			get
			{
				return _Item;
			}
		}

		private PacketProcessor MyProcessor = new PacketProcessor(0xF2, 52);
		public MultimeterScreen Screen;
		public MultimeterMenu Menu;

        Packet121GW processor = new Packet121GW();
        public new string Id { get; set; } = "";
        void ProcessPacket(byte[] pInput)
		{
			try
			{
				processor.ProcessPacket(pInput);
                Id = processor.Serial.ToString();
                IdChanged?.Invoke(this, EventArgs.Empty);

                Logger.Sample(processor.MainValue);

                var temp = processor.MainRangeLabel;
                ChartTitle = temp;
                Screen.Update(processor);
				Screen.InvalidateSurface();
			}
			catch
			{
				MyProcessor.Reset();
			}
		}

		public void		 Reset() => Logger.Reset();

        private string _ChartTitle = "NOT_A_TITLE";
		public string ChartTitle
		{
			set
			{
				if (value != _ChartTitle)
				{
                    Chart.Title = value;
                    _ChartTitle = value;
					Reset();
				}
			}
		}

		public Multimeter ( BLE.IDeviceBLE pDevice )
		{
			mDevice = pDevice ?? throw new Exception("Multimeter must connect to a BLE device, not null.");

			mDevice.Change += (o, e) => { MyProcessor.Recieve(e.Bytes); };
			MyProcessor.mCallback += ProcessPacket;

			Screen  = new MultimeterScreen();
			Menu	= new MultimeterMenu();

			#region MULTIMETER_BUTTON_EVENTS
			void SendKeycode(Packet121GW.Keycode keycode)
			{
				SendData(Packet121GW.GetKeycode(keycode));
			}
			Menu.HoldClicked		+= (s, e) => { SendKeycode(Packet121GW.Keycode.HOLD);   };
			Menu.RelClicked		 += (s, e) => { SendKeycode(Packet121GW.Keycode.REL);	};
			Menu.ModeChanged		+= (s, e) => { SendKeycode(Packet121GW.Keycode.MODE);   };
			Menu.RangeChanged	   += (s, e) => { SendKeycode(Packet121GW.Keycode.RANGE);  };
			#endregion

			#region CHART_CONSTRUCTION
			Chart = 
				new SmartChart(
				new SmartData(
					new SmartAxisPair(
						new SmartAxisHorizontal ("Horizontal",  -0.1f,  0.1f), 
						new SmartAxisVertical   ("Vertical",    -0.2f,  0.1f)), Logger.Data));

			Chart.Clicked += (o, e) => 
            {
                Plot_FullScreenClicked(o, e);
            };
			#endregion

			ChartMenu = new SmartChartMenu(true, true);
			ChartMenu.SaveClicked   += (s, e) => { Chart.SaveCSV(); };
			ChartMenu.ResetClicked  += (s, e) => { Reset();		 };

			DefineGrid(1, 4);
			AutoAdd(Screen);	FormatCurrentRow(GridUnitType.Star);
			AutoAdd(Menu);	    FormatCurrentRow(GridUnitType.Auto);
			AutoAdd(Chart);	    FormatCurrentRow(GridUnitType.Star);
            AutoAdd(ChartMenu); FormatCurrentRow(GridUnitType.Auto);

			Item = ActiveItem.Screen;
		}

        public override void OrientationChanged(Orientation New)
        {
            BatchBegin();
            if (Screen.IsVisible == true)
            {
                RestoreItems();
                if (New == Orientation.Landscape)
                    MaximiseItem(Screen);
            }
            BatchCommit();
        }

		private void Plot_FullScreenClicked(object sender, EventArgs e)
		{
			Item = (Item == ActiveItem.FullscreenPlot) ? ActiveItem.Screen : ActiveItem.FullscreenPlot;
		}
		private void SendData   (byte[] pData)
		{
			foreach (var serv in mDevice.Services)
				foreach(var chara in serv.Characteristics)
					chara.Send(pData);
		}
    }
}