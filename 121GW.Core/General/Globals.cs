﻿using System;
using SkiaSharp;
using Xamarin.Forms;
using System.Diagnostics;

namespace App_121GW
{
    public static class Globals
	{
		public static float Clip(float pInput, float pMinimum, float pMaximum)
		{
			if (float.IsInfinity(pInput))	return 1.0f;
			if (float.IsNaN(pInput))		return 1.0f;

			if (pInput > pMaximum) return pMaximum;
			if (pInput < pMinimum) return pMinimum;

			return pInput;
		}

		private static Random random = new Random();
		public static void RunMainThread(Action input)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				try
				{
					input?.Invoke();
				}
				catch (Exception e)
				{
					Debug.WriteLine("Error Caught :  public void RunMainThread(Action input)\n\n\n");
                    Debug.WriteLine(e);
                }
			});
		}

		public static void Alert(string pText)
		{
			
		}

		public static string StandardDateTime()
		{
			return DateTime.Now.ToString("yyyyMMdd_HHmmss");
		}

		public static SKTypeface Typeface
		{
			get
			{
				var output = SKTypeface.FromFamilyName("tahoma", 100, 1, SKFontStyleSlant.Upright);
				return output;
			}
		}
		public static float     TitleFontSize
		{
			get
			{
				return (float)Device.GetNamedSize(NamedSize.Large, typeof(Label)); ;
			}
		}
		public static float     MajorFontSize
		{
			get
			{
				return (float)Device.GetNamedSize(NamedSize.Default, typeof(Label)); ;
			}
		}
		public static float     MinorFontSize
		{
			get
			{
				return (float)Device.GetNamedSize(NamedSize.Small, typeof(Label)); ;
			}
		}

		private static T        GetResource<T>(string name)
		{
			return (T)Application.Current.Resources[name];
		}
		public static double    RandomBetween(double min, double max)
		{
			var output = random.NextDouble() * (max - min) + min;
			return output;
		}
		public static float     RandomBetween(float min, float max) => (float)RandomBetween((double)min, (double)max);
		public static int BorderWidth { get; } = 3;

		private static double   Brightness(Color A)
		{
			return A.Luminosity;
		}
		public static float     Contrast(Color A, Color B)
		{
			return (float)(Math.Abs(Brightness(A) - Brightness(B)));
		}

        public static Color     HighlightColor
		{
			get
			{
				switch (Device.RuntimePlatform)
				{
					case Device.iOS:
						return Color.FromHex("#FFFFFF");
					case Device.Android:
						return Color.FromHex("#FFFFFF");
					case Device.UWP:
						return Color.FromHex("#000000");
				}
				return Color.Black;
			}
		}
		public static Color     TextColor
		{
			get
			{
				switch (Device.RuntimePlatform)
				{
					case Device.iOS:
						return Color.FromHex("#000000");
					case Device.Android:
						return Color.FromHex("#000000");
					case Device.UWP:
						return Color.FromHex("#000000");
				}
				return Color.Black;
			}
		}
		public static Color     FocusColor
		{
			get
			{
				switch (Device.RuntimePlatform)
				{
					case Device.iOS:
						return Color.FromHex("#7E827A");
					case Device.Android:
						return Color.FromHex("#7E827A");
					case Device.UWP:
						return Color.FromHex("#7E827A");
				}
				return Color.Black;
			}
		}
		public static Color     BackgroundColor
		{
			get
			{
				switch (Device.RuntimePlatform)
				{
					case Device.iOS:
						return Color.FromHex("#FFFFFF");
					case Device.Android:
						return Color.FromHex("#FFFFFF");
					case Device.UWP:
						return Color.FromHex("#FFFFFF");
				}
				return Color.Black;
			}
		}
		public static Color     BorderColor
		{
			get
			{
				switch (Device.RuntimePlatform)
				{
					case Device.iOS:
						return Color.FromHex("#292F33");
					case Device.Android:
						return Color.FromHex("#292F33");
					case Device.UWP:
						return Color.FromHex("#292F33");
				}
				return Color.Black;
			}
		}
		public static double Padding { get; set; } = 3.0f;

		public static Color     UniqueColor(Range ContrastRange)
		{
			int maxtest = 0;
			while (maxtest++ < 10000)
			{
				var r = RandomBetween(0.0, 1.0);
				var g = RandomBetween(0.0, 1.0);
				var b = RandomBetween(0.0, 1.0);
				var color = new Color(r, g, b);

				if (ContrastRange.InRange(Contrast(color, BackgroundColor)))
					return color;
			}

			return Color.DodgerBlue;
		}
		public static Color     UniqueColor()
		{
			return UniqueColor(new Range(0.6f, 0.9f));
		}
	};
}
