﻿using System;
using SkiaSharp;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace App_121GW
{
	public class PathLayer : ILayer
	{
		public SKColor BackgroundColor
		{
			get
			{
				return mUndrawPaint.Color;
			}
			set
			{
				mUndrawPaint.Color = value;
			}
		}
		public SKColor DrawColor
		{
			get
			{
				return mDrawPaint.Color;
			}
			set
			{
				mDrawPaint.Color = value;
			}
		}

		public string Name
		{
			get;
			private set;
		}
		public Polycurve mImage;
		private SKPaint mDrawPaint;
		private SKPaint mUndrawPaint;

		private bool mActive;
		private VariableMonitor<bool> mRenderChanged;
		private VariableMonitor<bool> mChanged;
		public event EventHandler OnChanged
		{
			add
			{
				mChanged.OnChanged += value;
			}
			remove
			{
				mChanged.OnChanged -= value;
			}
		}

		public PathLayer(Polycurve pImage, string pName, bool pActive = true)
		{
			mChanged = new VariableMonitor<bool>();
			mRenderChanged = new VariableMonitor<bool>();

			//Open the defined image
			mActive = pActive;
			mImage = pImage;
			Name = pName;

			//
			var transparency = Color.FromRgba(0, 0, 0, 0).ToSKColor();

			mDrawPaint = new SKPaint
			{
				BlendMode = SKBlendMode.Src,
				Color = Globals.TextColor.ToSKColor(),
				ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.Dst)
			};
			mUndrawPaint = new SKPaint
			{
				BlendMode = SKBlendMode.Src,
				Color = Globals.BackgroundColor.ToSKColor(),
				ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.Dst)
			};

			Off();
		}

		public void Set(bool pState)
		{
			bool temp = mActive;
			mActive = pState;
			mChanged.Update(ref mActive);
		}
		public void On() => Set(true);
		public void	Off() => Set(false);
		public void	Redraw()
		{
			mChanged.UpdateOverride = true;
			mRenderChanged.UpdateOverride = true;
		}

		public override string ToString() => Name;
		public int Width => (int)mImage.Width;
		public int Height => (int)mImage.Height;

		public void Render (ref SKCanvas pSurface, SKRect pDestination)
		{
			//This is render changed variable, don't move it to set, that is wrong
			if (mRenderChanged.Update(ref mActive))
			{
				var isize   = mImage.CanvasSize;

				var xscale  = pDestination.Width / isize.Width;
				var yscale  = pDestination.Height / isize.Height;

				var transform = SKMatrix.MakeIdentity();
				transform.SetScaleTranslate(xscale, yscale, pDestination.Left, pDestination.Top);

				//If the item is inside the another path then it should undraw it whendraw operation takes place.
				if (mActive)	mImage.Draw (ref pSurface, transform, ref mDrawPaint, ref mUndrawPaint);
				else			mImage.Draw (ref pSurface, transform, ref mUndrawPaint, ref mUndrawPaint);
			}
		}
	}
}