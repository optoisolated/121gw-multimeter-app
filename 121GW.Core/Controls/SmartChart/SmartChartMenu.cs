using App_121GW;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace App_121GW
{
	public class SmartChartMenu : AutoGrid
	{
		private readonly GeneralButton mSave;
		private readonly GeneralButton mReset;
		public event EventHandler SaveClicked;
		public event EventHandler ResetClicked;

		private void ButtonPress_Save(object sender, EventArgs e) => SaveClicked?.Invoke(sender, e);
		private void ButtonPress_Reset(object sender, EventArgs e) => ResetClicked?.Invoke(sender, e);

		public SmartChartMenu(bool ShowSave = true, bool ShowReset = true)
		{
			//Add Relative Button
			mReset  = new GeneralButton("Reset",    ButtonPress_Reset);
			mSave   = new GeneralButton("Save",     ButtonPress_Save);

			//Define Grid
			DefineGrid(2, 1);
			if (ShowReset)  AutoAdd(mReset);
			if (ShowSave)   AutoAdd(mSave);
		}
	}
}
