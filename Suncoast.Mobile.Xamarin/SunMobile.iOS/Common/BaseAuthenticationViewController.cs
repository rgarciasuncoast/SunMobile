using System;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;

namespace SunMobile.iOS.Common
{
	public class BaseAuthenticationViewController : BaseViewController
	{
		public BaseAuthenticationViewController(IntPtr handle) : base(handle)
		{
		}

		protected async void InitialViewMessage(string message)
		{
			if (!string.IsNullOrEmpty(message))
			{
				var label = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "EA7F09B2-3E63-4BE8-AA05-5594FDAE4FC8", "Login");
				await AlertMethods.Alert(View, label, message, "OK");
			}
		}
	}
}