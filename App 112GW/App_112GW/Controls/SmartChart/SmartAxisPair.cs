﻿using SkiaSharp;

namespace rMultiplatform
{
    public abstract class ASmartAxisPair : ASmartElement
    {
        public ASmartData Parent;
        public ASmartAxis Horizontal, Vertical;
        public abstract SKMatrix Transform(SKSize dimension);

        public void Reset()
        {
            Horizontal.Range.Reset();
            Vertical.Range.Reset();
        }
        public void Zoom(SKPoint Amount, SKPoint About)
        {
            Horizontal.Range.Zoom(Amount.X, About.X);
            Vertical.Range.Zoom(Amount.Y, About.Y);
        }
        public void Pan(SKPoint Amount)
        {
            Horizontal.Range.Pan(Amount.X);
            Vertical.Range.Pan(Amount.Y);
        }
        public void Set(SKRect Boundary)
        {
            Horizontal.Range.SetBoundary(Boundary.Left, Boundary.Right);
            Vertical.Range.SetBoundary(Boundary.Top, Boundary.Bottom);
        }

        public abstract void Draw(SKCanvas canvas, SKSize dimension);
    }

    public class SmartAxisPair : ASmartAxisPair
    {
        public SmartAxisPair(ASmartAxis pHorizontal, ASmartAxis pVertical)
        {
            Horizontal = pHorizontal;
            Vertical = pVertical;
        }

        //Takes a SKPath and transforms it based on the transformations present in the axis
        public override SKMatrix Transform(SKSize dimension)
        {
            var matrix      = SKMatrix.MakeIdentity();
            var horz_map    = Horizontal.CoordinateFromValue(dimension.Width);
            var vert_map    = Vertical.CoordinateFromValue(dimension.Height);
            matrix.ScaleX   = horz_map.Scale;
            matrix.TransX   = horz_map.Translation;
            matrix.ScaleY   = vert_map.Scale;
            matrix.TransY   = vert_map.Translation;
            return matrix;
        }

        public override void Draw(SKCanvas canvas, SKSize dimension)
        {
            Horizontal.Position = Padding.BottomPosition(dimension.Height);
            Vertical.Position   = Padding.LeftPosition  (dimension.Width);

            Horizontal.Draw(canvas, dimension);
            Vertical.Draw(canvas, dimension);
        }
    }
}