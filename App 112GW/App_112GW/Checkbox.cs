﻿using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;
namespace App_112GW
{
    class CheckboxRenderer:
#if __ANDROID__
        SKGLView
#elif __IOS__
        SKGLView
#else
        SKCanvasView
#endif
    {
        public bool                     Checked;
        public int                      CornerRadius;
        public Color                    BorderColor;
        public Color                    TextColor;

        private SKPaint                 mPaintStyle;
        private SKPaint                 mFillStyle;
        private TapGestureRecognizer    mTapRecogniser;

        public CheckboxRenderer()
        {
            HorizontalOptions = LayoutOptions.End;
            VerticalOptions = LayoutOptions.Fill;
                        
            //Setup responses to gestures
            mTapRecogniser = new TapGestureRecognizer();
            mTapRecogniser.Tapped += TapCallback;
            GestureRecognizers.Add(mTapRecogniser);
           
            //Setup defaults
            Checked = false;
            TextColor = Globals.ColorText;
            BackgroundColor = Globals.BackgroundColor;
            CornerRadius = 0;

            mPaintStyle = new SKPaint()
            {
                Color = TextColor.ToSKColor(),
                Style = SKPaintStyle.Stroke
            };
            mFillStyle = new SKPaint()
            {
                Color = BackgroundColor.ToSKColor(),
                Style = SKPaintStyle.Fill
            };
        }

        private void TapCallback(object sender, EventArgs args)
        {
            Checked = !Checked;
            InvalidateSurface();
        }

#if __ANDROID__
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#elif __IOS__
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#else
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
#endif
        {
            
            using (var can = e.Surface.Canvas)
            {
                SKRect temp = can.ClipBounds;
                WidthRequest = temp.Size.Height;
                HorizontalOptions = LayoutOptions.End;

                mPaintStyle.StrokeWidth = (float)WidthRequest / 16.0f;

                temp.Left += mPaintStyle.StrokeWidth;
                temp.Top += mPaintStyle.StrokeWidth;
                temp.Right -= mPaintStyle.StrokeWidth;
                temp.Bottom -= mPaintStyle.StrokeWidth;

                can.DrawRect(temp, mFillStyle);
                can.DrawRect(temp, mPaintStyle);

                var Pah = new SKPath();
                SKPoint[] Pts = new SKPoint[]
                    {
                        new SKPoint((float)(Width * 330/1332), (float)(Height * 600/1332)),
                        new SKPoint((float)(Width * 600/1332), (float)(Height * 863/1332)),
                        new SKPoint((float)(Width * 1070/1332), (float)(Height * 390/1332))
                    };

                Pah.AddPoly(Pts, false);
                if (Checked)
                {
                    can.DrawPath(Pah, mPaintStyle);
                }
            }
        }
    }

    class Checkbox : Grid
    {
        private CheckboxRenderer    mRenderer;
        private Label               mLabel;

        private void AddView(View pInput, int pX, int pY, int pXSpan = 1, int pYSpan = 1)
        {
            Children.Add(pInput);
            SetColumn(pInput, pX);
            SetRow(pInput, pY);

            SetColumnSpan(pInput, pXSpan);
            SetRowSpan(pInput, pYSpan);
        }

        public Checkbox(string pLabel)
        {
            mLabel = new Label(){
                Text = pLabel,
                Style = Globals.LabelStyle,
                HorizontalTextAlignment = TextAlignment.End,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Fill
            };

            mRenderer = new CheckboxRenderer();

            //
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            AddView(mLabel, 0, 0);
            AddView(mRenderer, 1, 0);
        }
    }
}
