using System;
using Foundation;
using SunMobile.iOS.Common;
using SunMobile.Shared.Data;
using SunMobile.Shared.Methods;

namespace SunMobile.iOS.LoanCenter
{
	partial class LoanCenterViewController : BaseViewController
	{
        #region Required Parameters
        public LoanCenterTypes LoanType { get; set; }
        #endregion

		public LoanCenterViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            LoadWebPage();
		}

        private async void LoadWebPage()
        {
            var methods = new ExternalServicesMethods();

            ShowActivityIndicator();

            var url = await methods.GetLoanCenterUrl(LoanType, View, this);

            HideActivityIndicator();

            loanCenterWebView.LoadRequest(new NSUrlRequest(new NSUrl(url)));
        }		
	}
}