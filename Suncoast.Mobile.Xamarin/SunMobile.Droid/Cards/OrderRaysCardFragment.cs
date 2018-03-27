using Android.OS;
using Android.Views;
using Android.Support.V4.View;
using Android.Widget;
using Android.Text;
using Android.Graphics;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects.Products;
using SunMobile.Shared.Cards;
using SunMobile.Shared.Utilities.General;
using Android.Content;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums;
using Android.App;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using System;
using SunMobile.Shared.Logging;

namespace SunMobile.Droid.Cards
{
	public class OrderRaysCardFragment : BaseFragment
	{
		public int FIRST_PAGE = 0;
		public float BIG_SCALE = 1.0f;
		public float SMALL_SCALE = 0.8f;
		public const int PAGER_MARGIN = 0;
		private LinearLayout dotsLayout;
		private TextView[] _dots;
		private List<CardInfo> _debitCards;
		private List<CardInfo> _creditCards;
        private CustomCardRequest _customCardRequest;
        private int _currentImagePosition;

        public OrderRaysCardPagerAdapter adapter;
        public TextView lblCard;
        public TableRow rowSelectCard;
        public TableRow rowNames;
        public EditText txtNames;
		public ViewPager viewPager;
        public TextView lblSelectCardHeader;
        public TextView lblSelectImageHeader;
        public TextView lblNamesHeader;
        public Button btnSubmit;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.OrderRaysCardView, null);
			RetainInstance = true;

            _customCardRequest = new CustomCardRequest();

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			((MainActivity)Activity).SetActionBarTitle("Tampa Bay Rays Card");

            lblCard = Activity.FindViewById<TextView>(Resource.Id.lblCard);

            rowSelectCard = Activity.FindViewById<TableRow>(Resource.Id.rowSelectCard);
            rowSelectCard.Click += (sender, e) => 
            {
                SelectCard();
            };

            rowNames = Activity.FindViewById<TableRow>(Resource.Id.rowNames);
            rowNames.Visibility = ViewStates.Gone;

            txtNames = Activity.FindViewById<EditText>(Resource.Id.txtNames);
            txtNames.TextChanged += (sender, e) => Validate();

            btnSubmit = Activity.FindViewById<Button>(Resource.Id.btnSubmit);
            btnSubmit.Enabled = false;
            btnSubmit.Click += (sender, e) => Confirm();

            lblSelectCardHeader = Activity.FindViewById<TextView>(Resource.Id.lblSelectCardHeader);
            lblSelectImageHeader = Activity.FindViewById<TextView>(Resource.Id.lblSelectImageHeader);
            lblNamesHeader = Activity.FindViewById<TextView>(Resource.Id.lblNamesHeader);

			viewPager = Activity.FindViewById<ViewPager>(Resource.Id.myviewpager);
            #pragma warning disable CS0618 // Type or member is obsolete
            viewPager.SetOnPageChangeListener(adapter);
            #pragma warning restore CS0618 // Type or member is obsolete
			viewPager.OffscreenPageLimit = 3;
			viewPager.PageMargin = PAGER_MARGIN;
			viewPager.PageSelected += (sender, e) =>
			{
				OnPageSelected(e.Position);
			};

			dotsLayout = Activity.FindViewById<LinearLayout>(Resource.Id.viewPagerCountDots);			

            ClearAll();

            CheckEligibility();
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                if (((MainActivity)Activity) != null)
                {
                    ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "361ccc96-5ceb-11e7-907b-a6006ad3dba0", "Tampa Bay Rays Card"));
                }

                lblSelectCardHeader.Text = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "47f4e6df-36f7-4f2c-bde8-0341d05c957f", "Select Card to Replace");
                lblSelectImageHeader.Text = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "f11e9bda-49a3-4231-b72b-77b63157d86b", "Select Card Image");
                lblNamesHeader.Text = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "c813a754-6a4b-4f53-973e-913a378e7ecc", "Please specify the name(s) on the account to be replaced");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "OrderRaysCardFragment:SetCultureConfiguration");
			}
        }

		private async void CheckEligibility()
		{
			var methods = new CardMethods();
            var isEligible = await methods.CheckRaysCardEligibility(Activity);

			if (!isEligible)
			{
                NavigationService.NavigatePop();
			}
		}

        private void ClearAll()
        {
            lblCard.Text = string.Empty;
            txtNames.Text = string.Empty;
        }

		private void Validate()
		{
            btnSubmit.Enabled = _customCardRequest.IsDebitCard || !string.IsNullOrEmpty(txtNames.Text);
		}

		private async void Confirm()
		{
            var response = await AlertMethods.Alert(Activity, "SunMobile", $"Are you sure you want to replace your {lblCard.Text}? Please note that members who switch from their current Suncoast Visa credit card to the Suncoast Rays Visa credit card will get a new credit card number.", "OK", "Cancel");

			if (response == "OK")
			{
				OrderCard();
			}
		}

		private async void OrderCard()
		{
			var methods = new CardMethods();

			var card = _customCardRequest.IsDebitCard ? _debitCards[_currentImagePosition] : _creditCards[_currentImagePosition];

			_customCardRequest.CustomCardType = card.ServiceCode;
			_customCardRequest.ReplacementNames = txtNames.Text;

			ShowActivityIndicator();

            var response = await methods.RequestCustomCard(_customCardRequest, Activity);

			HideActivityIndicator();

			if (response != null && response.Success)
			{
				await AlertMethods.Alert(Activity, "SunMobile", "Your Tampa Bay Rays Card has been successfully ordered.  Please allow 7-10 business days to receive card.", "OK");
                NavigationService.NavigatePop();				
			}
			else
			{
				await AlertMethods.Alert(Activity, "SunMobile", "Your request was not able to be fulfilled.  Please try again later.", "OK");
			}
		}

		private void SelectCard()
		{
            GeneralUtilities.CloseKeyboard(Activity);

			var intent = new Intent(Activity, typeof(SelectCardsActivity));
            var selectedCards = new List<BankCard>();
			var json = JsonConvert.SerializeObject(selectedCards);
			intent.PutExtra("CheckedCards", json);
            intent.PutExtra("SingleSelection", true);	
            intent.PutExtra("ShowOnlyCardsEligibleForRaysReplacement", true);
			StartActivityForResult(intent, 0);
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

				var response = await methods.GetCardImages(request, Activity);

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

		public override async void OnActivityResult(int requestCode, int resultCode, Intent data)
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
						var selectedCards = JsonConvert.DeserializeObject<List<BankCard>>(json);
                        var card = selectedCards[0];
                        lblCard.Text = methods.GetCardDisplayName(card);
                        rowNames.Visibility = (card.CardType == CardTypes.CreditCard ? ViewStates.Visible : ViewStates.Gone);
                        await LoadCardImages(card.CardType == CardTypes.CreditCard ? CardImageTypes.CardType.Credit : CardImageTypes.CardType.Debit);

						_customCardRequest = new CustomCardRequest
						{
							IsDebitCard = card.CardType == CardTypes.VisaDebitCard,
							Suffix = card.CardAccountNumber.Substring(card.CardAccountNumber.Length - 4),
							CardHolderName = card.CardHolderName
						};
                       
                        adapter = new OrderRaysCardPagerAdapter(this, ChildFragmentManager, card.CardType == CardTypes.CreditCard ? _creditCards : _debitCards);
						viewPager.Adapter = adapter;
                        adapter.NotifyDataSetChanged();
                        viewPager.SetCurrentItem(FIRST_PAGE, true);

                        if (_dots != null)
                        {
                            for (int i = 0; i < _dots.Length; i++)
                            {
                                dotsLayout.RemoveView(_dots[i]);
                            }
                        }

                        _dots = new TextView[(card.CardType == CardTypes.CreditCard ? _creditCards.Count : _debitCards.Count)];

						for (int i = 0; i < _dots.Length; i++)
						{
							_dots[i] = new TextView(Activity);
                            #pragma warning disable CS0618 // Type or member is obsolete
							_dots[i].Text = Html.FromHtml("&#8226;").ToString();
                            #pragma warning restore CS0618 // Type or member is obsolete
							_dots[i].TextSize = 30;
							dotsLayout.AddView(_dots[i]);
						}

                        OnPageSelected(FIRST_PAGE);

                        Validate();
						break;										
				}
			}
		}
	}
}