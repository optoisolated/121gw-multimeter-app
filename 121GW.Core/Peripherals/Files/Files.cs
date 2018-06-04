using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace App_121GW
{
    public interface IFiles
    {
        Task<bool> Save(string pContent);
    }

    public class Files
    {
        public static async Task<bool> Save(string pContent)
        {
            return await DependencyService.Get<IFiles>().Save(pContent);
        }
    }
}
