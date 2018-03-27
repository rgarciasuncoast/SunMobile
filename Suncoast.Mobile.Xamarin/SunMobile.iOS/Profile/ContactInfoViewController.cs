using System;
using System.Collections.Generic;
using System.Linq;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;
using SunBlock.DataTransferObjects.Extensions;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.States;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public partial class ContactInfoViewController : BaseTableViewController
	{
		public event Action<bool> Updated = delegate { };
        public bool EditMode { get; set; }

		private static readonly string cultureViewId = "7337FA75-3ABD-4F3F-9749-C416175D506B";

		public ContactInfoViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var update = CultureTextProvider.GetMobileResourceText(cultureViewId, "A4C66F8C-6886-4274-821C-AFCFBABE4897", "Update");
			var rightButton = new UIBarButtonItem(update, UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			rightButton.Clicked += (sender, e) => Submit();

			txtContactAddress1.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 40;
			};

			txtContactAddress2.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 40;
			};

			txtContactCity.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 40;
			};

			txtContactState.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 40;
			};

			txtContactZip.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return replacementString.IsNumericOrEmpty() && newLength <= 10;
			};

			txtContactZip.EditingChanged += OnZipCodeTextChanged;
			txtContactCellPhone.EditingChanged += OnPhoneTextChanged;
			txtContactWorkPhone.EditingChanged += OnPhoneTextChanged;
			txtContactHomePhone.EditingChanged += OnPhoneTextChanged;

			var stateNames = new List<string>(USStates.USStateList.Keys.ToList());
			CommonMethods.CreateDropDownFromTextField(txtContactState, stateNames);

            SetEditMode(EditMode);

            btnEdit.TouchUpInside += (sender, e) => SetEditMode(true);

			// Hides the remaining rows.
			tableViewMain.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

			LoadContactInfo();
		}

        private void SetEditMode(bool editMode)
        {
            EditMode = editMode;

            Title = EditMode ? "Manage Contact Info" : "Verify Contact Info";
            NavigationItem.RightBarButtonItem.Title = EditMode ? "Update" : "Continue";

            if (EditMode)
            {
                cellFooter.RemoveFromSuperview();
            }

            txtContactAddress1.Enabled = EditMode;
            txtContactAddress2.Enabled = EditMode;
            txtContactCity.Enabled = EditMode;
            txtContactState.Enabled = EditMode;
            txtContactZip.Enabled = EditMode;
            txtContactCellPhone.Enabled = EditMode;
            txtContactHomePhone.Enabled = EditMode;
            txtContactWorkPhone.Enabled = EditMode;
            txtContactEmail.Enabled = EditMode;
        }

		private void OnPhoneTextChanged(object sender, EventArgs e)
		{
			((UITextField)sender).Text = Conversions.ToPhoneNumber(((UITextField)sender).Text);
		}

		private void OnZipCodeTextChanged(object sender, EventArgs e)
		{
			((UITextField)sender).Text = Conversions.ToZipCode(((UITextField)sender).Text);
		}

		public override void SetCultureConfiguration()
		{
			try
			{
				var viewText = CultureTextProvider.GetMobileResourceText(cultureViewId, "E0A46944-E931-4ED1-B3C0-7A7E9473F1CF");

				if (!string.IsNullOrEmpty(viewText))
				{
					Title = viewText;
				}

				CultureTextProvider.SetMobileResourceText(lblContactMemberName, cultureViewId, "577123E8-C6D0-438A-9280-3EF13183FCE1");
				CultureTextProvider.SetMobileResourceText(lblContactAddress1, cultureViewId, "20BE74E9-457B-4401-B1BC-CEFDF2C09064");
				CultureTextProvider.SetMobileResourceText(lblContactAddress2, cultureViewId, "EA5485C0-6310-468B-AD1B-5AB3AE78CCED");
				CultureTextProvider.SetMobileResourceText(lblContactCity, cultureViewId, "16D29AF2-44A2-44C0-9AC4-151D9D2B30DE");
				CultureTextProvider.SetMobileResourceText(lblContactState, cultureViewId, "B3250E81-426E-4F93-94D1-A626C36AE28B");
				CultureTextProvider.SetMobileResourceText(lblContactZip, cultureViewId, "D516852C-9702-431B-89CD-0C1980FE0C0B");
				CultureTextProvider.SetMobileResourceText(lblContactHomePhone, cultureViewId, "FFD806D6-CECF-4D9F-BDB0-C29FDC16312B");
				CultureTextProvider.SetMobileResourceText(lblContactWorkPhone, cultureViewId, "A5598D20-169B-411C-B46B-4B91146F8556");
				CultureTextProvider.SetMobileResourceText(lblContactCellPhone, cultureViewId, "620A81D3-4193-429B-BB48-FCFC2E83EEA3");
				CultureTextProvider.SetMobileResourceText(lblContactEmail, cultureViewId, "90514BB8-2D92-476E-B61B-AEE00389360D");
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "ContactInfoViewController:SetCultureConfiguration");
			}
		}

		private void ClearAll()
		{
			txtContactMemberName.Text = string.Empty;
			txtContactAddress1.Text = string.Empty;
			txtContactAddress2.Text = string.Empty;
			txtContactCity.Text = string.Empty;
			txtContactState.Text = string.Empty;
			txtContactZip.Text = string.Empty;
			txtContactHomePhone.Text = string.Empty;
			txtContactWorkPhone.Text = string.Empty;
			txtContactCellPhone.Text = string.Empty;
		}

		private async void LoadContactInfo()
		{
			var accountMethods = new AccountMethods();
			var memberRequest = new MemberInformationRequest { MemberId = int.Parse(SessionSettings.Instance.UserId) };

			ShowActivityIndicator();

			var response = await accountMethods.GetMemberInformation(memberRequest, View);

			HideActivityIndicator();

            if (response != null)
            {
                txtContactMemberName.Text = response.FullName;
                txtContactAddress1.Text = response.Address1.ToUpper();
                txtContactAddress2.Text = response.Address2.ToUpper();
                txtContactCity.Text = response.City.ToUpper();
                txtContactState.Text = response.State.ToUpper();
                txtContactZip.Text = Conversions.ToZipCode(response.Zip);
                txtContactHomePhone.Text = Conversions.ToPhoneNumber(response.HomePhone);
                txtContactWorkPhone.Text = Conversions.ToPhoneNumber(response.WorkPhone);
                txtContactCellPhone.Text = Conversions.ToPhoneNumber(response.CellNumber);
                txtContactEmail.Text = response.EmailAddress.ToUpper();
            }
		}

		private UpdateProfileRequest PopulateUpdateProfileRequest()
		{
			var request = new UpdateProfileRequest();
			request.UserName = txtContactMemberName.Text;
			request.Address1 = txtContactAddress1.Text;
			request.Address2 = txtContactAddress2.Text;
			request.City = txtContactCity.Text;
			request.State = txtContactState.Text;
			request.ZipCode = txtContactZip.Text;
			request.Email = txtContactEmail.Text;

			if (!string.IsNullOrEmpty(txtContactWorkPhone.Text))
			{
				request.WorkPhone = txtContactWorkPhone.Text;
			}
			if (!string.IsNullOrEmpty(txtContactHomePhone.Text))
			{
				request.HomePhone = txtContactHomePhone.Text;
			}
			if (!string.IsNullOrEmpty(txtContactCellPhone.Text))
			{
				request.CellPhone = txtContactCellPhone.Text;
			}

			return request;
		}

		private async void Submit()
		{
            if (EditMode)
            {
                var methods = new AccountMethods();
                var request = PopulateUpdateProfileRequest();

                var message = methods.ValidateUpdateProfileRequest(request);

                if (string.IsNullOrEmpty(message))
                {
                    ShowActivityIndicator();

                    var response = await methods.UpdateProfileInformation(request, View);

                    HideActivityIndicator();

                    if (response != null && response.Success)
                    {
                        var contactSuccessUpdate = CultureTextProvider.GetMobileResourceText("", "", "Contact information was successfully updated.");
                        await AlertMethods.Alert(View, "SunMobile", contactSuccessUpdate, CultureTextProvider.OK());
                        Logging.Track("Contact information updated.");
                        Updated(true);
                        NavigationController.PopViewController(true);
                    }
                    else if ((response != null && !response.Success) || response == null)
                    {
                        var contactUpdateFailedString = CultureTextProvider.GetMobileResourceText("", "", "Contact information update failed.  {0}");
                        await AlertMethods.Alert(View, "SunMobile", string.Format(contactUpdateFailedString, response?.FailureMessage ?? string.Empty), CultureTextProvider.OK());
                    }
                }
                else
                {
                    await AlertMethods.Alert(View, "SunMobile", message, CultureTextProvider.OK());
                }
            }
            else
            {
                Updated(false);
            }
		}
	}
}