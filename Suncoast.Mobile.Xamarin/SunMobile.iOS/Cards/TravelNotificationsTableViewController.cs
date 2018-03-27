using System;
using System.Collections.Generic;
using Foundation;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Correspondence;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.iOS.Common;
using SunMobile.Shared.Cards;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using UIKit;

namespace SunMobile.iOS.Cards
{
    public partial class TravelNotificationsTableViewController : BaseTableViewController, ICultureConfigurationProvider
    {
        private List<BankCard> _selectedCards;
        private DateTime _startDate;
        private DateTime _endDate;
        private List<string> _selectedStates;
        private List<string> _selectedCountries;

        public TravelNotificationsTableViewController(IntPtr handle) : base(handle)
        {
            _selectedCards = new List<BankCard>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var rightButton = new UIBarButtonItem(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "c5e2b7af-d23c-462a-953a-7bd91b01d149", "Submit"), UIBarButtonItemStyle.Plain, null);
            rightButton.TintColor = AppStyles.TitleBarItemTintColor;
            NavigationItem.SetRightBarButtonItem(rightButton, false);
            NavigationItem.RightBarButtonItem.Enabled = false;
            rightButton.Clicked += (sender, e) => Confirm();

            txtAdditionalDetails.Changed += (sender, e) => Validate();

            // Hides the remaining rows.
            mainTableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

            _selectedStates = new List<string>();
            _selectedCountries = new List<string>();

            ClearAll();

            CommonMethods.CreateTextViewWithPlaceHolder(txtAdditionalDetails, CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "0c7e131f-ba88-410b-a050-0a8f6436285f", "(Optional)"));
        }

        private void ClearAll()
        {
            lblCards.Text = string.Empty;
            lblDestinations.Text = string.Empty;
            lblStartDate.Text = string.Empty;
            lblEndDate.Text = string.Empty;
            txtAdditionalDetails.Text = string.Empty;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            switch (indexPath.Row)
            {
                case 0: // Cards
                    SelectCards();
                    break;
                case 1: // Destinations
                    SelectDestinations();
                    break;
                case 2: // Start Date
                    SelectDate(true);
                    break;
                case 3: // End Date
                    SelectDate(false);
                    break;
            }
        }

        private void SelectCards()
        {
            GeneralUtilities.CloseKeyboard(View);

            var cardMethods = new CardMethods();

            var selectCardsViewController = AppDelegate.StoryBoard.InstantiateViewController("SelectCardsViewController") as SelectCardsViewController;

            selectCardsViewController.CardsSelected += items =>
            {
                _selectedCards = items;

                var cardsText = string.Empty;

                foreach (var card in items)
                {
                    cardsText += cardMethods.GetCardDisplayName(card) + "\n";
                }

                if (cardsText != string.Empty)
                {
                    cardsText = cardsText.Substring(0, cardsText.Length - 1);
                }

                lblCards.Text = cardsText;
                lblCards.SizeToFit();

                Validate();
            };

            NavigationController.PushViewController(selectCardsViewController, true);
        }

        private void SelectDestinations()
        {
            GeneralUtilities.CloseKeyboard(View);

            var selectTravelDestinationsViewController = AppDelegate.StoryBoard.InstantiateViewController("SelectTravelDestinationsViewController") as SelectTravelDestinationsViewController;
            selectTravelDestinationsViewController.SelectedStates = _selectedStates;
            selectTravelDestinationsViewController.SelectedCountries = _selectedCountries;

            selectTravelDestinationsViewController.StatesSelected += (states) =>
            {
                _selectedStates = states;
                SetDestinations();
            };

            selectTravelDestinationsViewController.CountriesSelected += (countries) =>
            {
                _selectedCountries = countries;
                SetDestinations();
            };

            NavigationController.PushViewController(selectTravelDestinationsViewController, true);
        }

        private void SetDestinations()
        {
            string destinations = string.Empty;

            foreach (string s in _selectedStates)
            {
                destinations += s + "\n";
            }

            foreach (string s in _selectedCountries)
            {
                destinations += s + "\n";
            }

            if (destinations != string.Empty)
            {
                destinations = destinations.Substring(0, destinations.Length - 1);
            }

            lblDestinations.Text = destinations;

            Validate();
        }

        private void SelectDate(bool isStartDate)
        {
            GeneralUtilities.CloseKeyboard(View);

            var datePickerViewController = AppDelegate.StoryBoard.InstantiateViewController("DatePickerViewController") as DatePickerViewController;
            datePickerViewController.DisableHolidays = false;
            datePickerViewController.DisableWeekends = false;
            datePickerViewController.StartDate = DateTime.Today;

            if (!isStartDate && _startDate != DateTime.MinValue)
            {
                datePickerViewController.StartDate = _startDate;
            }

            datePickerViewController.ItemSelected += date =>
            {
                if (isStartDate)
                {
                    _startDate = date.Date;
                    lblStartDate.Text = string.Format("{0:MM/dd/yyyy}", date);
                }
                else
                {
                    _endDate = date.Date;
                    lblEndDate.Text = string.Format("{0:MM/dd/yyyy}", date);
                }

                Validate();
            };

            NavigationController.PushViewController(datePickerViewController, true);
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
            NavigationItem.RightBarButtonItem.Enabled = methods.ValidateSubmitTravelNotificationsRequest(request);
        }

        private async void Confirm()
        {
            var request = PopulateSubmitTravelNotificationsRequest();

            if (_startDate <= _endDate)
            {
                var response = await AlertMethods.Alert(View, CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "0cb4aab6-520c-4285-b271-6d3e45206f16", "Confirm Travel Notification"),
                                                                        string.Format(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "cefa1912-cff6-4a71-aa06-f4f4f5e81a70", "Would you like to submit a travel notification from {0} to {1}, traveling to \n{2}\nfor the cards \n{3}?"), request.StartDate, request.EndDate, request.Locations, lblCards.Text),
                                                                        CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "c5e2b7af-d23c-462a-953a-7bd91b01d149", "Submit"), CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "4a413a48-b8d7-4289-bf30-351b5dab5dc3", "No, Review"));

                if (response == CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "c5e2b7af-d23c-462a-953a-7bd91b01d149", "Submit"))
                {
                    SubmitTravelNotification(request);
                }
            }
            else
            {
                await AlertMethods.Alert(View, CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "2e4602e6-9afa-43ac-8f44-e255236cc1e4", "Travel Notifications"), CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "049C3F72-7623-412C-9AD6-7DD1700C7BFE", "Start date cannot be after End date."), CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "353f4067-a827-408e-890a-da2a52e38d6f", "OK"));
            }
        }

        private async void SubmitTravelNotification(SubmitTravelNotificationsRequest request)
        {
            request.Locations = request.Locations.Replace("\n", ", ");

            ShowActivityIndicator();

            var methods = new CardMethods();
            var response = await methods.SubmitTravelNotifications(request, null);

            HideActivityIndicator();

            if (response != null && response.Success)
            {
                await AlertMethods.Alert(View, "SunMobile", CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "a11ccd03-d2b1-45d3-9605-378893a8f191", "Successfully submitted travel notification."), CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "353f4067-a827-408e-890a-da2a52e38d6f", "OK"));
                NavigationController.PopViewController(true);
            }
            else if (response != null && !string.IsNullOrEmpty(response.FailureMessage))
            {
                await AlertMethods.Alert(View, "SunMobile", response.FailureMessage, CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "353f4067-a827-408e-890a-da2a52e38d6f", "OK"));
            }
            else
            {
                await AlertMethods.Alert(View, "SunMobile", CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "d9470103-5b15-4d81-8a38-da8836d7860f", "Error submitting travel notification."), CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "353f4067-a827-408e-890a-da2a52e38d6f", "OK"));
            }
        }

        public override void SetCultureConfiguration()
        {
            Title = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "2e4602e6-9afa-43ac-8f44-e255236cc1e4", "Travel Notifications");
            CultureTextProvider.SetMobileResourceText(lblCardsTravelingWith, "408B726E-56B9-420D-B97A-47F3B8506420", "9aed3bda-67f4-4ca5-8dd0-1abeb367f4a6", "Cards Traveling With");
            CultureTextProvider.SetMobileResourceText(lblTravelDestinations, "408B726E-56B9-420D-B97A-47F3B8506420", "98a5bd12-6690-4af0-8e80-7dcbbee39c3a", "Travel Destinations");
            CultureTextProvider.SetMobileResourceText(lblTravelStartDate, "408B726E-56B9-420D-B97A-47F3B8506420", "f93658cc-eea5-4400-8a34-95ebcee550fc", "Travel Start Date");
            CultureTextProvider.SetMobileResourceText(lblTravelEndDate, "408B726E-56B9-420D-B97A-47F3B8506420", "6b46806b-4cbd-4de3-8cdf-11dfd9dbab0b", "Travel End Date");
            CultureTextProvider.SetMobileResourceText(lblAdditionalDetails, "408B726E-56B9-420D-B97A-47F3B8506420", "3fae6cbc-f1c7-4207-901c-68036cff5292", "Additional Details (Optional)");
            CultureTextProvider.SetMobileResourceText(lblAdditionalDetailsExample, "408B726E-56B9-420D-B97A-47F3B8506420", "231607a3-67a8-487b-bd7f-7e7903a157ab", "(e.g., method of travel, destination cities, contact number while traveling)");
        }
    }
}