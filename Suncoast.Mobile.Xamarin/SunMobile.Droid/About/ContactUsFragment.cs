﻿using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunMobile.Droid.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using Uri = Android.Net.Uri;

namespace SunMobile.Droid.About
{
	public class ContactUsFragment : BaseFragment
	{
		private TextView txtInfo;
		private TextView txtPhoneSpecialist;
		private TextView txtPhoneCreditCards;
		private TextView txtPhoneDebitCards;
		private TextView txtPhoneDebitCardsAfterHours;
		private TextView txtEmailSupport;
		private TextView txtSendMessage;
		private TableRow rowWebsite;
		private TableRow rowFacebook;
		private TableRow rowTwitter;
		private TextView lblVersion;
		private TextView lblSpeakToASpecialist;
		private TextView lblReportLostAndStolenCards;
		private TextView lblCreditCards;
		private TextView lblDebitATMCards;
		private TextView lblDebitATMCardsAfterHours;
		private TextView lblDigitalBankingEmail;
		private TextView lblRoutingNumber;
		private TextView lblFindABranchOrATM;
        private TextView lblFacebook;
        private TextView lblTwitter;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.ContactUsView, null);

			return view;
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("55027AD1-817A-4206-8398-56BA3B6EBB6E", "D7C9182E-E4DF-4161-8967-F5B93E3BC1B2", "Contact Us"));
                CultureTextProvider.SetMobileResourceText(txtInfo, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "8DD68B42-604C-46FC-B0CA-0922DA918E49", "Members Care Center\\n Mon - Fri 7am - 8pm Easter\\n Sat 8am-1pm Eastern");
                CultureTextProvider.SetMobileResourceText(lblSpeakToASpecialist, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "E2D88609-F679-4C7A-93AC-A189CFF6E6E4", "Speak to a Specialist");
                CultureTextProvider.SetMobileResourceText(lblReportLostAndStolenCards, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "EB9F49C5-68A9-4028-A737-8E9380DA094F", "Report Lost & Stolen Cards");
                CultureTextProvider.SetMobileResourceText(lblCreditCards, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "B3EF4DA7-E85A-4FD8-A426-BBE5947F97E4", "Credit Cards");
                CultureTextProvider.SetMobileResourceText(lblDebitATMCards, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "30E98B2B-7435-4C26-AD57-8D77935C129B", "Debit/ATM Cards");
                CultureTextProvider.SetMobileResourceText(lblDebitATMCardsAfterHours, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "271B49FD-7982-4A6C-976D-C0441268A5C1", "Debit/ATM Cards after hours");
                CultureTextProvider.SetMobileResourceText(lblDigitalBankingEmail, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "CC768CEE-B784-47ED-8C02-B93F3DE6ED06", "Digital Banking Email");
                CultureTextProvider.SetMobileResourceText(lblRoutingNumber, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "14AE6724-39AA-4964-BDB8-BDDD7008543B", "Routing Number");
                CultureTextProvider.SetMobileResourceText(lblFindABranchOrATM, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "6F7564E9-F36E-436E-A2DC-7D62AE284CEC", "Find a Branch or ATM");
                CultureTextProvider.SetMobileResourceText(lblFacebook, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "D93F6E03-B2FA-42DB-A24B-167C078EB6BD", "Suncoast Facebook Page");
                CultureTextProvider.SetMobileResourceText(lblTwitter, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "C565E50A-0853-4FEE-A50D-196335DE9F69", "Suncoast Twitter Page");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "ContactUsFragment:SetCultureConfiguration");
			}
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			SetupView();			
		}

		public override void SetupView()
		{
			base.SetupView();

			try
			{
				txtInfo = Activity.FindViewById<TextView>(Resource.Id.txtInfo);

				txtPhoneSpecialist = Activity.FindViewById<TextView>(Resource.Id.txtPhoneSpecialist);
				txtPhoneSpecialist.Text = SessionSettings.Instance.GetStartupSettings["PhoneSpecialist"];
				txtPhoneSpecialist.Click += CallNumber;

				txtSendMessage = Activity.FindViewById<TextView>(Resource.Id.txtSendMessage);
				txtSendMessage.Click += SendMessage;

				txtPhoneCreditCards = Activity.FindViewById<TextView>(Resource.Id.txtPhoneCreditCards);
				txtPhoneCreditCards.Text = SessionSettings.Instance.GetStartupSettings["PhoneCreditCards"];
				txtPhoneCreditCards.Click += CallNumber;

				txtPhoneDebitCards = Activity.FindViewById<TextView>(Resource.Id.txtPhoneDebitCards);
				txtPhoneDebitCards.Text = SessionSettings.Instance.GetStartupSettings["PhoneDebitCards"];
				txtPhoneDebitCards.Click += CallNumber;

				txtPhoneDebitCardsAfterHours = Activity.FindViewById<TextView>(Resource.Id.txtPhoneDebitCardsAfterHours);
				txtPhoneDebitCardsAfterHours.Text = SessionSettings.Instance.GetStartupSettings["PhoneDebitCardsAfterHours"];
				txtPhoneDebitCardsAfterHours.Click += CallNumber;

				txtEmailSupport = Activity.FindViewById<TextView>(Resource.Id.txtEmailSupport);
				txtEmailSupport.Text = SessionSettings.Instance.GetStartupSettings["SupportEmail"];
				txtEmailSupport.Click += SendEmail;

				rowWebsite = Activity.FindViewById<TableRow>(Resource.Id.WebSiteRow);
				rowWebsite.Click += (sender, e) => OpenWebsite("Suncoast Credit Union");

				rowFacebook = Activity.FindViewById<TableRow>(Resource.Id.FacebookRow);
				rowFacebook.Click += (sender, e) => OpenWebsite("FacebookUrl", "Suncoast Facebook");

                lblFacebook = Activity.FindViewById<TextView>(Resource.Id.lblFacebook);

				rowTwitter = Activity.FindViewById<TableRow>(Resource.Id.TwitterRow);
				rowTwitter.Click += (sender, e) => OpenWebsite("TwitterUrl", "Suncoast Twitter");

                lblTwitter = Activity.FindViewById<TextView>(Resource.Id.lblTwitter);

				lblVersion = Activity.FindViewById<TextView>(Resource.Id.lblVersion);
				lblVersion.Text = "Version " + GeneralUtilities.GetAppVersion(Activity);

				lblSpeakToASpecialist = Activity.FindViewById<TextView>(Resource.Id.lblSpeakToASpecialist);
				lblReportLostAndStolenCards = Activity.FindViewById<TextView>(Resource.Id.lblReportLostAndStolenCards);
				lblCreditCards = Activity.FindViewById<TextView>(Resource.Id.lblCreditCards);
				lblDebitATMCards = Activity.FindViewById<TextView>(Resource.Id.lblDebitATMCards);
				lblDebitATMCardsAfterHours = Activity.FindViewById<TextView>(Resource.Id.lblDebitATMCardsAfterHours);
				lblDigitalBankingEmail = Activity.FindViewById<TextView>(Resource.Id.lblDigitalBankingEmail);
				lblRoutingNumber = Activity.FindViewById<TextView>(Resource.Id.lblRoutingNumber);
				lblFindABranchOrATM = Activity.FindViewById<TextView>(Resource.Id.lblFindABranchOrATM);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "ContactUsFragment:SetupView");
			}
		}

		private void SendEmail(object sender, EventArgs e)
		{
			try
			{
				GeneralUtilities.SendEmail(Activity, txtEmailSupport.Text, "SunMobile", string.Empty, true);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "ContactUsFragment:SendEmail");
			}
		}

		private void SendMessage(object sender, EventArgs e)
		{
			try
			{
				NavigationService.NavigatePop(true);
				((MainActivity)Activity).ListItemClicked(9);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "ContactUsFragment:SendMessage");
			}
		}

		private void CallNumber(object sender, EventArgs e)
		{
			try
			{
				var phoneNumber = ((TextView)sender).Text;

				var callIntent = new Intent(Intent.ActionDial);
				callIntent.SetData(Uri.Parse("tel:" + phoneNumber));
				StartActivity(callIntent);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "ContactUsFragment:CallNumber");
			}
		}

        private void OpenWebsite(string title)
        {
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(title);

               // string url = SessionSettings.Instance.GetStartupSettings[key];

                var webViewFragment = new BaseWebViewFragment();
                webViewFragment.Url = "http://www.suncoastcreditunion.com";

                NavigationService.NavigatePush(webViewFragment, true);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "ContactUsFragment:OpenWebsite");
            }
        }

		private void OpenWebsite(string key, string title)
		{
			try
			{
				((MainActivity)Activity).SetActionBarTitle(title);

			//	string url = SessionSettings.Instance.GetStartupSettings[key];

				var webViewFragment = new BaseWebViewFragment();
                webViewFragment.Url = "http://www.suncoastcreditunion.com";

				NavigationService.NavigatePush(webViewFragment, true);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "ContactUsFragment:OpenWebsite");
			}
		}
	}
}