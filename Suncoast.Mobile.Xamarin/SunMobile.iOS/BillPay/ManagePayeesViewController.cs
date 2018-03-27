using System;
using System.Collections.Generic;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.BillPay.V2;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using UIKit;

namespace SunMobile.iOS.BillPay
{
	public partial class ManagePayeesViewController : BaseViewController
	{
		private StatusResponse<List<Payee>> _payeeViewModel;
		private UIRefreshControl _refreshControl;

		public ManagePayeesViewController(IntPtr handle) : base(handle)
		{
		}	

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var rightButtonText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "5B79EC78-2D23-4E79-B85E-42EA2E16A1E3", "Add");
			var rightButton = new UIBarButtonItem(rightButtonText, UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			rightButton.Clicked += (sender, e) => AddPayee();

			_refreshControl = new UIRefreshControl();
			mainTableView.AddSubview(_refreshControl);
			_refreshControl.ValueChanged += Refresh;

			segmentActive.ValueChanged += ActiveChanged;

			LoadPayees(true);			
		}

		public override void SetCultureConfiguration()
		{
			NavigationItem.Title = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "5DA51339-4D43-4A6E-ADB7-0616BB5CCB77", "Manage Payees");
			segmentActive.SetTitle(CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "65BF6355-7D3B-4D4E-A0F1-D9EA155B503F", "Active"), 0);
			segmentActive.SetTitle(CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "B44F5732-E9CE-40AF-AB30-6B40EAD0CDCE", "Inactive"), 1);
		}

		private void Refresh(object sender, EventArgs e)
		{
			_payeeViewModel = null;

			var selectedSegmentId = segmentActive.SelectedSegment;

			LoadPayees(selectedSegmentId == 0);
		}

		private void ActiveChanged(object sender, EventArgs e)
		{
			var selectedSegmentId = segmentActive.SelectedSegment;

			LoadPayees(selectedSegmentId == 0);
		}

		private async void LoadPayees(bool isActive)
		{
			var methods = new BillPayMethods();

			var request = new GetPayeesRequest
			{
				MemberId = GeneralUtilities.GetMemberIdAsInt()
			};

			if (_payeeViewModel == null)
			{
				if (!_refreshControl.Refreshing)
				{
					ShowActivityIndicator();
				}

				_payeeViewModel = await methods.GetPayees(request, null);

				if (!_refreshControl.Refreshing)
				{
					HideActivityIndicator();
				}
				else
				{
					_refreshControl.EndRefreshing();
				}
			}

			if (_payeeViewModel != null && _payeeViewModel.Success)
			{
				var tableViewSource = new PayeesV2TableViewSource(_payeeViewModel.Result, isActive);

				tableViewSource.ItemSelected += item =>
				{
					var updatePayeeViewController = AppDelegate.StoryBoard.InstantiateViewController("UpdatePayeeViewController") as UpdatePayeeViewController;
					updatePayeeViewController.PayeeToEdit = (Payee)item.Data;

					updatePayeeViewController.Updated += obj =>
					{
						_payeeViewModel = null;
						LoadPayees(true);
					};

					NavigationController.PushViewController(updatePayeeViewController, true);
				};

				mainTableView.Source = tableViewSource;
				mainTableView.ReloadData();
			}
		}

		private void AddPayee()
		{
			var updatePayeeViewController = AppDelegate.StoryBoard.InstantiateViewController("UpdatePayeeViewController") as UpdatePayeeViewController;
			updatePayeeViewController.PayeeToEdit = null;

			updatePayeeViewController.Updated += obj =>
			{
				_payeeViewModel = null;
				LoadPayees(true);
			};

			NavigationController.PushViewController(updatePayeeViewController, true);
		}
	}
}