using Xamarin.Forms;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

[assembly: Xamarin.Forms.Dependency(typeof(App_121GW.UWP.Files))]
namespace App_121GW.UWP
{
    class Files : IFiles
    {
        DataTransferManager DataTransferManager = null;
        private string mContent = "";

        public Task<bool> Save(string pContent)
        {
            if (DataTransferManager == null)
            {
                mContent = pContent;
                DataTransferManager = DataTransferManager.GetForCurrentView();
                DataTransferManager.DataRequested += DataTransferManager_DataRequested;
                DataTransferManager.ShowShareUI();
                return Task.FromResult<bool>(true);
            }
            return Task.FromResult<bool>(false);
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName + " logfile.csv";
            DataRequestDeferral deferral = request.GetDeferral();

            try
            {
                request.Data.SetText(mContent);
            }
            finally
            {
                deferral.Complete();
                DataTransferManager = null;
            }
        }
    }
}
