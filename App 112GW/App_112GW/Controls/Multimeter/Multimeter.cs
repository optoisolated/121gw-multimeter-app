﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Themes;
using Xamarin.Forms;
using App_112GW;
using System.Diagnostics;

namespace rMultiplatform
{
    public partial class Multimeter : ContentView
    {
        private BLE.IDeviceBLE mDevice;
        PacketProcessor MyProcessor = new PacketProcessor(0xF2, 26);
        void ProcessPacket(byte[] pInput)
        {
            var processor = new Packet112GW();

            try
            {
                processor.ProcessPacket(pInput);
                Data.Sample((float)processor.MainValue);
                Screen.Update(processor);
                Screen.InvalidateSurface();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public StackLayout              MultimeterGrid;
        public MultimeterScreen         Screen;
        public MultimeterMenu           Menu;
        public ChartData                Data;
        public ChartView                Plot;
        bool                            Item = true;

        private void ValueChanged ( object o, BLE.CharacteristicEvent v )
        {
            Debug.WriteLine("Recieved: " + v.NewValue);
            MyProcessor.Recieve(v.Bytes);
        }
        public Multimeter ( BLE.IDeviceBLE pDevice )
        {
            MyProcessor.mCallback += ProcessPacket; 
            mDevice = pDevice;
            mDevice.Change += ValueChanged;

            // 
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.StartAndExpand;

            // Assures that a non-zero height is allocated
            MinimumHeightRequest = 200;

            // InitializeComponent ();
            Screen = new MultimeterScreen();
            Screen.BackgroundColor = Globals.BackgroundColor;
            Screen.Clicked += Menu_BackClicked;

            var id = mDevice.Id;

            Menu = new MultimeterMenu(id.Substring(id.Length - 5));
            Menu.BackgroundColor = Globals.BackgroundColor;
            Menu.BackClicked    += Menu_BackClicked;
            Menu.PlotClicked    += Menu_PlotClicked;
            Menu.HoldClicked    += Menu_HoldClicked;
            Menu.RelClicked     += Menu_RelClicked;
            Menu.ModeChanged    += Menu_ModeChanged;
            Menu.RangeChanged   += Menu_RangeChanged;

            Data = new ChartData(ChartData.ChartDataMode.eRolling, "Time (s)", "Volts (V)", 0.1f, 10.0f);
            Plot = new ChartView() { Padding = new ChartPadding(0.1) };
            Plot.AddGrid(new ChartGrid());
            Plot.AddAxis(new ChartAxis(5, 5, 0, 20) {   Label = "Time (s)",     Orientation = ChartAxis.AxisOrientation.Horizontal, AxisLocation = 0.9, LockToAxisLabel = "Volts (V)",  LockAlignment = ChartAxis.AxisLock.eMiddle, ShowDataKey = false });
            Plot.AddAxis(new ChartAxis(5, 5, 0, 0)  {   Label = "Volts (V)",    Orientation = ChartAxis.AxisOrientation.Vertical,   AxisLocation = 0.1, LockToAxisLabel = "Time (s)", LockIndex = 1});
            Plot.AddData(Data);

            MultimeterGrid = new StackLayout();
            MultimeterGrid.Children.Add(Screen);
            MultimeterGrid.Children.Add(Menu);
            MultimeterGrid.Children.Add(Plot);

            Menu.IsVisible = true;
            Screen.IsVisible = false;
            Plot.IsVisible = false;

            Content = MultimeterGrid;
            SetView();
        }
        private void SendData(byte[] pData)
        {
            foreach (var serv in mDevice.Services)
                foreach(var chara in serv.Characteristics)
                    chara.Send(pData);
        }

        private void Menu_RangeChanged(object sender, EventArgs e)
        {
            var data = Packet112GW.GetKeycode(Packet112GW.Keycode.RANGE);
            SendData(data);
        }
        private void Menu_ModeChanged(object sender, EventArgs e)
        {
            var data = Packet112GW.GetKeycode(Packet112GW.Keycode.MODE);
            SendData(data);
        }
        private void Menu_RelClicked(object sender, EventArgs e)
        {
            var data = Packet112GW.GetKeycode(Packet112GW.Keycode.REL);
            SendData(data);
        }
        private void Menu_HoldClicked(object sender, EventArgs e)
        {
            var data = Packet112GW.GetKeycode(Packet112GW.Keycode.HOLD);
            SendData(data);
        }

        private void SetView()
        {
            try
            {
                switch (Item)
                {
                    case true:
                        Menu.IsVisible = true;
                        Screen.IsVisible = false;
                        break;
                    case false:
                        Menu.IsVisible = false;
                        Screen.IsVisible = true;
                        break;
                }

                Plot.IsVisible = Menu.PlotEnabled;
            }
            catch (Exception Msg)
            {
                Debug.WriteLine(Msg.Message);
            }
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
        }
        public void Menu_BackClicked(object sender, EventArgs e)
        {
            Item = !Item;
            SetView();
        }
        public void Menu_PlotClicked(object sender, EventArgs e)
        {
            SetView();
        }
    }
}