using UIKit;

namespace App_121GW.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			UIButton.Appearance.TintColor = UIColor.LightGray;
			UIButton.Appearance.SetTitleColor(UIColor.FromRGB(0, 127, 14), UIControlState.Normal);

			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}