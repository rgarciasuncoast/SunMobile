using System;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;
using SunBlock.DataTransferObjects.Extensions;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.States;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid.Profile
{
	public class ContactInfoFragment : BaseFragment
	{
        private TextView txtMemberName;
		private TextView lblMemberName;
		private EditText txtAddress1;
		private TextView lblAddress1;
		private EditText txtAddress2;
		private TextView lblAddress2;
        private EditText txtCity;
		private TextView lblCity;
		private Spinner spinnerState;
		private TextView lblState;
		private EditText txtZip;
		private TextView lblZip;
		private EditText txtHomePhone;
		private TextView lblHomePhone;
		private EditText txtWorkPhone;
		private TextView lblWorkPhone;
		private EditText txtCellPhone;
		private TextView lblCellPhone;
		private EditText txtEmail;
		private TextView lblEMail;
		private Button btnEdit;
        private Button btnUpdate;

        public event Action<bool> Updated = delegate { };
        public bool EditMode { get; set; }

        private static readonly string cultureViewId = "7337FA75-3ABD-4F3F-9749-C416175D506B";

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.ContactInfoView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			SetupView();
		}

		public override void SetCultureConfiguration()
		{
			try
			{
				var viewText = CultureTextProvider.GetMobileResourceText(cultureViewId, "E0A46944-E931-4ED1-B3C0-7A7E9473F1CF");

				if (!string.IsNullOrEmpty(viewText))
				{
					((MainActivity)Activity).SetActionBarTitle(viewText);
				}

                CultureTextProvider.SetMobileResourceText(lblMemberName, cultureViewId, "577123E8-C6D0-438A-9280-3EF13183FCE1");
                CultureTextProvider.SetMobileResourceText(lblAddress1, cultureViewId, "20BE74E9-457B-4401-B1BC-CEFDF2C09064");
                CultureTextProvider.SetMobileResourceText(lblAddress2, cultureViewId, "EA5485C0-6310-468B-AD1B-5AB3AE78CCED");
                CultureTextProvider.SetMobileResourceText(lblCity, cultureViewId, "16D29AF2-44A2-44C0-9AC4-151D9D2B30DE");
                CultureTextProvider.SetMobileResourceText(lblState, cultureViewId, "B3250E81-426E-4F93-94D1-A626C36AE28B");
                CultureTextProvider.SetMobileResourceText(lblZip, cultureViewId, "D516852C-9702-431B-89CD-0C1980FE0C0B");
                CultureTextProvider.SetMobileResourceText(lblHomePhone, cultureViewId, "FFD806D6-CECF-4D9F-BDB0-C29FDC16312B");
                CultureTextProvider.SetMobileResourceText(lblWorkPhone, cultureViewId, "A5598D20-169B-411C-B46B-4B91146F8556");
                CultureTextProvider.SetMobileResourceText(lblCellPhone, cultureViewId, "620A81D3-4193-429B-BB48-FCFC2E83EEA3");
                CultureTextProvider.SetMobileResourceText(lblEMail, cultureViewId, "90514BB8-2D92-476E-B61B-AEE00389360D");
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "ContactInfoFragment:SetCultureConfiguration");
			}
		}

		public override void SetupView()
		{
			base.SetupView();

			((MainActivity)Activity).SetActionBarTitle("Manage Contact Info");

            txtMemberName = Activity.FindViewById<TextView>(Resource.Id.txtContactInfoMemberName); 
            txtAddress1 = Activity.FindViewById<EditText>(Resource.Id.txtContactInfoAddress1);
			txtAddress2 = Activity.FindViewById<EditText>(Resource.Id.txtContactInfoAddress2);
			txtCity = Activity.FindViewById<EditText>(Resource.Id.txtContactInfoCity);
			spinnerState = Activity.FindViewById<Spinner>(Resource.Id.spinnerContactInfoState);
			var stateAdapter = new ArrayAdapter(Activity, Resource.Layout.support_simple_spinner_dropdown_item, USStates.USStateList.Keys.ToList());
			spinnerState.Adapter = stateAdapter;
			txtZip = Activity.FindViewById<EditText>(Resource.Id.txtContactInfoZip);
            txtHomePhone = Activity.FindViewById<EditText>(Resource.Id.txtContactInfoHomePhone);
            txtWorkPhone = Activity.FindViewById<EditText>(Resource.Id.txtContactInfoWorkPhone);
            txtCellPhone = Activity.FindViewById<EditText>(Resource.Id.txtContactInfoCellPhone);
            txtEmail = Activity.FindViewById<EditText>(Resource.Id.txtContactInfoEmail);
            btnEdit = Activity.FindViewById<Button>(Resource.Id.btnEdit);
            btnEdit.Click += (sender, e) => SetEditMode(true);
            btnUpdate = Activity.FindViewById<Button>(Resource.Id.btnUpdate);
            btnUpdate.Click += (sender, e) => Submit();

            lblMemberName = Activity.FindViewById<TextView>(Resource.Id.lblContactInfoMemberName);
            lblAddress1 = Activity.FindViewById<TextView>(Resource.Id.lblContactInfoAddress1);
            lblAddress2 = Activity.FindViewById<TextView>(Resource.Id.lblContactInfoAddress2);
            lblCity = Activity.FindViewById<TextView>(Resource.Id.lblContactInfoCity);
            lblState = Activity.FindViewById<TextView>(Resource.Id.lblContactInfoState);
            lblZip = Activity.FindViewById<TextView>(Resource.Id.lblContactInfoZip);
            lblHomePhone = Activity.FindViewById<TextView>(Resource.Id.lblContactInfoHomePhone);
            lblWorkPhone = Activity.FindViewById<TextView>(Resource.Id.lblContactInfoWorkPhone);
            lblCellPhone = Activity.FindViewById<TextView>(Resource.Id.lblContactInfoCellPhone);
            lblEMail = Activity.FindViewById<TextView>(Resource.Id.lblContactInfoEmail);

            txtHomePhone.AfterTextChanged += OnPhoneTextChanged;
			txtWorkPhone.AfterTextChanged += OnPhoneTextChanged;
			txtCellPhone.AfterTextChanged += OnPhoneTextChanged;

            txtZip.AfterTextChanged += OnZipChanged;

            SetEditMode(EditMode);

			LoadContactInfo();
		}

        private void SetEditMode(bool editMode)
        {
            EditMode = editMode;
            
            ((MainActivity)Activity).SetActionBarTitle(EditMode ? "Manage Contact Info" : "Verify Contact Info");
            btnUpdate.Text = EditMode ? "Update" : "Continue";

            if (EditMode)
            {
                btnEdit.Visibility = ViewStates.Gone;
            }

            txtAddress1.Enabled = EditMode;
            txtAddress2.Enabled = EditMode;
            txtCity.Enabled = EditMode;
            spinnerState.Enabled = EditMode;
            txtZip.Enabled = EditMode;
            txtCellPhone.Enabled = EditMode;
            txtHomePhone.Enabled = EditMode;
            txtWorkPhone.Enabled = EditMode;
            txtEmail.Enabled = EditMode;
        }

        private UpdateProfileRequest PopulateUpdateProfileRequest()
        {
            var request = new UpdateProfileRequest();
            request.UserName = txtMemberName.Text.ToUpper();
            request.Address1 = txtAddress1.Text.ToUpper();
            request.Address2 = txtAddress2.Text.ToUpper();
            request.City = txtCity.Text.ToUpper();
            request.State = spinnerState.SelectedItem.ToString().ToUpper();
            request.ZipCode = txtZip.Text;
            request.Email = txtEmail.Text.ToUpper();

            if (!string.IsNullOrEmpty(txtWorkPhone.Text))
            {
                request.WorkPhone = txtWorkPhone.Text;
            }
            if (!string.IsNullOrEmpty(txtHomePhone.Text))
            {
                request.HomePhone = txtHomePhone.Text;
            }
            if (!string.IsNullOrEmpty(txtCellPhone.Text))
            {
                request.CellPhone = txtCellPhone.Text;
            }

            return request;
        }

		private async void Submit()
		{
            if (EditMode)
            {
                var methods = new AccountMethods();
                var request = new UpdateProfileRequest();
                request = PopulateUpdateProfileRequest();

                var message = methods.ValidateUpdateProfileRequest(request);

                if (string.IsNullOrEmpty(message))
                {
                    ShowActivityIndicator();

                    var response = await methods.UpdateProfileInformation(request, Activity);

                    HideActivityIndicator();

                    if (response != null && response.Success)
                    {
                        var contactSuccessUpdate = CultureTextProvider.GetMobileResourceText("", "", "Contact information was successfully updated.");
                        await AlertMethods.Alert(Activity, "SunMobile", contactSuccessUpdate, CultureTextProvider.OK());
                        NavigationService.NavigatePop();
                        Updated(true);
                        Logging.Track("Contact information updated.");
                    }
                    else if ((response != null && !response.Success) || response == null)
                    {
                        var contactUpdateFailedString = CultureTextProvider.GetMobileResourceText("", "", "Contact information update failed.  {0}");
                        await AlertMethods.Alert(Activity, "SunMobile", string.Format(contactUpdateFailedString, response?.FailureMessage ?? string.Empty), CultureTextProvider.OK());
                    }
                }
                else
                {
                    await AlertMethods.Alert(Activity, "SunMobile", message, CultureTextProvider.OK());
                }
            }
            else
            {
                Updated(false);
            }
		}

		private async void LoadContactInfo()
		{
			var accountMethods = new AccountMethods();
			var memberRequest = new MemberInformationRequest { MemberId = int.Parse(SessionSettings.Instance.UserId) };

            ShowActivityIndicator();

            var response = await accountMethods.GetMemberInformation(memberRequest, Activity);

            HideActivityIndicator();

            if (response != null)
            {
                txtMemberName.Text = response.FullName;
                txtAddress1.Text = response.Address1.ToUpper();
                txtAddress2.Text = response.Address2.ToUpper();
                txtCity.Text = response.City.ToUpper();
                spinnerState.SetSelection(USStates.USStateList.Keys.ToList().FindIndex(x => x == response.State));
                txtZip.Text = Conversions.ToZipCode(response.Zip);
                txtHomePhone.Text = Conversions.ToPhoneNumber(response.HomePhone);
                txtWorkPhone.Text = Conversions.ToPhoneNumber(response.WorkPhone);
                txtCellPhone.Text = Conversions.ToPhoneNumber(response.CellNumber);
                txtEmail.Text = response.EmailAddress.ToUpper();
            }
		}

        private void OnPhoneTextChanged(object sender, EventArgs e)
		{
            ((EditText)sender).AfterTextChanged -= OnPhoneTextChanged;
            ((EditText)sender).Text = Conversions.ToPhoneNumber(((EditText)sender).Text);
			((EditText)sender).SetSelection(((EditText)sender).Text.Length);
			((EditText)sender).AfterTextChanged += OnPhoneTextChanged;
		}

		private void OnZipChanged(object sender, EventArgs e)
		{
			((EditText)sender).AfterTextChanged -= OnZipChanged;

            ((EditText)sender).Text = Conversions.ToZipCode(((EditText)sender).Text);

			((EditText)sender).SetSelection(((EditText)sender).Text.Length);

			((EditText)sender).AfterTextChanged += OnZipChanged;
		}
	}
}