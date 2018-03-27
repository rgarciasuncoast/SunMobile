using System;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.BillPay
{
	public partial class SelectPayeeTableViewController : BaseTableViewController
	{
		public event Action<ListViewItem> PayeeSelected = delegate { };
		private UIRefreshControl _refreshControl;

		public SelectPayeeTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.Title = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "140EE20C-3240-428A-8AA5-4D7CEAC739E3", "Select Payee");

			var leftButton = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
			leftButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetLeftBarButtonItem(leftButton, false);
			leftButton.Clicked += (sender, e) => NavigationController.PopViewController(true);

			_refreshControl = new UIRefreshControl();
			TableView.AddSubview(_refreshControl);
			_refreshControl.ValueChanged += (sender, e) => LoadPayees();

			LoadPayees();
		}

		private async void LoadPayees()
		{
			var methods = new BillPayMethods();			

			if (!_refreshControl.Refreshing)
			{
				ShowActivityIndicator();
			}

            var response = await methods.GetPayees(null, View);

			if (!_refreshControl.Refreshing)
			{
				HideActivityIndicator();
			}
			else
			{
				_refreshControl.EndRefreshing();
			}

            if (response != null && response?.Result != null)
			{
                var tableViewSource = new PayeesV2TableViewSource(response.Result, true);

				tableViewSource.ItemSelected += item =>
				{
					PayeeSelected(item);
					NavigationController.PopViewController(true);
				};

				TableView.Source = tableViewSource;
				TableView.ReloadData();
			}
		}
	}
}