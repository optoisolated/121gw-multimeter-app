﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_112GW
{
	public partial class MainPage : ContentPage
	{
		SKSize dimen;
		float aspect;
		float padding = 20;
		Random randy = new Random();
		const string MultimeterLayer = "./Layers";

		private static Color ColorText		= Color.FromRgb(0xAA, 0xAA, 0xAA);
		private static Color BorderColor	= Color.FromRgb(0x33, 0x33, 0x33);
		private static double FontSize		= Device.GetNamedSize(NamedSize.Medium, typeof(Button));
		private static Style ButtonStyle	= new Style(typeof(Button))
		{
			Setters =
			{
				new Setter{ Property = Button.TextColorProperty, Value = ColorText},
				new Setter{ Property = Button.BorderColorProperty, Value = BorderColor},
				new Setter{ Property = Button.FontSizeProperty, Value = FontSize}
			}
		};
		private Button		ButtonAddDevice		= new Button { Text = "Add Device", Style = ButtonStyle};
		private Button		ButtonStartLogging	= new Button { Text = "Start Logging", Style = ButtonStyle };
		private Grid		UserGrid			= new Grid {HorizontalOptions=LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, RowSpacing = 1, ColumnSpacing = 1, Padding = 1};

		void InitSurface()
		{
			UserGrid.BackgroundColor = BackgroundColor;
			UserGrid.RowDefinitions.Add(	new RowDefinition		{ Height	= new GridLength(1, GridUnitType.Star)		});
			UserGrid.RowDefinitions.Add(	new RowDefinition		{ Height	= new GridLength(50, GridUnitType.Absolute)	});
			UserGrid.ColumnDefinitions.Add(	new ColumnDefinition	{ Width		= new GridLength(1, GridUnitType.Star)		});
			UserGrid.ColumnDefinitions.Add(	new ColumnDefinition	{ Width		= new GridLength(1, GridUnitType.Star)		});

            TimeSpan DefTim = new TimeSpan(0, 0, 3);
            MultimeterScreen Temp = new MultimeterScreen(MultimeterLayer, DefTim);

            UserGrid.Children.Add	(Temp);
			Grid.SetRow				(Temp, 0);
			Grid.SetColumn			(Temp, 0);
			Grid.SetRowSpan			(Temp, 1);
			Grid.SetColumnSpan		(Temp, 2);

			UserGrid.Children.Add	(ButtonAddDevice,		0, 1);
			UserGrid.Children.Add	(ButtonStartLogging,	1, 1);
			Grid.SetColumnSpan		(ButtonAddDevice,		1);
			Grid.SetColumnSpan		(ButtonStartLogging,	1);
			
			ButtonAddDevice.Clicked		+= AddDevice;
			ButtonStartLogging.Clicked	+= StartLogging;
			
			Content = UserGrid;
		}

		public MainPage ()
		{
			InitializeComponent();
			BackgroundColor = Color.FromRgb(38, 38, 38);

			InitSurface();
		}

		void AddDevice (object sender, EventArgs args)
		{
            //Devices.Add(new MultimeterScreen(MultimeterLayer));
            //Devices.Last().SetLargeSegments("0.000");
            //Devices.Last().SetBargraph(0);

            //Calculate ratios
            //dimen = Devices.Last().Dimensions().ToSKSize();
            //aspect = dimen.Height / dimen.Width;
        }
        void StartLogging (object sender, EventArgs args)
		{
			
		}
	}
}
