﻿using System;
using System.Linq;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ResolutionGroupName("rMultiplatform")]
[assembly: ExportEffect(typeof(rMultiplatform.UWP.Touch), "Touch")]

namespace rMultiplatform.UWP
{
    class Touch : PlatformEffect
    {
        FrameworkElement        view;
        rMultiplatform.Touch    effect;

        //Required by Platform Effect
        protected override void OnAttached()
        {
            // Get the Windows FrameworkElement corresponding to the Element that the effect is attached to
            view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the PCL
            effect = (rMultiplatform.Touch)Element.Effects.FirstOrDefault(e => e is rMultiplatform.Touch);

            if (effect != null && view != null)
            {
                // Set event handlers on FrameworkElement
                view.PointerEntered += MoveHandler;
                view.PointerMoved += MoveHandler;
                view.PointerPressed += PressedHandler;
                view.PointerReleased += ReleasedHandler;
                view.PointerExited += ReleasedHandler;
                view.PointerCanceled += ReleasedHandler;
            }
        }
        protected override void OnDetached()
        {
            // Release event handlers on FrameworkElement
            view.PointerEntered -= MoveHandler;
            view.PointerMoved -= MoveHandler;
            view.PointerPressed -= PressedHandler;
            view.PointerReleased -= ReleasedHandler;
            view.PointerExited -= ReleasedHandler;
            view.PointerCanceled -= ReleasedHandler;
        }

        //Shared handler functions
        private Point GetPoint(object sender, PointerRoutedEventArgs args)
        {
            var pp = args.GetCurrentPoint(sender as UIElement).Position;
            return new Point(pp.X, pp.Y);
        }

        // Common handlers
        void ReleasedHandler(object sender, PointerRoutedEventArgs args)
        {
            effect.ReleasedHandler(sender, GetPoint(sender, args));
        }
        void MoveHandler    (object sender, PointerRoutedEventArgs args)
        {
            effect.MoveHandler(sender, GetPoint(sender, args));
        }
        void PressedHandler (object sender, PointerRoutedEventArgs args)
        {
            effect.PressedHandler(sender, GetPoint(sender, args));
        }
    }
}
