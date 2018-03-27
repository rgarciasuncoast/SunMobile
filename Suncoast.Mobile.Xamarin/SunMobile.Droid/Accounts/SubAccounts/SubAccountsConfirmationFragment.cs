using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;

namespace SunMobile.Droid.Accounts.SubAccounts
{
    public class SubAccountsConfirmationFragment : SubAccountsBaseContentFragment, ISubAccountsView
    {
        private TextView labelConfirmationDescription;
        private TextView txtElectrionicSignatureHeading;
        private TextView txtSignatureConfirmation;
        private TextView lblMemberFullName;
        private EditText txtSignature;
        private ImageView btnFinish;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SubAccountsConfirmationView, null);
            RetainInstance = true;

            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            labelConfirmationDescription = Activity.FindViewById<TextView>(Resource.Id.labelConfirmationDescription);
            txtElectrionicSignatureHeading = Activity.FindViewById<TextView>(Resource.Id.txtElectronicSignatureHeading);
            txtSignatureConfirmation = Activity.FindViewById<TextView>(Resource.Id.txtSubAccountConfirmationElectronicSignatureText);
            txtSignature = Activity.FindViewById<EditText>(Resource.Id.txtSubAccountConfirmationElectronicSignature);

            lblMemberFullName = Activity.FindViewById<TextView>(Resource.Id.txtSubAccountConfirmationElectronicSignatureName);
            lblMemberFullName.Text = Info.MemberFullName;
            btnFinish = Activity.FindViewById<ImageView>(Resource.Id.btnFinish);
            btnFinish.Click += (sender, e) => Validate();
        }

        public override void SetCultureConfiguration()
        {
            try
            {
                if (Info.CreateDebitCard)
                {
                    CultureTextProvider.SetMobileResourceText(labelConfirmationDescription, cultureViewId, "6C08C3C1-9CCC-4766-92C9-53AF3EB06232", "By proceeding, after electronically signing below, you will be openin a Smart Checking account.\n\n " +
                    "Your requested debit card will be mailed to your address of record and received within 7-10 business days. Your PIN will be received 3-5 business days following receipt of the card.\n\n" +
                    "If immediate card access is required, you may request an Instant Issue Debit Card by visiting your local Suncoast Credit Union branch. Please note: If a debit card has already been ordered, " +
                    "we will not be able to fulfill your Instant Issue Debit Card request.");
                }
                else
                {
                    CultureTextProvider.SetMobileResourceText(labelConfirmationDescription, cultureViewId, "C1999BC8-18E8-43C8-BAEA-09D2AC2827AC", "By proceeding, after electronically signing below, you will be openin a Smart Checking account.");
                }

                CultureTextProvider.SetMobileResourceText(txtElectrionicSignatureHeading, cultureViewId, "F0E65503-483D-4D59-95F5-0EAA7A432EE4", "Electronic Signature");
                CultureTextProvider.SetMobileResourceText(txtSignatureConfirmation, cultureViewId, "CB8D7FFA-265A-43D1-830D-3309F0D702F1", "By typing your name as it appears below, you hereby request the account and services indicated above. " +
                "You also agree that your typed signature shall have the same force and effect as your written signature.");
                txtSignature.Hint = CultureTextProvider.GetMobileResourceText(cultureViewId, "D11C9134-AEA8-45D6-AE11-18938E0856B8", "Full Name");

            }

            catch (Exception ex)
            {
                Logging.Log(ex, "SubAccountsConfirmationFragment:SetCultureConfiguration");
            }
        }

        public async void CreateAccount()
        {
            await CreateRocketAccount(true);
        }

        public string Validate()
        {
            var returnValue = string.Empty;

            if (txtSignature.Text.ToLower().Trim() != lblMemberFullName.Text.ToLower())
            {
                returnValue = CultureTextProvider.GetMobileResourceText(cultureViewId, "B61B9980-66C4-4B2F-84BB-01EBB36F1DD4", "Signature does not match the member's full name.");
            }
            else
            {
                CreateAccount();
            }

            return returnValue;
        }
    }
}