using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Android;
using Android.App;
using Android.Content;

[assembly: Xamarin.Forms.Dependency(typeof(App_121GW.Droid.Files))]
namespace App_121GW.Droid
{
    class Files : IFiles
    {
        public async Task<bool> Save(string pContent)
        {
            var intent = new Intent(Intent.ActionSend);
            intent.SetType("text/plain");
            intent.PutExtra(Intent.ExtraText, pContent ?? string.Empty);
            var chooserIntent = Intent.CreateChooser(intent, "Save your logged data." ?? string.Empty);
            chooserIntent.SetFlags(ActivityFlags.ClearTop);
            chooserIntent.SetFlags(ActivityFlags.NewTask);
            global::Android.App.Application.Context.StartActivity(chooserIntent);
            return await Task.FromResult(true);
        }
    }
}
