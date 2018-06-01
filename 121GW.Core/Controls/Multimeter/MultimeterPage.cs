﻿using App_121GW.BLE;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace App_121GW
{
    public class MultimeterPage : GeneralPage
    {
        public Multimeter Device { get; set; }
        public MultimeterPage(IDeviceBLE pDevice) : base ("", new Multimeter(pDevice))
        {
            base.Title = "Test";
            Device = Content as Multimeter;
            Device.PropertyChanged += Device_PropertyChanged;
        }

        public new string Id
        {
            get { return base.Title; }
            set
            {
                base.Title = "[ " + value + " ]";
            }
        }
        private void Device_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Id")
				Globals.RunMainThread(() => { Id = Device.Id;});
        }
    }
}
