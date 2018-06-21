using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Diagnostics;
using Xamarin.Forms.Internals;
using System.Threading.Tasks;
using UIKit;
using TabbedPageDemo.iOS;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms.Internals;
using static Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page;

//[assembly: ExportRenderer(typeof(TabbedPage), typeof(ExtendedTabbedPageRenderer))]
//namespace TabbedPageDemo.iOS
//{
//	public class ExtendedTabbedPageRenderer : TabbedRenderer
//	{
//		protected override async Task<Tuple<UIImage, UIImage>> GetIcon(Page page)
//		{
//			if (!string.IsNullOrEmpty(page.Icon?.File))
//			{
//				var source = global::Xamarin.Forms.Internals.Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(page.Icon);
//				var icon = await source.LoadImageAsync(page.Icon);
//				return Tuple.Create(icon, (UIImage)null);
//			}
//			return null;
//		}
//	};
//}