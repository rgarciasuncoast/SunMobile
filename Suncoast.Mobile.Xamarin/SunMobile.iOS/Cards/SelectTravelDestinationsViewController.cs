using System;
using System.Collections.Generic;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.iOS.Common;
using SunMobile.Shared.Cards;
using SunMobile.Shared.Culture;
using SunMobile.Shared.States;
using UIKit;

namespace SunMobile.iOS.Cards
{
	public partial class SelectTravelDestinationsViewController : BaseViewController, ICultureConfigurationProvider
	{
		public event Action<List<string>> StatesSelected = delegate { };
        public event Action<List<string>> CountriesSelected = delegate { };
        // These are optional objects you can pass in
        public List<string> SelectedStates { get; set; }
        public List<string> SelectedCountries { get; set; }
		
		private string _travelInformationUrl;

		public SelectTravelDestinationsViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();						

			var rightButton = new UIBarButtonItem(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "7486a25e-86cb-45fd-8ffa-13ae76aa95f9", "Done"), UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			NavigationItem.RightBarButtonItem.Enabled = false;
			rightButton.Clicked += (sender, e) => Submit();

			btnRestrictionInformation.TouchUpInside += (sender, e) => ShowRestrictionInformation();

			var listType = new List<string>();
			listType.Add(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "84ea763b-4d1f-4e24-983c-b28b9ec3faeb", "United States"));
			listType.Add(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "2831015f-e9b6-4ae2-9af0-598cb2a05eea", "International"));
            txtListType.Text = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "84ea763b-4d1f-4e24-983c-b28b9ec3faeb", "United States");

			var stateList = new List<string>(USStates.USStateList.Values);
			var countryList = Countries.CountryList;

			CommonMethods.CreateDropDownFromTextFieldWithDelegate(txtListType, listType, (text) =>
			{
				ListTypeChanged(text);
			});

			// Hides the remaining rows.
			tableViewStates.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

			LoadStates(stateList);
			LoadCountries(countryList);
			GetTravelNotificationUrl();
		}

		private async void GetTravelNotificationUrl()
		{
			var methods = new CardMethods();

            ShowActivityIndicator();

			var response = await methods.GetTravelNotificationInfo(null, null);

            HideActivityIndicator();

			if (response != null && response.Success)
			{
				_travelInformationUrl = response.AdditionalInfoUrl;
			}
		}

		private void ShowRestrictionInformation()
		{
			if (!string.IsNullOrEmpty(_travelInformationUrl))
			{
				var webViewController = AppDelegate.StoryBoard.InstantiateViewController("WebViewController") as WebViewController;
				webViewController.HeaderTitle = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "5CCD7143-A551-4871-9618-11389C83FCE4", "Travel Information");
				webViewController.Url = _travelInformationUrl;
				NavigationController.PushViewController(webViewController, true);
			}
		}

		private void ListTypeChanged(string text)
		{
			if (text == CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "84ea763b-4d1f-4e24-983c-b28b9ec3faeb", "United States"))
			{
				tableViewStates.Hidden = false;
				tableViewCountries.Hidden = true;
			}
			else
			{
				tableViewStates.Hidden = true;
				tableViewCountries.Hidden = false;
			}
		}

		private void Submit()
		{
			var itemsSelected = new List<string>();
            SelectedStates.Sort();
            SelectedCountries.Sort();
            StatesSelected(SelectedStates);
            CountriesSelected(SelectedCountries);			
			NavigationController.PopViewController(true);
		}

		private void LoadStates(List<string> list)
		{
            var tableViewSource = new StringListTableViewSource(list, SelectedStates);

			tableViewSource.ItemsSelected += items =>
			{
				SelectedStates = items;
				NavigationItem.RightBarButtonItem.Enabled = true;
			};

			tableViewStates.Source = tableViewSource;
			tableViewStates.ReloadData();
		}

		private void LoadCountries(List<string> list)
		{
			var tableViewSource = new StringListTableViewSource(list, SelectedCountries);

			tableViewSource.ItemsSelected += items =>
			{
				SelectedCountries = items;
				NavigationItem.RightBarButtonItem.Enabled = true;
			};

			tableViewCountries.Source = tableViewSource;
			tableViewCountries.ReloadData();
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "a17f6f23-f571-4a3d-b248-f454b868d572", "Destinations");
			CultureTextProvider.SetMobileResourceText(lblDisclaimer, "408B726E-56B9-420D-B97A-47F3B8506420", "98dc330b-9e71-4705-a7b7-64027e18cac6", "Please be aware that there are restrictions in certain countries for debit and credit card use.");
			CultureTextProvider.SetMobileResourceText(btnRestrictionInformation, "408B726E-56B9-420D-B97A-47F3B8506420", "72002be1-66d6-42c0-8374-16909dcf1b7b", "Additional Travel Notification and Restriction Information");			
		}
	}
}