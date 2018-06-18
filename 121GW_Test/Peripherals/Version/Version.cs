using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(App_121GW.iOS.Version))]
namespace App_121GW.iOS
{
	public class Version : IVersion
	{
		public string Build() => NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
		public string App() => NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
	}
}
