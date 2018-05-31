using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace App_121GW
{
    static class SmartDPI
    {
        public static (float, float) GetScale(SKCanvas canvas, SKSize dimension, SKSize view)
        {
            return (dimension.Width / view.Width, dimension.Height / view.Height);
        }
    };
}