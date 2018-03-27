using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums;
using SunBlock.DataTransferObjects.Products;
using SunMobile.iOS.Common;
using SunMobile.Shared.Cards;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Images;
using UIKit;
using Xamarin.iOS.iCarouselBinding;

namespace SunMobile.iOS.Cards
{
	public partial class OrderRaysCardViewController : BaseViewController
	{
        private iCarousel _carousel;
        private List<CardInfo> _debitCards;
        private List<CardInfo> _creditCards;
        private CustomCardRequest _customCardRequest;

		public OrderRaysCardViewController(IntPtr handle) : base(handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			var rightButton = new UIBarButtonItem("Submit", UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			NavigationItem.RightBarButtonItem.Enabled = false;
			rightButton.Clicked += (sender, e) => Confirm();

			var tapGesture = new UITapGestureRecognizer();
			tapGesture.AddTarget(() => SelectCard());
            viewSelectCard.AddGestureRecognizer(tapGesture);

			ClearAll();

            viewPlaceholder.Hidden = true;
            viewNames.Hidden = true;
            viewSeparatorBottom.Hidden = true;

            txtNames.EditingChanged += (sender, e) => Validate();

			_carousel = new iCarousel
			{
				Type = iCarouselType.CoverFlow2,				
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                Frame = new CGRect(0, viewPlaceholder.Frame.Top + 30, View.Frame.Width, 200)
			};

			View.AddSubview(_carousel);

            CheckEligibility();
        }

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "361ccc96-5ceb-11e7-907b-a6006ad3dba0", "Tampa Bay Rays Card");
            lblSelectCardHeader.Text = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "47f4e6df-36f7-4f2c-bde8-0341d05c957f", "Select Card to Replace");
            lblSelectCardImage.Text = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "f11e9bda-49a3-4231-b72b-77b63157d86b", "Select Card Image");
            lblNamesLabel.Text = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "c813a754-6a4b-4f53-973e-913a378e7ecc", "Please specify the name(s) on the account to be replaced");
		}

        private async void CheckEligibility()
        {
            try
            {
                var methods = new CardMethods();
                var isEligible = await methods.CheckRaysCardEligibility(View);

                if (!isEligible)
                {
                    NavigationController.PopViewController(true);
                }
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "OrderRaysCardViewController:CheckEligibility");
            }
        }

        private void SelectCard()
        {
            var cardMethods = new CardMethods();
            var selectCardsViewController = AppDelegate.StoryBoard.InstantiateViewController("SelectCardsViewController") as SelectCardsViewController;
            selectCardsViewController.SingleSelection = true;
            selectCardsViewController.ShowOnlyCardsEligibleForRaysReplacement = true;

            selectCardsViewController.CardsSelected += async items =>
            {
                if (items != null && items.Count > 0)
                {
                    var card = items[0];
                    lblCard.Text = cardMethods.GetCardDisplayName(card);
                    viewNames.Hidden = viewSeparatorBottom.Hidden = !(card.CardType == CardTypes.CreditCard);
                    await LoadCardImages(card.CardType == CardTypes.CreditCard ? CardImageTypes.CardType.Credit : CardImageTypes.CardType.Debit);
                    _carousel.DataSource = new CarouselDataSource(this, card.CardType);
                    _carousel.ReloadData();
                    _carousel.ScrollToItemAtIndex(0, 0);

					_customCardRequest = new CustomCardRequest
					{
						IsDebitCard = card.CardType == CardTypes.VisaDebitCard,
						Suffix = card.CardAccountNumber.Substring(card.CardAccountNumber.Length - 4),
						CardHolderName = card.CardHolderName
					};

                    Validate();
                }
            };

			NavigationController.PushViewController(selectCardsViewController, true);
		}

        private async Task LoadCardImages(CardImageTypes.CardType cardType)
        {
            var methods = new CardMethods();
            var request = new CardImageRequest
            {
                CardType = cardType.ToString(),
                JustRaysCards = true
            };

            if (_debitCards == null || _creditCards == null)
            {
                ShowActivityIndicator();

                var response = await methods.GetCardImages(request, View);

                HideActivityIndicator();

                if (response != null && response.Success)
                {
                    _creditCards = new List<CardInfo>();
                    _debitCards = new List<CardInfo>();

                    foreach (var card in response.CardImages)
                    {
                        if (card.CardType == CardImageTypes.CardType.Credit.ToString())
                        {
                            _creditCards.Add(card);
                        }
                        else
                        {
                            _debitCards.Add(card);
                        }
                    }
                }
            }
        }

        private void ClearAll()
        {
            lblCard.Text = string.Empty;
            txtNames.Text = string.Empty;
        }

        private void Validate()
        {
            NavigationItem.RightBarButtonItem.Enabled = _customCardRequest.IsDebitCard || !string.IsNullOrEmpty(txtNames.Text);
        }

        private async void Confirm()
        {
            var response = await AlertMethods.Alert(View, "SunMobile", $"Are you sure you want to replace your {lblCard.Text}? Please note that members who switch from their current Suncoast Visa credit card to the Suncoast Rays Visa credit card will get a new credit card number.", "OK", "Cancel");

            if (response == "OK")
            {
                OrderCard();
            }
        }

        private async void OrderCard()
        {
            var methods = new CardMethods();

            var card = _customCardRequest.IsDebitCard ? _debitCards[(int)_carousel.CurrentItemIndex] : _creditCards[(int)_carousel.CurrentItemIndex];

            _customCardRequest.CustomCardType = card.ServiceCode;
            _customCardRequest.ReplacementNames = txtNames.Text;

            ShowActivityIndicator();

            var response = await methods.RequestCustomCard(_customCardRequest, View);

            HideActivityIndicator();

            if (response != null && response.Success)
            {
                await AlertMethods.Alert(View, "SunMobile", "Your Tampa Bay Rays Card has been successfully ordered.  Please allow 7-10 business days to receive card.", "OK");
                NavigationController.PopViewController(true);
            }
            else
            {
                await AlertMethods.Alert(View, "SunMobile", "Your request was not able to be fulfilled.  Please try again later.", "OK");
            }
		}

        private class CarouselDataSource : iCarouselDataSource
        {
            private OrderRaysCardViewController _parentView;
            private CardTypes _cardType;

            public CarouselDataSource(OrderRaysCardViewController parentView, CardTypes cardType)
            {
                _parentView = parentView;
                _cardType = cardType;
            }

            public override nint NumberOfItemsInCarousel(iCarousel carousel)
            {
                return (_cardType == CardTypes.CreditCard ? _parentView._creditCards.Count : _parentView._debitCards.Count);
            }

            public override UIView ViewForItemAtIndex(iCarousel carousel, nint index, UIView view)
            {
				var mainView = new UIView(new CGRect(0, 0, 280, 200));

				var imageView = new UIImageView(new CGRect(0, 0, 280, 180))
				{
					Image = Images.ConvertByteArrayToUIImage(_cardType == CardTypes.CreditCard ? _parentView._creditCards[(int)index].CardImage : _parentView._debitCards[(int)index].CardImage),
					ContentMode = UIViewContentMode.ScaleAspectFit
				};

				mainView.AddSubview(imageView);

				return mainView;
            }
        }
	}
}