using System;
using Xamarin.Forms;

namespace App_121GW
{
	public class Multimeter : AutoGrid
    {
        public BLE.IDeviceBLE	mDevice;
        public SmartChart		Chart;
        public SmartChartMenu	ChartMenu;
        public SmartChartLogger Logger = new SmartChartLogger(10, SmartChartLogger.LoggerMode.Rescaling);

		private PacketProcessor MyProcessor = new PacketProcessor(0xF2, 19);
		private Packet121GW		Processor = new Packet121GW();
		public MultimeterScreen Screen;
		public MultimeterMenu	Menu;

		public enum ActiveItem
        {
            Plot,
            Screen,
            Both
        }
        private ActiveItem _Item = ActiveItem.Screen;
        public ActiveItem Item
        {
            set
            {
                _Item = value;
                BatchBegin();
                switch (value)
                {
                    case ActiveItem.Screen:
                        RestoreItems();
						MaximiseItem(Screen);
                        break;
                    case ActiveItem.Plot:
                        RestoreItems();
						MinimiseItems(Menu, Screen);
						break;
                    case ActiveItem.Both:
                        RestoreItems();
                        break;
                }
                BatchCommit();
            }
            get
            {
                return _Item;
            }
        }
        void FullscreenClicked(ActiveItem pItem)
        {
            if (CurrentOrientation == Orientation.Landscape)
            {
                switch (Item)
                {
                    case ActiveItem.Plot:
                        Item = ActiveItem.Screen;
                        break;
                    case ActiveItem.Screen:
                        Item = ActiveItem.Plot;
                        break;
                    case ActiveItem.Both:
                        Item = pItem;
                        break;
                };
            }
        }

        private string _Id = "Device";
        public new string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    base.OnPropertyChanged("Id");
                }
            }
        }
        private string _ChartTitle = "NOT_A_TITLE";

        bool TitleSetDeferred = false;
        public string ChartTitle
        {
            set
            {
                if (value != _ChartTitle || TitleSetDeferred)
                {
                    if (Chart == null)
                        TitleSetDeferred = true;
                    else
                    {
                        Chart.Title = value;
                        TitleSetDeferred = false;
                    }

                    _ChartTitle = value;
                    base.OnPropertyChanged("ChartTitle");
                    Reset();
                }
            }
        }

        private bool PacketReady => Screen != null && Logger != null;

        void ProcessPacket(byte[] pInput)
        {
            if (Processor.ProcessPacket(pInput))
            {
                if (PacketReady)
                {
                    try
                    {
                        Id = Processor.Serial.ToString();
                        Logger.Sample(Processor.MainValue * (float)Processor.MainRangeMultiple);
                        ChartTitle = Processor.MainRangeLabel;
                        Screen.Update(Processor);
                        Screen.InvalidateSurface();
                    }
                    catch
                    {
                        MyProcessor.Reset();
                    }
                }
                else
                    MyProcessor.Reset();
            }
            else
                MyProcessor.Reset();
        }
        public void Reset() => Logger.Reset();
        public Multimeter(BLE.IDeviceBLE pDevice)
        {
            mDevice = pDevice ?? throw new Exception("Multimeter must connect to a BLE device, not null.");

            mDevice.ValueChanged += (o, e) => MyProcessor.Recieve(e.Bytes);
            MyProcessor.mCallback += ProcessPacket;

            Screen = new MultimeterScreen();
            Screen.Clicked += (o, e) => { FullscreenClicked(ActiveItem.Screen); }; ;
            Menu = new MultimeterMenu();

            void SendKeycode(Packet121GW.Keycode keycode)
            {
                SendData(Packet121GW.GetKeycode(keycode));
            }

            Menu.HoldClicked	+= (s, e) => { SendKeycode(Packet121GW.Keycode.HOLD); };
            Menu.RelClicked		+= (s, e) => { SendKeycode(Packet121GW.Keycode.REL); };
            Menu.ModeChanged	+= (s, e) => { SendKeycode(Packet121GW.Keycode.MODE); };
            Menu.RangeChanged	+= (s, e) => { SendKeycode(Packet121GW.Keycode.RANGE); };

            Chart =
                new SmartChart(
                new SmartData(
                    new SmartAxisPair(
                        new SmartAxisHorizontal	("Horizontal",	-0.1f, 0.1f),
                        new SmartAxisVertical	("Vertical",	-0.2f, 0.1f)), Logger.Data));

            Chart.Clicked += (o, e) => { FullscreenClicked(ActiveItem.Plot); };

            ChartMenu = new SmartChartMenu(true, true);
            ChartMenu.SaveClicked += (s, e) => { Chart.SaveCSV(); };
            ChartMenu.ResetClicked += (s, e) => { Reset(); };

            DefineGrid(1, 4);
            AutoAdd(Screen); FormatCurrentRow(GridUnitType.Auto);
            AutoAdd(Menu); FormatCurrentRow(GridUnitType.Auto);
            AutoAdd(Chart); FormatCurrentRow(GridUnitType.Star);
            AutoAdd(ChartMenu); FormatCurrentRow(GridUnitType.Auto);

            Item = ActiveItem.Both;
            Screen.Enable();
            Chart.Enable();
        }
        
        public override void OrientationChanged(Orientation Update)
        {
            if (CurrentOrientation != Update)
            {
                if (Update == Orientation.Landscape)
                {
                    if (Item == ActiveItem.Both)
                        Item = ActiveItem.Screen;
                }
                else Item = ActiveItem.Both;
            }

            //Must be called at end.
            base.OrientationChanged(Update);
        }

		private void SendData   (byte[] pData)
		{
			foreach (var serv in mDevice.Services)
				foreach(var chara in serv.Characteristics)
					chara.Send(pData);
		}
    }
}