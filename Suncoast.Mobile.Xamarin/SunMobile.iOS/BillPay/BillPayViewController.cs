using System;
using SunBlock.DataTransferObjects.BillPay.V2;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using UIKit;

namespace SunMobile.iOS.BillPay
{
	public partial class BillPayViewController : BaseViewController
	{		
		public bool StartWithPending { get; set; }
		private UIRefreshControl _refreshControl;

		public BillPayViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();			

			Title = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "7C9B25A3-C56A-4112-99C6-4A34798BFF02", "Bill Pay");
			var pendingText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "4FCFEB18-9278-4B44-A75E-651F1D68258E", "Pending");
			var historyText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "C60D552C-A5D5-4017-8FDC-8FDF441F39BC", "History");
			segmentListType.SetTitle(pendingText, 0);
			segmentListType.SetTitle(historyText, 1);

			_refreshControl = new UIRefreshControl();
			tableViewPayments.AddSubview(_refreshControl);
			_refreshControl.ValueChanged += RefreshList;
			
			segmentListType.ValueChanged += RefreshList;
			
			segmentListType.SelectedSegment = StartWithPending ? 0 : 1;

			RefreshList(segmentListType, null);
		}		

		private void RefreshList(object sender, EventArgs e)
		{
			// Clear the table
			var tableViewSource = new PaymentsTableViewSource(null, true);
			tableViewPayments.Source = tableViewSource;
			tableViewPayments.ReloadData();

			var selectedSegmentId = segmentListType.SelectedSegment;

			if (selectedSegmentId == 0)
			{				
				RefreshPending();
			}

			if (selectedSegmentId == 1)
			{
                RefreshHistory();
			}
		}

		private async void RefreshHistory()
		{
			try
			{
                var endDate = DateTime.Now.AddDays(1);
                var startDate = endDate.AddYears(-2);												

				var methods = new BillPayMethods();

                var request = new PaymentSearchRequest
				{					
					StartDate = startDate,
					EndDate = endDate
				};

				if (!_refreshControl.Refreshing)
				{
					ShowActivityIndicator();
				}

                var response = await methods.GetHistoryPayments(request, View);

				if (!_refreshControl.Refreshing)
				{
					HideActivityIndicator();
				}
				else
				{
					_refreshControl.EndRefreshing();
				}

                if (response != null && response.Result != null)
				{
                    var tableViewSource = new PaymentsTableViewSource(response.Result, false);

					tableViewSource.ItemSelected += item =>
					{
						var controller = AppDelegate.StoryBoard.InstantiateViewController("BillPayDetailsViewController") as BillPayDetailsViewController;
						controller.CurrentPayment = (Payment)item.Data;
						controller.IsPending = false;
						NavigationController.PushViewController(controller, true);
					};

					tableViewPayments.Source = tableViewSource;
					tableViewPayments.ReloadData();
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "BillPayViewController:RefreshHistory");
			}
		}

		private async void RefreshPending()
		{
			try
			{
				var methods = new BillPayMethods();				

				if (!_refreshControl.Refreshing)
				{
					ShowActivityIndicator();
				}

                var response = await methods.GetPendingPayments(null, View);

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
                    var tableViewSource = new PaymentsTableViewSource(response.Result, true);

					tableViewSource.ItemSelected += item =>
					{
						var controller = AppDelegate.StoryBoard.InstantiateViewController("BillPayDetailsViewController") as BillPayDetailsViewController;
						controller.CurrentPayment = (Payment)item.Data;
						controller.IsPending = true;

						controller.PaymentChanged += isPending =>
						{
							segmentListType.SelectedSegment = isPending ? 0 : 1;
							RefreshList(segmentListType, null);
						};

						NavigationController.PushViewController(controller, true);
					};

					tableViewPayments.Source = tableViewSource;
					tableViewPayments.ReloadData();
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "BillPayViewController:RefreshPending");
			}
		}
	}
}