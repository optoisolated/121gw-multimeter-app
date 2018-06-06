using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace App_121GW
{
	public class SVGPath : ILayer
	{
		private SKPath mPath = null;
		private SKPaint	mDrawPaint;
		private SKPaint	mUndrawPaint;

		private bool mActive = false;
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
	
		public SVGPath(string pSvgData, string pName, bool pActive = true)
		{
			mPath = SKPath.ParseSvgPathData(pSvgData);
			if (mPath == null) throw new Exception("Could not create path from SVG.");

			mChanged = new VariableMonitor<bool>();
			mRenderChanged = new VariableMonitor<bool>();

			Name	= pName;
			mActive	= pActive;

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

		public string Name { get; private set; }

		public int Width		=> (int)mPath.Bounds.Width;
		public int Height		=> (int)mPath.Bounds.Height;

		public void Set(bool pState)
		{
			bool temp = mActive;
			mActive = pState;
			mChanged.Update(ref mActive);
		}
		public void On()
		{
			Set(true);
		}
		public void Off()
		{
			Set(false);
		}
		public void Redraw()
		{
			mChanged.UpdateOverride = true;
			mRenderChanged.UpdateOverride = true;
		}

		public void Render(ref SKCanvas pSurface, SKRect pDestination)
		{
			if (mRenderChanged.Update(ref mActive))
			{
				if (mActive)	pSurface.DrawPath(mPath, mDrawPaint);
				else			pSurface.DrawPath(mPath, mUndrawPaint);
			}
		}
	}
}
