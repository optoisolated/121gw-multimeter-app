using Android.Content.PM;
using Xamarin.Forms;

[assembly: Dependency(typeof(App_121GW.Droid.Version))]
namespace App_121GW.Droid
{
	public class Version : IVersion
	{
		public string App()
		{
			var context = global::Android.App.Application.Context;
			PackageManager manager = context.PackageManager;
			PackageInfo info = manager.GetPackageInfo(context.PackageName, 0);
			return info.VersionName;
		}

		public string Build()
		{
			var context = global::Android.App.Application.Context;
			PackageManager manager = context.PackageManager;
			PackageInfo info = manager.GetPackageInfo(context.PackageName, 0);
			return info.VersionCode.ToString();
		}
	}
}