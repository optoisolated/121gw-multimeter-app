using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Android;
using Android.App;

namespace rMultiplatform
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
#if __ANDROID__
            //Email or cloud
            //var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //var filePath = Path.Combine(documentsPath, "121GW Log " + System.DateTime.Now.ToShortTimeString() + " " + System.DateTime.Now.ToShortDateString() + ".csv");
            //System.IO.File.WriteAllText(filePath, content);

            Globals.RunMainThread(() =>
            {
                var intent = new Android.Content.Intent(Android.Content.Intent.ActionSend);
                intent.SetType("plain/text");
                intent.SetFlags(Android.Content.ActivityFlags.NewTask);
                intent.PutExtra(Android.Content.Intent.ExtraSubject, "Logging");
                intent.PutExtra(Android.Content.Intent.ExtraText, content);
                Android.App.Application.Context.StartActivity(intent);
            });
#elif __IOS__
			//Email or cloud

#else
			//Anywhere
			var documentpicker = new Windows.Storage.Pickers.FileSavePicker();
			documentpicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
			documentpicker.FileTypeChoices.Add("Comma seperated files.", new List<string>() { ".csv" });
			documentpicker.SuggestedFileName = "Logfile";
			documentpicker.PickSaveFileAsync().AsTask().ContinueWith((Task<Windows.Storage.StorageFile> resutl)=> 
			{
				var file = resutl.Result;
                if (file != null)
				    Windows.Storage.FileIO.WriteTextAsync(file, content);
			});
#endif
        }
		public static string LoadFile(string filename)
		{
#if __ANDROID__
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#elif __IOS__
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#else
			var documentpicker = new Windows.Storage.Pickers.FileOpenPicker();
			documentpicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
			documentpicker.FileTypeFilter.Add(".txt"); 
			documentpicker.FileTypeFilter.Add(".csv");

			var file = documentpicker.PickSingleFileAsync().AsTask().Result;
			if (file != null)
			{
				var content = Windows.Storage.FileIO.ReadTextAsync(file).AsTask().Result;
				return content;
			}
#endif
			return "";
		}
	}
}
