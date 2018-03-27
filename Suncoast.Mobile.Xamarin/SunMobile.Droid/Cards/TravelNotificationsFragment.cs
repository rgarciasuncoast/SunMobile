using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Correspondence;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Droid.Accounts;
using SunMobile.Droid.Common;
using SunMobile.Shared.Cards;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.General;

namespace SunMobile.Droid.Cards
{
    public class TravelNotificationsFragment : BaseFragment, ICultureConfigurationProvider
    {
        private TableRow rowCards;
        private TableRow rowDestinations;
        private TableRow rowStartDate;
        private TableRow rowEndDate;
        private TextView lblCards;
        private TextView lblDestinations;
        private TextView lblStartDate;
        private TextView lblEndDate;
        private EditText txtAdditionalDetails;
        private Button btnSubmit;
        private TextView lblCardsTravelingWithHeader;
        private TextView lblTravelDestinationsHeader;
        private TextView lblTraveStartDateHeader;
        private TextView lblTravelEndDateHeader;
        private TextView lblAdditionalDetailsHeader;
        private TextView lblAdditionalDetailsExampleHeader;
        private DateTime _startDate;
        private DateTime _endDate;
        private List<BankCard> _selectedCards;
        private List<string> _selectedDestinations;
        private List<string> _selectedStates;
        private List<string> _selectedCountries;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.TravelNotificationsView, null);
            RetainInstance = true;

            // This will make sure the toolbar stays visible on rotation.  
            // It was being hidden because of the keyboard open.
            Activity.Window.SetSoftInputMode(SoftInput.AdjustResize);

            _selectedCards = new List<BankCard>();
            _selectedDestinations = new List<string>();
            _selectedStates = new List<string>();
            _selectedCountries = new List<string>();

            return view;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString("Cards", lblCards.Text);
            outState.PutString("Destinations", lblDestinations.Text);
            outState.PutString("StartDate", lblStartDate.Text);
            outState.PutString("EndDate", lblEndDate.Text);
            outState.PutString("AdditionalDetails", txtAdditionalDetails.Text);

            var json = JsonConvert.SerializeObject(_selectedCards);
            outState.PutString("SelectCards", json);
            json = JsonConvert.SerializeObject(_selectedDestinations);
            outState.PutString("ItemsSelect", json);
            json = JsonConvert.SerializeObject(_selectedStates);
            outState.PutString("SelectStates", json);
            json = JsonConvert.SerializeObject(_selectedCountries);
            outState.PutString("SelectCountries", json);

            base.OnSaveInstanceState(outState);
        }

        public override void SetCultureConfiguration()
        {
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "2E4602E6-9AFA-43AC-8F44-E255236CC1E4", "Travel Notifications"));
                CultureTextProvider.SetMobileResourceText(lblCardsTravelingWithHeader, "408B726E-56B9-420D-B97A-47F3B8506420", "9AED3BDA-67F4-4CA5-8DD0-1ABEB367F4A6", "Cards Traveling With");
                CultureTextProvider.SetMobileResourceText(lblTravelDestinationsHeader, "408B726E-56B9-420D-B97A-47F3B8506420", "98A5BD12-6690-4AF0-8E80-7DCBBEE39C3A", "Travel Destinations");
                CultureTextProvider.SetMobileResourceText(lblTraveStartDateHeader, "408B726E-56B9-420D-B97A-47F3B8506420", "f93658cc-eea5-4400-8a34-95ebcee550fc", "Travel Start Date");
                CultureTextProvider.SetMobileResourceText(lblTravelEndDateHeader, "408B726E-56B9-420D-B97A-47F3B8506420", "6b46806b-4cbd-4de3-8cdf-11dfd9dbab0b", "Travel End Date");
                CultureTextProvider.SetMobileResourceText(lblAdditionalDetailsHeader, "408B726E-56B9-420D-B97A-47F3B8506420", "3fae6cbc-f1c7-4207-901c-68036cff5292", "Additional Details (Optional)");
                CultureTextProvider.SetMobileResourceText(lblAdditionalDetailsExampleHeader, "408B726E-56B9-420D-B97A-47F3B8506420", "231607a3-67a8-487b-bd7f-7e7903a157ab", "(e.g. method of travel, destination cities, contact number while traveling)");
                CultureTextProvider.SetMobileResourceText(btnSubmit, "408B726E-56B9-420D-B97A-47F3B8506420", "c5e2b7af-d23c-462a-953a-7bd91b01d149", "Submit");
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "TravelNotificationsFragment:SetCultureConfiguration");
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            lblCardsTravelingWithHeader = Activity.FindViewById<TextView>(Resource.Id.lblCardsTravelingWithHeader);
            lblTravelDestinationsHeader = Activity.FindViewById<TextView>(Resource.Id.lblTravelDestinationsHeader);
            lblTraveStartDateHeader = Activity.FindViewById<TextView>(Resource.Id.lblTravelStartDateHeader);
            lblTravelEndDateHeader = Activity.FindViewById<TextView>(Resource.Id.lblTravelEndDateHeader);
            lblAdditionalDetailsHeader = Activity.FindViewById<TextView>(Resource.Id.lblAdditionalDetailsHeader);
            lblAdditionalDetailsExampleHeader = Activity.FindViewById<TextView>(Resource.Id.lblAdditionalDetailsExampleHeader);

            rowCards = Activity.FindViewById<TableRow>(Resource.Id.rowTravelCards);
            rowCards.Click += (sender, e) => SelectCards();

            rowDestinations = Activity.FindViewById<TableRow>(Resource.Id.rowTravelDestinations);
            rowDestinations.Click += (sender, e) => SelectDestinations();

            rowStartDate = Activity.FindViewById<TableRow>(Resource.Id.rowStartDate);
            rowStartDate.Click += (sender, e) => SelectDate(true);

            rowEndDate = Activity.FindViewById<TableRow>(Resource.Id.rowEndDate);
            rowEndDate.Click += (sender, e) => SelectDate(false);

            lblCards = Activity.FindViewById<TextView>(Resource.Id.lblTravelCards);
            lblDestinations = Activity.FindViewById<TextView>(Resource.Id.lblTravelDestinations);
            lblStartDate = Activity.FindViewById<TextView>(Resource.Id.lblStartDate);
            lblEndDate = Activity.FindViewById<TextView>(Resource.Id.lblEndDate);
            txtAdditionalDetails = Activity.FindViewById<EditText>(Resource.Id.txtAdditionalDetails);

            btnSubmit = Activity.FindViewById<Button>(Resource.Id.btnSubmit);
            btnSubmit.Enabled = false;
            btnSubmit.Click += (sender, e) => Confirm();

            ClearAll();

            if (savedInstanceState != null)
            {
                lblCards.Text = savedInstanceState.GetString("Cards");
                lblDestinations.Text = savedInstanceState.GetString("Destinations");
                lblStartDate.Text = savedInstanceState.GetString("StartDate");
                lblEndDate.Text = savedInstanceState.GetString("EndDate");
                txtAdditionalDetails.Text = savedInstanceState.GetString("AdditionalDetails");

                var json = savedInstanceState.GetString("SelectCards");
                _selectedCards = JsonConvert.DeserializeObject<List<BankCard>>(json);
                json = savedInstanceState.GetString("ItemsSelect");
                _selectedDestinations = JsonConvert.DeserializeObject<List<string>>(json);
                json = savedInstanceState.GetString("SelectStates");
                _selectedStates = JsonConvert.DeserializeObject<List<string>>(json);
                json = savedInstanceState.GetString("SelectCountries");
                _selectedCountries = JsonConvert.DeserializeObject<List<string>>(json);

                Validate();
            }
        }

        private void ClearAll()
        {
            lblCards.Text = string.Empty;
            lblDestinations.Text = string.Empty;
            lblStartDate.Text = string.Empty;
            lblEndDate.Text = string.Empty;
            txtAdditionalDetails.Text = string.Empty;
        }

        private void SelectCards()
        {
            GeneralUtilities.CloseKeyboard(View);

            var intent = new Intent(Activity, typeof(SelectCardsActivity));

            var json = JsonConvert.SerializeObject(_selectedCards);
            intent.PutExtra("CheckedCards", json);

            StartActivityForResult(intent, 0);
        }

        private void SelectDestinations()
        {
            GeneralUtilities.CloseKeyboard(View);

            var intent = new Intent(Activity, typeof(SelectTravelDestinationsActivity));

            var json = JsonConvert.SerializeObject(_selectedStates);
            intent.PutExtra("SelectedStates", json);

            json = JsonConvert.SerializeObject(_selectedCountries);
            intent.PutExtra("SelectedCountries", json);

            StartActivityForResult(intent, 1);
        }

        private void SelectDate(bool isStartDate)
        {
            GeneralUtilities.CloseKeyboard(View);

            var intent = new Intent(Activity, typeof(SelectDateActivity));
            intent.PutExtra("DisableHolidays", false);
            intent.PutExtra("DisableWeekends", false);
            intent.PutExtra("StartDate", DateTime.Today.ToString());

            if (!isStartDate && _startDate != DateTime.MinValue)
            {
                intent.PutExtra("StartDate", _startDate.ToString());
            }

            StartActivityForResult(intent, isStartDate ? 2 : 3);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == (int)Result.Ok && data != null)
            {
                string json;

                switch (requestCode)
                {
                    case 0:
                        var methods = new CardMethods();

                        json = data.GetStringExtra("SelectedCards");
                        _selectedCards = JsonConvert.DeserializeObject<List<BankCard>>(json);

                        var cardsText = string.Empty;

                        foreach (var card in _selectedCards)
                        {
                            cardsText += methods.GetCardDisplayName(card) + "\n";
                        }

                        if (cardsText != string.Empty)
                        {
                            cardsText = cardsText.Substring(0, cardsText.Length - 1);
                        }

                        lblCards.Text = cardsText;

                        Validate();
                        break;
                    case 1:
                        json = data.GetStringExtra("ItemsSelected");
                        _selectedDestinations = JsonConvert.DeserializeObject<List<string>>(json);

                        json = data.GetStringExtra("SelectedStates");
                        _selectedStates = JsonConvert.DeserializeObject<List<string>>(json);

                        json = data.GetStringExtra("SelectedCountries");
                        _selectedCountries = JsonConvert.DeserializeObject<List<string>>(json);

                        string destinations = string.Empty;

                        foreach (string s in _selectedDestinations)
                        {
                            destinations += s + "\n";
                        }

                        if (destinations != string.Empty)
                        {
                            destinations = destinations.Substring(0, destinations.Length - 1);
                        }

                        lblDestinations.Text = destinations;

                        Validate();
                        break;
                    case 2:
                        json = data.GetStringExtra("SelectedDate");
                        _startDate = JsonConvert.DeserializeObject<DateTime>(json);
                        lblStartDate.Text = string.Format("{0:MM/dd/yyyy}", _startDate);
                        Validate();
                        break;
                    case 3:
                        json = data.GetStringExtra("SelectedDate");
                        var selectedDate = JsonConvert.DeserializeObject<DateTime>(json);
                        lblEndDate.Text = string.Format("{0:MM/dd/yyyy}", selectedDate);
                        _endDate = selectedDate;
                        Validate();
                        break;
                }
            }
        }

        private SubmitTravelNotificationsRequest PopulateSubmitTravelNotificationsRequest()
        {
            var request = new SubmitTravelNotificationsRequest();

            request.MemberId = GeneralUtilities.GetMemberIdAsInt();
            request.Cards = _selectedCards;
            request.Locations = lblDestinations.Text;
            request.StartDate = lblStartDate.Text;
            request.EndDate = lblEndDate.Text;
            request.AdditionalDetails = txtAdditionalDetails.Text;

            return request;
        }

        private void Validate()
        {
            var request = PopulateSubmitTravelNotificationsRequest();

            var methods = new CardMethods();
            btnSubmit.Enabled = methods.ValidateSubmitTravelNotificationsRequest(request);
        }

        private async void Confirm()
        {
            var request = PopulateSubmitTravelNotificationsRequest();

            if (_startDate <= _endDate)
            {
                var confirmMessage = string.Format(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "cefa1912-cff6-4a71-aa06-f4f4f5e81a70", "Would you like to submit a travel notification from {0} to {1}, traveling to \n{2}\nfor the cards \n{3}?"), request.StartDate, request.EndDate, request.Locations, lblCards.Text);
                var response = await AlertMethods.Alert(Activity, CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "0cb4aab6-520c-4285-b271-6d3e45206f16", "Confirm Travel Notification"),
                    confirmMessage,
                    CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "c5e2b7af-d23c-462a-953a-7bd91b01d149", "Submit"), CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "4a413a48-b8d7-4289-bf30-351b5dab5dc3", "No, Review"));

                if (response == CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "c5e2b7af-d23c-462a-953a-7bd91b01d149", "Submit"))
                {
                    SubmitTravelNotification(request);
                }
            }
            else
            {
                await AlertMethods.Alert(Activity, CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "2e4602e6-9afa-43ac-8f44-e255236cc1e4", "Travel Notifications"), CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "049C3F72-7623-412C-9AD6-7DD1700C7BFE", "Start date cannot be after End date."), CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "353f4067-a827-408e-890a-da2a52e38d6f", "OK"));
            }
        }

        private async void SubmitTravelNotification(SubmitTravelNotificationsRequest request)
        {
            request.Locations = request.Locations.Replace("\n", ", ");

            ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "9687DCE3-59CA-428A-906E-A649E6DE3DE1", "Loading..."));

            var methods = new CardMethods();
            var response = await methods.SubmitTravelNotifications(request, Activity);

            HideActivityIndicator();

            if (response != null && response.Success)
            {
                await AlertMethods.Alert(Activity, "SunMobile", CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "a11ccd03-d2b1-45d3-9605-378893a8f191", "Successfully submitted travel notification."), "OK");
                NavigationService.NavigatePop(false);

            }
            else if (response != null && !string.IsNullOrEmpty(response.FailureMessage))
            {
                await AlertMethods.Alert(Activity, "SunMobile", response.FailureMessage, CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "353f4067-a827-408e-890a-da2a52e38d6f", "OK"));
            }
            else
            {
                await AlertMethods.Alert(Activity, "SunMobile", CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "d9470103-5b15-4d81-8a38-da8836d7860f", "Error submitting travel notification."), CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "353f4067-a827-408e-890a-da2a52e38d6f", "OK"));
            }
        }
    }
}