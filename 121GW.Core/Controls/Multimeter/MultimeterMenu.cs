﻿using System;
using Xamarin.Forms;
using App_121GW;

namespace App_121GW
{
	public class MultimeterMenu : AutoGrid
	{
		public event EventHandler HoldClicked;
		public event EventHandler RelClicked;
		public event EventHandler ModeChanged;
		public event EventHandler RangeChanged;

		private readonly GeneralButton mMode;
		private readonly GeneralButton mHold;
		private readonly GeneralButton mRange;
		private readonly GeneralButton mRelative;

		private void ButtonPress_Hold	    (object sender, EventArgs e) => HoldClicked?.Invoke (sender, e);
		private void ButtonPress_Relative   (object sender, EventArgs e) => RelClicked?.Invoke  (sender, e);
		private void PickerChange_Range	    (object sender, EventArgs e) => RangeChanged?.Invoke(sender, e);
		private void PickerChange_Mode	    (object sender, EventArgs e) => ModeChanged?.Invoke (sender, e);

		public MultimeterMenu()
		{
            //##############################################################
            mHold       = new GeneralButton("Hold",     ButtonPress_Hold);
			mRelative   = new GeneralButton("Rel",      ButtonPress_Relative);
			mRange      = new GeneralButton("Range",    PickerChange_Range);
			mMode       = new GeneralButton("Mode",     PickerChange_Mode);
            //##############################################################

            DefineGrid(4, 1);
			AutoAdd(mHold);
			AutoAdd(mMode);
			AutoAdd(mRelative);
			AutoAdd(mRange);
		}
	}
}
