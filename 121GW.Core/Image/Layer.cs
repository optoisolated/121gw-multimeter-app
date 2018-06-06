﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_121GW
{
	public interface ILayer
	{
		SKColor BackgroundColor{ get; set; }
		SKColor DrawColor{ get; set; }
		int Width { get; }
		int Height { get; }
		string Name { get; }

		event EventHandler OnChanged;

		void Set(bool pState);
		void Off();
		void On();
		void Redraw();

		string ToString();
		void Render(ref SKCanvas pSurface, SKRect pDestination);
	}
	public class LayerCompare : Comparer<ILayer>
	{
		// Compares by Length, Height, and Width.
		public override int Compare(ILayer x, ILayer y)
		{
			return x.Name.CompareTo(y.Name);
		}
	}
}