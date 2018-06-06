using SkiaSharp;
using System;
using System.Threading;

namespace App_121GW
{
	public abstract class ASmartAxisPair : ASmartElement
	{
		public ASmartData Parent;
		public ASmartAxis Horizontal, Vertical;
		public abstract SKMatrix Transform(SKSize dimension);
		public SKRect AxisClip(SKSize dimension)
		{
			return new SKRect(Horizontal.AxisStart(dimension.Width), Vertical.AxisStart(dimension.Height), Horizontal.AxisEnd(dimension.Width), Vertical.AxisEnd(dimension.Height));
		}

		public bool EnableTouchVertical = false;
		public bool EnableTouchHorizontal = true;

        Mutex mutex = new Mutex();
        public void Wait() => mutex.WaitOne();
        public void Release() => mutex.ReleaseMutex();
        public void Reset()
        {
            Wait();
            Horizontal.Range.Reset();
			Vertical.Range.Reset();
            Release();
        }

        public void Zoom(float dx, float dy, float cx, float cy)
		{
            Wait();
            if (EnableTouchHorizontal)  Horizontal.Zoom(dx, cx);
			if (EnableTouchVertical)    Vertical.Zoom(dy, cy);

			Parent.Parent.InvalidateSurface();
            Release();
        }
		public void Zoom(SKPoint Amount, SKPoint About) => Zoom(Amount.X, Amount.Y, About.X, About.Y);
		public void Pan(float dx, float dy)
        {
            Wait();
            if (EnableTouchHorizontal)  Horizontal.Pan(dx);
			if (EnableTouchVertical)    Vertical.Pan(dy);

			Parent.Parent.InvalidateSurface();
            Release();
		}
		public void Pan(SKPoint Amount) => Pan(Amount.X, Amount.Y);

        public void Set(SKRect Boundary)
		{
			int ticks	=	(int)Vertical.MajorTicks;

			var top_si	=	new SIValue(Boundary.Bottom);
			var bot_si	=	new SIValue(Boundary.Top);

			float top	=	top_si.SI_Ceiling;
			float bot	=	bot_si.SI_Floor;
			
			while (true)
			{
				{
					var dist = (int)(top - bot);
					if ((dist % ticks) == 0) break;
					else top += 1;
				}
				{
					var dist = (int)(top - bot);
					if ((dist % ticks) == 0) break;
					else bot -= 1;
				}
			}

			top /= (float)top_si.Multiply;
			bot /= (float)bot_si.Multiply;

            Horizontal.Range.SetBoundary(Boundary.Left, Boundary.Right);
			Vertical.Range.SetBoundary(top, bot);
		}

		public abstract void Draw(SKCanvas canvas, SKSize dimension, SKSize view);
	}

	public class SmartAxisPair : ASmartAxisPair
	{
		public SmartAxisPair(ASmartAxis pHorizontal, ASmartAxis pVertical)
		{
			Horizontal = pHorizontal;
			Vertical = pVertical;
		}
		public override SKMatrix Transform(SKSize dimension)
		{
			var horz_map	= Horizontal.CoordinateFromValue(dimension.Width);
			var vert_map	= Vertical.CoordinateFromValue(dimension.Height);
			return Map.CreateMatrix(horz_map, vert_map);
		}
		public override void Draw(SKCanvas canvas, SKSize dimension, SKSize view)
		{
            Wait();

            Horizontal.Position = Padding.BottomPosition(dimension.Height);
			Vertical.Position   = Padding.LeftPosition  (dimension.Width);
			Horizontal.Draw(canvas, dimension, view);
			Vertical.Draw(canvas, dimension, view);

            Release();
        }
	}
}