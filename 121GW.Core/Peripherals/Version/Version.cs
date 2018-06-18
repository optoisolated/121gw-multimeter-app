using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace App_121GW
{
    public interface IVersion
    {
		string Build();
		string App();
	}
	public static class Version
	{
		public static string Build() => DependencyService.Get<IVersion>().Build();
		public static string App() => DependencyService.Get<IVersion>().App();
	}
}
