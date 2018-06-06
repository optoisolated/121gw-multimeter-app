using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;



namespace App_121GW
{
	using SKSvg = SkiaSharp.Extended.Svg.SKSvg;
	public class SVGLayer : ILayer
	{
		private bool mActive;
		public SKSvg mImage;
		private VariableMonitor<bool> mRenderChanged;
		private VariableMonitor<bool> mChanged;
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

		public string Name
		{
			get;
			private set;
		}

		public SVGLayer(SKSvg pImage, string pName, bool pActive = true)
		{
			mChanged = new VariableMonitor<bool>();
			mRenderChanged = new VariableMonitor<bool>();

			//Open the defined image
			mActive = pActive;
			mImage = pImage;
			Name = pName;
			
			var transparency = Color.FromRgba(0, 0, 0, 0).ToSKColor();
			mDrawPaint = new SKPaint
			{
				Color = SKColors.Red,
				IsAntialias = true,
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

		public int Width => (int)mImage.CanvasSize.Width;
		public int Height => (int)mImage.CanvasSize.Height;
		public void	 Render(ref SKCanvas pSurface, SKRect pDestination)
		{
			//This is render changed variable, don't move it to set, that is wrong
			if (mRenderChanged.Update(ref mActive))
			{
				var isize = mImage.CanvasSize;
				var xscale = pDestination.Width / isize.Width;
				var yscale = pDestination.Height / isize.Height;
				var transform = SKMatrix.MakeIdentity();
				transform.SetScaleTranslate(xscale, yscale, pDestination.Left, pDestination.Top);

				if (mActive)
					pSurface.DrawPicture(mImage.Picture, ref transform, mDrawPaint);
				else
					pSurface.DrawPicture(mImage.Picture, ref transform, mUndrawPaint);
			}
		}
	}
}