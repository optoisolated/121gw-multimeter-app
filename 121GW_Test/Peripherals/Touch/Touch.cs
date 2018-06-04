using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("App_121GW")]
[assembly: ExportEffect(typeof(App_121GW.iOS.Touch), "Touch")]

namespace App_121GW.iOS
{
    class Touch : PlatformEffect
    {
        UIView mView;
        TouchRecognizer touchRecognizer;

        protected override void OnAttached()
        {
            mView = Control ?? Container;
            touchRecognizer = new TouchRecognizer(Element, mView);
            mView.AddGestureRecognizer(touchRecognizer);
        }

        protected override void OnDetached()
        {
            if (touchRecognizer != null)
                mView.RemoveGestureRecognizer(touchRecognizer);
        }
    }
}