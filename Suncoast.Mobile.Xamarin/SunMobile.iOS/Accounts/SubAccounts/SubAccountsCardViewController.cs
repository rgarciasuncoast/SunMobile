using System;
using System.Collections.Generic;
using CoreGraphics;
using SunBlock.DataTransferObjects.Products;
using SunMobile.Shared.Cards;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.Images;
using UIKit;
using Xamarin.iOS.iCarouselBinding;

namespace SunMobile.iOS.Accounts.SubAccounts
{
    public partial class SubAccountsCardViewController : SubAccountsBaseContentViewController, ISubAccountsView
    {
        private iCarousel _carousel;
        private List<CardInfo> _debitCards;

        public SubAccountsCardViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            viewPlaceHolder.Hidden = true;
            viewSeparatorBottom.Hidden = true;

            _carousel = new iCarousel
            {
                Type = iCarouselType.CoverFlow2,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                Frame = new CGRect(0, viewPlaceHolder.Frame.Top + 90, View.Frame.Width, 200)
            };

            View.AddSubview(_carousel);

            LoadCardImages();
        }

        public override void SetCultureConfiguration()
        {
            try
            {
                CultureTextProvider.SetMobileResourceText(lblDebitCard, cultureViewId, "46190A89-69A6-4A7C-88BA-0DC06E44BD1E", "Send me a Debit Card");
                CultureTextProvider.SetMobileResourceText(lblSelectCardImage, cultureViewId, "69248029-CB1D-4AE5-AC3C-36D463FCD70B", "Select Card Image");
                CultureTextProvider.SetMobileResourceText(lblCardDescription, cultureViewId, "6458A2B8-7332-48D3-840E-5BF3F7217644", "You’ll get a debit " +
                "card that rewards you and the community. It’s called the Suncoast Rewards Debit Card — and it’s " +
                "absolutely free. Here’s how it works:\n\nEarn Bonus Points\nUse your Rewards Debit Card for all " +
                "your purchases to earn ScoreCard® Rewards Bonus Points, redeemable for exciting travel and valuable " +
                "merchandise. When you use your card, you’ll earn one point for every $3 you spend.\n\nFuel Up Rewards\n" +
                "Save at the pump with Suncoast and ScoreCard Rewards. It's easy, when you swipe your Suncoast credit or " +
                "debit card at a participating BP station. Spend, swipe and save!\n\nGive Back\nIn addition to giving you " +
                "points, every time you make a purchase with your card, Suncoast donates two cents to the Suncoast Credit Union " +
                "Foundation, a 501(c)(3) nonprofit organization established to help children in the communities we serve.");

                lblCardDescription.SizeToFit();
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "SubAccountsCardViewController:SetCultureConfiguration");
            }
        }

        public string Validate()
        {
            string serviceCode = null;

            try
            {
                serviceCode = _debitCards[(int)_carousel.CurrentItemIndex].ServiceCode;
            }
            catch { }

            if (serviceCode == null)
            {
                serviceCode = "0";
            }

            ((SubAccountsViewController)ParentViewController.ParentViewController).CreateDebitCard = switchDebitCard.On;
            ((SubAccountsViewController)ParentViewController.ParentViewController).CardServiceCode = int.Parse(serviceCode);

            return string.Empty;
        }

        private async void LoadCardImages()
        {
            var methods = new CardMethods();
            var request = new CardImageRequest
            {
                CardType = CardImageTypes.CardType.Debit.ToString(),
                JustRaysCards = false
            };

            ShowActivityIndicator();

            var response = await methods.GetCardImages(request, View);

            HideActivityIndicator();

            _debitCards = new List<CardInfo>();

            if (response != null && response.Success)
            {
                foreach (var card in response.CardImages)
                {
                    _debitCards.Add(card);
                }
            }

            _carousel.DataSource = new CarouselDataSource(_debitCards);
            _carousel.ReloadData();
            _carousel.ScrollToItemAtIndex(0, 0);
        }

        private class CarouselDataSource : iCarouselDataSource
        {
            private List<CardInfo> _debitCards;

            public CarouselDataSource(List<CardInfo> debitCards)
            {
                _debitCards = debitCards;
            }

            public override nint NumberOfItemsInCarousel(iCarousel carousel)
            {
                return (_debitCards.Count);
            }

            public override UIView ViewForItemAtIndex(iCarousel carousel, nint index, UIView view)
            {
                var mainView = new UIView(new CGRect(0, 0, 280, 200));

                var imageView = new UIImageView(new CGRect(0, 0, 280, 180))
                {
                    Image = Images.ConvertByteArrayToUIImage(_debitCards[(int)index].CardImage),
                    ContentMode = UIViewContentMode.ScaleAspectFit
                };

                mainView.AddSubview(imageView);

                return mainView;
            }
        }
    }
}