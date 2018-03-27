using System;
using Foundation;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using UIKit;

namespace SunMobile.iOS.BillPay
{
	public partial class BillPayMenuTableViewController : BaseTableViewController
	{
		public BillPayMenuTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();			

			// Hides the remaining rows.
			tableViewMenu.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

			CommonMethods.AddBottomToolbar(this);

			GetBillPayEnrollment();
		}

		public override void SetCultureConfiguration()
		{
            Title = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "7C9B25A3-C56A-4112-99C6-4A34798BFF02", "Bill Pay");
			CultureTextProvider.SetMobileResourceText(lblSchedulePayment, "C42013B2-73B4-4942-87F3-4724E5D88592", "94F29FF8-C65F-49DF-81E1-15FAD7B34BFB", "Schedule Payment");
			CultureTextProvider.SetMobileResourceText(lblPendingPayments, "C42013B2-73B4-4942-87F3-4724E5D88592", "8B782F41-D7E5-4952-8D99-CB7DB932939A", "Pending Payments");
			CultureTextProvider.SetMobileResourceText(lblViewHistory, "C42013B2-73B4-4942-87F3-4724E5D88592", "BE706CA5-0A94-48B9-8B7C-8853740AB0AB", "View History");
			CultureTextProvider.SetMobileResourceText(lblManagePayees, "C42013B2-73B4-4942-87F3-4724E5D88592", "5DA51339-4D43-4A6E-ADB7-0616BB5CCB77", "Manage Payees");
		}

		private async void GetBillPayEnrollment()
		{
			try
			{
				ShowActivityIndicator();

				var methods = new BillPayMethods();
				var response = await methods.IsMemberEnrolledInBillPay(null, View);

				HideActivityIndicator();

				if (response?.IsMemberEnrolledInBillPay == false)
				{
					var agreementTextLabel = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "91890DFB-1A8D-4D21-9546-C97545C739F8", "Bill Pay Agreement");
					var agreementText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "CB130792-B92C-4877-BBE5-FAFA6E5745A5", response.BillPayEnrollmentAgreementText).Replace("\\n", "\n");
					var agreementTextAgree = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "6DF6EB75-22D3-4FF2-B2D5-1EE7CBCC8B79", "I Agree / Enroll");
					var agreementTextEmail = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "0F03A750-3B15-4FB5-8BD9-B292FAC75535", "Email");
					var agreementTextCancel = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "B3A73D02-2DAF-433D-A266-EA46DA6479D6", "Cancel");

					var agreementResponse = await AlertMethods.Alert(View, agreementTextLabel, agreementText, agreementTextAgree, agreementTextEmail, agreementTextCancel);

					if (agreementResponse == agreementTextAgree)
					{
						var memberBillPayEnrollementRequest = new SunBlock.DataTransferObjects.BillPay.MemberBillPayEnrollementRequest
                        {
							IsUnEnroll = false						
						};

						ShowActivityIndicator();

                        await methods.UpdateBillPayEnrollment(memberBillPayEnrollementRequest, View);

						HideActivityIndicator();
					}
					else if (agreementResponse == agreementTextEmail)
					{
						GeneralUtilities.SendEmail(this, null, agreementTextLabel, agreementText, false, true);
						View.UserInteractionEnabled = false;
						View.Alpha = 0.3f;
					}
					else
					{
						View.UserInteractionEnabled = false;
						View.Alpha = 0.3f;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "BillPayMenuTableViewController:GetBillPayEnrollment");
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			try
			{
				switch (indexPath.Row)
				{
					case 0:
						var billPaySchedulePaymentViewController = AppDelegate.StoryBoard.InstantiateViewController("BillPaySchedulePaymentViewController") as BillPaySchedulePaymentViewController;
						billPaySchedulePaymentViewController.PaymentScheduled += isPending =>
						{
							var controller = AppDelegate.StoryBoard.InstantiateViewController("BillPayViewController") as BillPayViewController;
							controller.StartWithPending = isPending;
							NavigationController.PushViewController(controller, true);
						};
						NavigationController.PushViewController(billPaySchedulePaymentViewController, true);
						break;
					case 1:
						var pendingViewController = AppDelegate.StoryBoard.InstantiateViewController("BillPayViewController") as BillPayViewController;
						pendingViewController.StartWithPending = true;
						NavigationController.PushViewController(pendingViewController, true);
						break;
					case 2:
						var historyViewController = AppDelegate.StoryBoard.InstantiateViewController("BillPayViewController") as BillPayViewController;
						historyViewController.StartWithPending = false;
						NavigationController.PushViewController(historyViewController, true);
						break;
					case 3:
						var managePayeesViewController = AppDelegate.StoryBoard.InstantiateViewController("ManagePayeesViewController") as ManagePayeesViewController;
						NavigationController.PushViewController(managePayeesViewController, true);
						break;
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "BillPayMenuTableViewController:RowSelected");
			}
		}
	}
}