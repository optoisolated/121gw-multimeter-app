using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;

using CoreGraphics;
using Foundation;
using UIKit;
namespace App_121GW.iOS
{
    class TouchRecognizer : UIGestureRecognizer
    {
        private readonly Element element; // Forms element for firing events
        private readonly UIView  view;    // iOS UIView 
        private App_121GW.Touch effect;
        
        public TouchRecognizer(Element element, UIView view)
        {
            this.element = element;
            this.view    = view;
            this.effect = (App_121GW.Touch)element.Effects.FirstOrDefault(e => e is App_121GW.Touch);
        }

        Point GetPoint(UITouch pInput)
        {
            CGPoint cgPoint = pInput.LocationInView(View);
            return new Point(cgPoint.X, cgPoint.Y);
        }
        static uint GetId(UITouch pInput)
        {
            return (uint)pInput.Handle.ToInt64();
        }

        // touches = touches of interest; evt = all touches of type UITouch
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            foreach (var touch in touches.Cast<UITouch>())
                effect.PressedHandler(element, GetPoint(touch), GetId(touch));
        }
        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            foreach (var touch in touches.Cast<UITouch>())
                effect.MoveHandler(element, GetPoint(touch), GetId(touch));
        }
        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            foreach (var touch in touches.Cast<UITouch>())
                effect.ReleasedHandler(element, GetPoint(touch), GetId(touch));
        }
        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            foreach (var touch in touches.Cast<UITouch>())
                effect.ReleasedHandler(element, GetPoint(touch), GetId(touch));
        }
    }
}