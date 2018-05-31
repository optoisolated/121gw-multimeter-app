using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_121GW
{
	public class GeneralLabel : Label
	{
		public GeneralLabel()
		{
			HorizontalOptions = LayoutOptions.CenterAndExpand;
			VerticalOptions = LayoutOptions.CenterAndExpand;
			TextColor = Globals.TextColor;
			FontSize = Globals.MinorFontSize;
		}
	}
}
