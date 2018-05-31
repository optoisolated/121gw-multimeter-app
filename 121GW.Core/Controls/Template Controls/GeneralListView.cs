using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin;

namespace App_121GW
{
	public class GeneralListView : ListView
	{
		public GeneralListView()
		{
			BackgroundColor	 = Globals.BackgroundColor;
			HorizontalOptions   = LayoutOptions.Fill;
			VerticalOptions	 = LayoutOptions.Fill;
		}
	}
}
