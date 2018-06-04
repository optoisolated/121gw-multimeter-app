using Xamarin.Forms;
using System.Threading.Tasks;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(App_121GW.iOS.Files))]
namespace App_121GW.iOS
{
    class Files : IFiles
    {
        UIViewController GetVisibleViewController()
        {
            var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            
            if (rootController.PresentedViewController == null)
                return rootController;
            if (rootController.PresentedViewController is UINavigationController)
                return ((UINavigationController)rootController.PresentedViewController).TopViewController;
            if (rootController.PresentedViewController is UITabBarController)
                return ((UITabBarController)rootController.PresentedViewController).SelectedViewController;

            return rootController.PresentedViewController;
        }

        public async Task<bool> Save(string pContent)
        {
            var item = new NSObject[] { NSObject.FromObject(pContent) };
            var activityController = new UIActivityViewController(item, null);
            var vc = GetVisibleViewController();
            await vc.PresentViewControllerAsync(activityController, true);
            return await Task.FromResult<bool>(true);
        }
    }
}
