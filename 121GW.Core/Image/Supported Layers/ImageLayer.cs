using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_121GW
{
	public class ImageLayer : ILayer
	{
		SKPaint mDrawPaint;
		SKPaint mUndrawPaint;
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

		private bool mActive;
		private VariableMonitor<bool> mChanged;
		private VariableMonitor<bool> mRenderChanged;
		public event EventHandler OnChanged { add => mChanged.OnChanged += value; remove => mChanged.OnChanged -= value; }

		public SKImage  mImage;
		public string Name { get; private set; }

		public ImageLayer(SKImage pImage, string pName, bool pActive = true)
		{
			mChanged = new VariableMonitor<bool>();
			mRenderChanged = new VariableMonitor<bool>();

			//Open the defined image
			mActive = pActive;
			mImage = pImage;
			Name = pName;

			var transparency			= Color.FromRgba(0, 0, 0, 0).ToSKColor();
			mDrawPaint = new SKPaint
			{
				BlendMode = SKBlendMode.SrcOver,
				ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver)
			};
			mUndrawPaint = new SKPaint
			{
				BlendMode = SKBlendMode.DstOut,
				ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver)
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
		public void Off() => Set(false);
		public void Redraw() => mRenderChanged.UpdateOverride = true;

		public override string ToString() => Name;

		public int Width => mImage.Width;
		public int Height => mImage.Height;

		public void Render(ref SKCanvas pSurface, SKRect pDestination)
		{
			//This is render changed variable, don't move it to set, that is wrong
			if (mRenderChanged.Update(ref mActive))
			{
				if (mActive)
					pSurface.DrawImage(mImage, pDestination, mDrawPaint);
				else
					pSurface.DrawImage(mImage, pDestination, mUndrawPaint);
			}
		}
	}
}