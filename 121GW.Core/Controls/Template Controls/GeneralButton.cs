using System;
using Xamarin.Forms;

namespace App_121GW
{
	public class GeneralButton : Button
	{
		public GeneralButton(string pText, EventHandler pEvent)
		{
			Text = pText;
			Clicked += pEvent;
			Margin = 0;
			base.FontSize = Globals.MajorFontSize;
		}
	}
}
