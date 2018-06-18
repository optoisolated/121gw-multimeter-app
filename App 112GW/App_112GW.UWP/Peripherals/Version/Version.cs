using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Windows.ApplicationModel;

[assembly: Dependency(typeof(App_121GW.UWP.Version))]
namespace App_121GW.UWP
{
	public class Version : IVersion
	{
		public string Build() => Package.Current.Id.Version.Build.ToString();
		public string App() => Package.Current.Id.Version.Major.ToString() + "." + Package.Current.Id.Version.Minor.ToString();
	}
}