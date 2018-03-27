using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;
using SunMobile.Droid.Profile;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.General;

namespace SunMobile.Droid.Accounts.SubAccounts
{
    public class SubAccountsContactFragment : SubAccountsBaseContentFragment, ISubAccountsView
    {
        private TextView lblWelcome;
        private TextView lblName;
        private TextView lblAddress1;
        private TextView lblAddress2;
        private TextView lblPhone;
        private TextView lblEmailAddress;
        private TextView lblWhatWeHave;
        private TextView lblInfoConfirm;
        private TextView lblTitleBody;
        private Button btnUpdate;
        private Button btnSubmit;
        private MemberInformation _memberInformation;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SubAccountsContactView, null);
            RetainInstance = true;

            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            lblWelcome = Activity.FindViewById<TextView>(Resource.Id.lblWelcome);
            lblName = Activity.FindViewById<TextView>(Resource.Id.lblName);
            lblAddress1 = Activity.FindViewById<TextView>(Resource.Id.lblAddress1);
            lblAddress2 = Activity.FindViewById<TextView>(Resource.Id.lblAddress2);
            lblPhone = Activity.FindViewById<TextView>(Resource.Id.lblPhone);
            lblEmailAddress = Activity.FindViewById<TextView>(Resource.Id.lblEmailAddress);
            lblWhatWeHave = Activity.FindViewById<TextView>(Resource.Id.lblInfo);
            lblInfoConfirm = Activity.FindViewById<TextView>(Resource.Id.lblInfoConfirm);
            lblTitleBody = Activity.FindViewById<TextView>(Resource.Id.lblTitleBody);
            btnUpdate = Activity.FindViewById<Button>(Resource.Id.btnUpdate);
            btnUpdate.Click += (sender, e) => UpdateContactInformation();
            btnSubmit = Activity.FindViewById<Button>(Resource.Id.btnSubmit);
            btnSubmit.Click += (sender, e) => GotoNextPage();

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("MemberInformation");
                _memberInformation = JsonConvert.DeserializeObject<MemberInformation>(json);
			}

            ClearAll();

            GetMemberInformation();
        }

        public override void SetCultureConfiguration()
        {
            try
            {

                CultureTextProvider.SetMobileResourceText(lblTitleBody, cultureViewId, "7EFD9659-0118-4C5F-B550-B5D333D04170", "It is the smart choice for you and " +
                "your community. Get all the benefits of free checking plus the added bonus of helping local schools.");

                CultureTextProvider.SetMobileResourceText(lblInfoConfirm, cultureViewId, "A1C02C99-F922-4E27-A3C8-2AB9EAF62CAD", "Once you have confirmed your information, click 'Information is Correct' or click 'Update Information' to make a change.");
                CultureTextProvider.SetMobileResourceText(btnUpdate, cultureViewId, "AAA7DC49-8503-4270-8774-A7AF5C93555A", "Update Information");
                CultureTextProvider.SetMobileResourceText(btnSubmit, cultureViewId, "6AB51975-6EAD-49FC-A58E-177CAAB1320D", "Information Is Correct");
            }

            catch (Exception ex)
            {
                Logging.Log(ex, "SubAccountsContactViewController:SetCultureConfiguration");
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            var json = JsonConvert.SerializeObject(_memberInformation);
			outState.PutString("MemberInformation", json);
			
            base.OnSaveInstanceState(outState);
        }

		private async void GetMemberInformation()
		{
            if (_memberInformation == null)
            {
                var methods = new AccountMethods();
                var request = new MemberInformationRequest
                {
                    MemberId = GeneralUtilities.GetMemberIdAsInt()
                };

                ShowActivityIndicator();

                _memberInformation = await methods.GetMemberInformation(request, Activity);

                HideActivityIndicator();
            }

			if (_memberInformation != null)
			{
                var welcomeText = CultureTextProvider.GetMobileResourceText(cultureViewId, "81B232A0-8B93-40B9-8C50-80E1F6BAABA4", ", as a valued member, we wish to " +
                "extend an invitation to you to open a new checking account to help meet your financial needs.");
				var friendlyFirstName = _memberInformation.FirstName.Substring(0, 1) + _memberInformation.FirstName.Substring(1).ToLower();
                lblWelcome.Text = friendlyFirstName + welcomeText;
                lblWhatWeHave.Text = CultureTextProvider.GetMobileResourceText(cultureViewId, "60B8A27A-2615-44BA-9250-45FBC733CD04", "Here is the information we have for you:");
				lblName.Text = _memberInformation.FullName;
				lblAddress1.Text = _memberInformation.Address1;
				lblAddress2.Text = $"{_memberInformation.City}, {_memberInformation.State} {_memberInformation.Zip}";
				lblPhone.Text = _memberInformation.HomePhone;
				lblEmailAddress.Text = _memberInformation.EmailAddress;

                Info.MemberFullName = _memberInformation.FullName;
			}
		}

		private void ClearAll()
		{
			lblWelcome.Text = string.Empty;			
			lblName.Text = string.Empty;
			lblAddress1.Text = string.Empty;
			lblAddress2.Text = string.Empty;
			lblPhone.Text = string.Empty;
			lblEmailAddress.Text = string.Empty;
		}

		private void UpdateContactInformation()
		{
            var contactInfoFragment = new ContactInfoFragment();
            contactInfoFragment.EditMode = true;

			contactInfoFragment.Updated += (obj) =>
			{
                _memberInformation = null;
				GetMemberInformation();
			};

            NavigationService.NavigatePush(contactInfoFragment, true, false);
		}		
		
        public string Validate()
		{
            return string.Empty;
		}
    }
}