using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Text;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Plugin.CurrentActivity;
using SunBlock.DataTransferObjects.Products;
using SunMobile.Droid.Cards;
using SunMobile.Shared.Cards;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;

namespace SunMobile.Droid.Accounts.SubAccounts
{
    public class SubAccountsCardFragment : SubAccountsBaseContentFragment, ISubAccountsView
    {
        private List<CardInfo> _debitCards;
        private OrderRaysCardPagerAdapter _adapter;
        private int _currentImagePosition;
        private Switch switchDebitCard;
        private ViewPager viewPager;
        private LinearLayout dotsLayout;
        private TextView[] _dots;
        private TextView lblDebitCard;
        private TextView lblSelectImageHeader;
        private TextView lblCardDescription;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SubAccountsCardView, null);
            RetainInstance = true;

            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            lblDebitCard = Activity.FindViewById<TextView>(Resource.Id.lblDebitCard);
            lblSelectImageHeader = Activity.FindViewById<TextView>(Resource.Id.lblSelectImageHeader);
            lblCardDescription = Activity.FindViewById<TextView>(Resource.Id.lblCardDescription);
            switchDebitCard = Activity.FindViewById<Switch>(Resource.Id.switchDebitCard);
            dotsLayout = Activity.FindViewById<LinearLayout>(Resource.Id.dotsLayout);
            viewPager = Activity.FindViewById<ViewPager>(Resource.Id.viewPager);
            viewPager.PageSelected += (sender, e) =>
            {
                OnPageSelected(e.Position);
            };

            if (savedInstanceState != null)
            {
                var json = savedInstanceState.GetString("DebitCards");
                _debitCards = JsonConvert.DeserializeObject<List<CardInfo>>(json);
            }

            LoadCardImages();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            var json = JsonConvert.SerializeObject(_debitCards);
            outState.PutString("DebitCards", json);

            base.OnSaveInstanceState(outState);
        }

        public override void SetCultureConfiguration()
        {
            try
            {
                CultureTextProvider.SetMobileResourceText(lblDebitCard, cultureViewId, "46190A89-69A6-4A7C-88BA-0DC06E44BD1E", "Send me a Debit Card");
                CultureTextProvider.SetMobileResourceText(lblSelectImageHeader, cultureViewId, "69248029-CB1D-4AE5-AC3C-36D463FCD70B", "Select Card Image");
                CultureTextProvider.SetMobileResourceText(lblCardDescription, cultureViewId, "6458A2B8-7332-48D3-840E-5BF3F7217644", "You’ll get a debit card " +
                "that rewards you and the community. It’s called the Suncoast Rewards Debit Card — and it’s absolutely " +
                "free. Here’s how it works:\n\nEarn Bonus Points\nUse your Rewards Debit Card for all your purchases to " +
                "earn ScoreCard® Rewards Bonus Points, redeemable for exciting travel and valuable merchandise. When you " +
                "use your card, you’ll earn one point for every $3 you spend.\n\nFuel Up Rewards\nSave at the pump with " +
                "Suncoast and ScoreCard Rewards. It's easy, when you swipe your Suncoast credit or debit card at a " +
                "participating BP station. Spend, swipe and save!\n\nGive Back\nIn addition to giving you points, every " +
                "time you make a purchase with your card, Suncoast donates two cents to the Suncoast Credit Union Foundation, " +
                "a 501(c)(3) nonprofit organization established to help children in the communities we serve.");
            }

            catch (Exception ex)
            {
                Logging.Log(ex, "SubAccountsCardFragment:SetCultureConfiguration");
            }
        }


        private async void LoadCardImages()
        {
            var methods = new CardMethods();
            var request = new CardImageRequest
            {
                CardType = CardImageTypes.CardType.Debit.ToString(),
                JustRaysCards = false
            };

            if (_debitCards == null)
            {
                ShowActivityIndicator();

                var response = await methods.GetCardImages(request, CrossCurrentActivity.Current.Activity);

                HideActivityIndicator();

                if (response != null && response.Success)
                {
                    _debitCards = new List<CardInfo>();

                    foreach (var card in response.CardImages)
                    {
                        _debitCards.Add(card);
                    }
                }
            }

            _adapter = new OrderRaysCardPagerAdapter(this, ChildFragmentManager, _debitCards);
            viewPager.Adapter = _adapter;
            _adapter.NotifyDataSetChanged();
            viewPager.SetCurrentItem(0, true);

            _dots = new TextView[_debitCards.Count];

            for (int i = 0; i < _dots.Length; i++)
            {
                _dots[i] = new TextView(CrossCurrentActivity.Current.Activity);
#pragma warning disable CS0618 // Type or member is obsolete
                _dots[i].Text = Html.FromHtml("&#8226;").ToString();
#pragma warning restore CS0618 // Type or member is obsolete
                _dots[i].TextSize = 30;
                dotsLayout.AddView(_dots[i]);
            }

            OnPageSelected(0);

            Validate();
        }

        public void OnPageSelected(int position)
        {
            try
            {
                _currentImagePosition = position;

                for (int i = 0; i < _dots.Length; i++)
                {
                    _dots[i].SetTextColor(Color.Black);
                }

                _dots[position].SetTextColor(Color.White);
            }
            catch { }
        }

        public string Validate()
        {
            string serviceCode = null;

            try
            {
                serviceCode = _debitCards[_currentImagePosition].ServiceCode;
            }
            catch { }

            if (serviceCode == null)
            {
                serviceCode = "0";
            }

            Info.CreateDebitCard = switchDebitCard.Checked;
            Info.CardServiceCode = int.Parse(serviceCode);

            return string.Empty;
        }
    }
}