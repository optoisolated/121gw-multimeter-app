using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
#if __ANDROID__
using Android;
using Android.App;
#endif

namespace App_121GW
{
    static class Files
	{
		public static string UniqueFilename()
		{
			string output = "";
			while (File.Exists(output = Path.GetRandomFileName())) ;
			return output;
		}

		public static void SaveFile(string content)
		{
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filename = Path.Combine(documents, "LogFile.csv");
            File.WriteAllText(filename, content);
        }
		public static string LoadFile(string filename)
		{
            return "";
		}
	}
}
