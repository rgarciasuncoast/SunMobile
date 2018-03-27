using System;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts;
using SunMobile.iOS.Common;
using SunMobile.Shared.Data;
using SunMobile.Shared.StringUtilities;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public partial class AlertSettingDetailTableViewController : BaseTableViewController
	{
		public AvailableBalanceThresholdAlertModel Model { get; set; }
		public event Action<AlertSetting> ItemChanged = delegate{};
		private bool _isDirty;

		public AlertSettingDetailTableViewController(IntPtr handle) : base(handle)
		{			
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			lblDescription.Text = Model.DisplayText;
			switchEnabled.On = Model.Enabled;

			if (Model.ThreshHoldAmount > 0)
			{
				txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(Model.ThreshHoldAmount.ToString());
			}

			txtAmount.EditingChanged += (sender, e) => 
			{
				txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(((UITextField)sender).Text);	
				var amount = StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(((UITextField)sender).Text));

				decimal result;
				decimal.TryParse(amount, out result);
				_isDirty = Model.ThreshHoldAmount != result;
			};

			switchEnabled.ValueChanged += (sender, e) =>
			{
				_isDirty = Model.Enabled != switchEnabled.On;
			};

			txtAmount.ShouldChangeCharacters = (textField, range, replacementString) => 
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return replacementString.IsNumericOrEmpty() && newLength <= 15;
			};

			// Hides the remaining rows.
			tableViewMain.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);	
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			if (_isDirty && (IsMovingFromParentViewController || IsBeingDismissed))
			{
				var amount = StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(txtAmount.Text));

				decimal result;
				decimal.TryParse(amount, out result);

				var alertSetting = new AlertSetting 
				{
					Description = "AvailableBalaceThresholdAlertSettings",
					Value = switchEnabled.On,
					Amount = result
				};

				ItemChanged(alertSetting);
			}
		}
	}
}