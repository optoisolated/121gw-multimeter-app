using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;

using CoreGraphics;
using Foundation;
using UIKit;
using System.Diagnostics;

namespace App_121GW.iOS
{
    class TouchRecognizer : UIGestureRecognizer
    {
        private readonly Element mElement; // Forms element for firing events
        private readonly UIView  mView;    // iOS UIView 
        private App_121GW.Touch mEffect;

        public float ApplicationScale => (float)UIScreen.MainScreen.Scale;


        public TouchRecognizer(Element element, UIView view)
        {
            mElement = element;
            mView = view;
            mEffect = (App_121GW.Touch)element.Effects.FirstOrDefault(e => e is App_121GW.Touch);
        }

        Point GetPoint(UITouch pInput)
        {
            CGPoint cgPoint = pInput.LocationInView(mView);
            float scale = 1;
            if (pInput.View != null)
                scale = (float)pInput.View.ContentScaleFactor;

            return new Point(cgPoint.X * scale * ApplicationScale, cgPoint.Y * scale * ApplicationScale);
        }
        static uint GetId(UITouch pInput)
        {
            return (uint)pInput.Handle.ToInt64();
        }
        
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            foreach (var touch in touches.Cast<UITouch>())
                mEffect.PressedHandler(mElement, GetPoint(touch), GetId(touch));
        }
        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            foreach (var touch in touches.Cast<UITouch>())
                mEffect.MoveHandler(mElement, GetPoint(touch), GetId(touch));
        }
        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            foreach (var touch in touches.Cast<UITouch>())
                mEffect.ReleasedHandler(mElement, GetPoint(touch), GetId(touch));
        }
        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            foreach (var touch in touches.Cast<UITouch>())
                mEffect.ReleasedHandler(mElement, GetPoint(touch), GetId(touch));
        }
    }
}