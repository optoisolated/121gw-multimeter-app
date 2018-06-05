using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_121GW
{
    //These need to be defined in the .Net Standard definions, this is not automatic. YET!

    public class GeneralRenderer :
#if __OPENGL__
		SKGLView
#else
        SKCanvasView
#endif
    {
        public delegate void PaintCanvas(SKCanvas c, SKSize s, SKSize v);
        public event PaintCanvas Paint;
        public GeneralRenderer(PaintCanvas PaintEvent)
        {
            Paint += PaintEvent;
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Fill;
            BackgroundColor = Globals.BackgroundColor;
        }

#if __OPENGL__
		protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#else
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
#endif
        {
            if (e.Surface == null) return;
            if (e.Surface.Canvas == null) return;
            Paint?.Invoke(e.Surface.Canvas, CanvasSize, base.Bounds.Size.ToSKSize());
        }
    }
}