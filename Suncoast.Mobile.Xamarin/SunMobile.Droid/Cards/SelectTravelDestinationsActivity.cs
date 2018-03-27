using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Droid.Common;
using SunMobile.Shared.Cards;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.States;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Accounts
{
	[Activity(Label = "SelectTravelDestinationsActivity", Theme = "@style/CustomHoloLightTheme")]
	public class SelectTravelDestinationsActivity : BaseActivity, ICultureConfigurationProvider
	{
		private TextView txtTitle;
		private TextView txtDone;
		private TextView txtRestrictions;
		private TextView btnRestrictionInfo;
		private ImageButton btnCloseWindow;
		private Spinner spinnerListType;
		private ListView listViewStates;
		private ListView listViewCountries;
		private Button btnAdditionalInformation;
		private string _travelInformationUrl;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.SelectTravelDestinationsView);

			txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
			txtRestrictions = FindViewById<TextView>(Resource.Id.lblRestrictions);
			btnRestrictionInfo = FindViewById<TextView>(Resource.Id.btnAdditionalInformation);

			btnCloseWindow = FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnCloseWindow.Click += (sender, e) => Finish();

			spinnerListType = FindViewById<Spinner>(Resource.Id.spinnerListType);
			spinnerListType.ItemSelected += (sender, e) => ListTypeChanged();

			listViewStates = FindViewById<ListView>(Resource.Id.listViewStates);
			listViewCountries = FindViewById<ListView>(Resource.Id.listViewCountries);
			listViewCountries.Visibility = Android.Views.ViewStates.Gone;

			txtDone = FindViewById<TextView>(Resource.Id.txtDone);
			txtDone.Click += (sender, e) => Submit();

			btnAdditionalInformation = FindViewById<Button>(Resource.Id.btnAdditionalInformation);
			btnAdditionalInformation.Click += (sender, e) => ShowRestrictionInformation();

			var stateList = new List<string>(USStates.USStateList.Values);
			var countryList = Countries.CountryList;

			var json = Intent.GetStringExtra("SelectedStates");
			var selectedStates = JsonConvert.DeserializeObject<List<string>>(json);

			json = Intent.GetStringExtra("SelectedCountries");
			var selectedCountries = JsonConvert.DeserializeObject<List<string>>(json);

			if (savedInstanceState != null)
			{
				json = savedInstanceState.GetString("SelectStates");
				selectedStates = JsonConvert.DeserializeObject<List<string>>(json);

				json = savedInstanceState.GetString("SelectCountries");
				selectedCountries = JsonConvert.DeserializeObject<List<string>>(json);
			}

			LoadStates(stateList, selectedStates);
			LoadCountries(countryList, selectedCountries);
			GetTravelNotificationUrl();			
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                CultureTextProvider.SetMobileResourceText(txtTitle, "408B726E-56B9-420D-B97A-47F3B8506420", "1e9837a3-f890-4b1e-94f0-2699e849674b", "Destinations");
                CultureTextProvider.SetMobileResourceText(txtDone, "408B726E-56B9-420D-B97A-47F3B8506420", "7486a25e-86cb-45fd-8ffa-13ae76aa95f9", "Done");
                CultureTextProvider.SetMobileResourceText(txtRestrictions, "408B726E-56B9-420D-B97A-47F3B8506420", "98dc330b-9e71-4705-a7b7-64027e18cac6", "Please be aware that there are restrictions in certain countries for debit and credit card use.");
                CultureTextProvider.SetMobileResourceText(btnRestrictionInfo, "408B726E-56B9-420D-B97A-47F3B8506420", "72002be1-66d6-42c0-8374-16909dcf1b7b", "Additional Travel Notification and Restriction Information");
                var listTypes = new List<string>();
                listTypes.Add(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "84ea763b-4d1f-4e24-983c-b28b9ec3faeb", "United States"));
                listTypes.Add(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "2831015f-e9b6-4ae2-9af0-598cb2a05eea", "International"));
                var adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, listTypes);
                spinnerListType.Adapter = adapter;
                spinnerListType.SetSelection(0);
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectTravelDestinationsActivity:SetCultureConfiguration");
			}
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			var selectedStates = ((StringWithCheckBoxListAdapter)listViewStates.Adapter).SelectedItems;
			var selectedCountries = ((StringWithCheckBoxListAdapter)listViewCountries.Adapter).SelectedItems;

			selectedStates.Sort();
			selectedCountries.Sort();

			var json = JsonConvert.SerializeObject(selectedStates);
			outState.PutString("SelectStates", json);

			json = JsonConvert.SerializeObject(selectedCountries);
			outState.PutString("SelectCountries", json);

			base.OnSaveInstanceState(outState);
		}

		private async void GetTravelNotificationUrl()
		{
			var methods = new CardMethods();
			var response = await methods.GetTravelNotificationInfo(null, this);

			if (response != null && response.Success)
			{
				_travelInformationUrl = response.AdditionalInfoUrl;
			}
		}

		private void ListTypeChanged()
		{
			if (spinnerListType.SelectedItemPosition == 0)
			{
				listViewStates.Visibility = Android.Views.ViewStates.Visible;
				listViewCountries.Visibility = Android.Views.ViewStates.Gone;

			}
			else
			{
				listViewStates.Visibility = Android.Views.ViewStates.Gone;
				listViewCountries.Visibility = Android.Views.ViewStates.Visible;
			}
		}

		private void LoadStates(List<string> list, List<string> selectedItems)
		{
			var listViewItems = new List<ListViewItem>();

			foreach (string s in list)
			{
				var listViewItem = new ListViewItem();
				listViewItem.Item1Text = s;

				if (selectedItems != null && selectedItems.Contains(s))
				{
					listViewItem.IsChecked = true;
				}

				listViewItems.Add(listViewItem);
			}

			var listAdapter = new StringWithCheckBoxListAdapter(this, Resource.Layout.StringWithCheckBoxListViewItem, listViewItems, Resource.Id.lblItem1Text, Resource.Id.checkbox);
			listAdapter.SelectedItems = selectedItems;
			listViewStates.Adapter = listAdapter;
		}

		private void LoadCountries(List<string> list, List<string> selectedItems)
		{
			var listViewItems = new List<ListViewItem>();

			foreach (string s in list)
			{
				var listViewItem = new ListViewItem();
				listViewItem.Item1Text = s;

				if (selectedItems != null && selectedItems.Contains(s))
				{
					listViewItem.IsChecked = true;
				}

				listViewItems.Add(listViewItem);
			}

			var listAdapter = new StringWithCheckBoxListAdapter(this, Resource.Layout.StringWithCheckBoxListViewItem, listViewItems, Resource.Id.lblItem1Text, Resource.Id.checkbox);
			listAdapter.SelectedItems = selectedItems;
			listViewCountries.Adapter = listAdapter;
		}

		private void ShowRestrictionInformation()
		{
			var intent = new Intent(this, typeof(WebViewActivity));
			intent.PutExtra("Title", "Travel Restriction Information");
			intent.PutExtra("Url", _travelInformationUrl);
			StartActivity(intent);
		}

		private void Submit()
		{
			var itemsSelected = new List<string>();

			var selectedStates = ((StringWithCheckBoxListAdapter)listViewStates.Adapter).SelectedItems;
			var selectedCountries = ((StringWithCheckBoxListAdapter)listViewCountries.Adapter).SelectedItems;

			selectedStates.Sort();
			selectedCountries.Sort();

			itemsSelected.AddRange(selectedStates);
			itemsSelected.AddRange(selectedCountries);

			var intent = new Intent();
			intent.PutExtra("ClassName", "SelectTravelDestinationsActivity");

			var json = JsonConvert.SerializeObject(itemsSelected);
			intent.PutExtra("ItemsSelected", json);

			json = JsonConvert.SerializeObject(selectedStates);
			intent.PutExtra("SelectedStates", json);

			json = JsonConvert.SerializeObject(selectedCountries);
			intent.PutExtra("SelectedCountries", json);

			SetResult(Result.Ok, intent);
			Finish();
		}
	}
}