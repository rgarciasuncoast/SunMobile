using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Droid.Common;
using SunMobile.Shared.Cards;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Cards
{
	[Activity(Label = "SelectCardsActivity", Theme = "@style/CustomHoloLightTheme")]
	public class SelectCardsActivity : BaseListActivity, ICultureConfigurationProvider
	{
		private TextView txtTitle;
		private ImageButton btnCloseWindow;
		private TextView txtDone;
		private SwipeRefreshLayout refresher;
		private StatusResponse<List<BankCard>> _viewModel;
		private List<BankCard> _selectedCards;
        private CardListAdapter _cardListAdapter;
        public bool SingleSelection { get; set; }
        public bool ShowOnlyCardsEligibleForRaysReplacement { get; set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			base.SetupView(Resource.Layout.SelectCardsView);

			txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
			txtDone = FindViewById<TextView>(Resource.Id.txtDone);

			refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.selectCardsRefresher);
			refresher.SetColorSchemeResources(Resource.Color.material_blue_grey_800);
			refresher.Refresh += Refresh;

			btnCloseWindow = FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnCloseWindow.Click += (sender, e) => Finish();

			txtDone = FindViewById<TextView>(Resource.Id.txtDone);
			txtDone.Click += (sender, e) => Submit();

			var json = Intent.GetStringExtra("CheckedCards");

			if (!string.IsNullOrEmpty(json))
			{
				_selectedCards = JsonConvert.DeserializeObject<List<BankCard>>(json);
			}

			SingleSelection = Intent.GetBooleanExtra("SingleSelection", false);

			if (SingleSelection)
			{
				txtDone.Visibility = Android.Views.ViewStates.Gone;
			}

            ShowOnlyCardsEligibleForRaysReplacement = Intent.GetBooleanExtra("ShowOnlyCardsEligibleForRaysReplacement", false);

			if (savedInstanceState != null)
			{
				json = savedInstanceState.GetString("SelectCards");
				var selectCards = JsonConvert.DeserializeObject<List<BankCard>>(json);
                _selectedCards = selectCards;
			}

            ListViewMain.ItemClick += (sender, e) =>
            {
                if (SingleSelection)
                {
                    var item = _cardListAdapter.GetListViewItem(e.Position).Data;
                    var itemsSelected = new List<BankCard> { (BankCard)item };
					var intent = new Intent();
					json = JsonConvert.SerializeObject(itemsSelected);
					intent.PutExtra("SelectedCards", json);
					intent.RemoveExtra("CheckedCards");
					SetResult(Result.Ok, intent);
					Finish();
                }
            };			

			LoadCards();
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                CultureTextProvider.SetMobileResourceText(txtTitle, "408B726E-56B9-420D-B97A-47F3B8506420", "1e9837a3-f890-4b1e-94f0-2699e849674b", "Cards");
                CultureTextProvider.SetMobileResourceText(txtDone, "408B726E-56B9-420D-B97A-47F3B8506420", "7486a25e-86cb-45fd-8ffa-13ae76aa95f9", "Done");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectCardsActivity:SetCultureConfiguration");
			}
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			var itemsSelected = new List<BankCard>();

			var selectedCards = ((CardListAdapter)ListAdapter).SelectedItems;

			itemsSelected.AddRange(selectedCards);

			var json = JsonConvert.SerializeObject(itemsSelected);
			outState.PutString("SelectCards", json);

			base.OnSaveInstanceState(outState);
		}

		private void Refresh(object sender, EventArgs e)
		{
			_viewModel = null;
			LoadCards();
		}

		private async void LoadCards()
		{
			var methods = new CardMethods();

			if (_viewModel == null)
			{
				if (!refresher.Refreshing)
				{
					ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "9687DCE3-59CA-428A-906E-A649E6DE3DE1", "Loading..."));
				}

				var cardListRequest = new CardListRequest
				{
					ExcludeAtmCards = true
				};

				_viewModel = await methods.CardList(cardListRequest, this);


				if (!refresher.Refreshing)
				{
					HideActivityIndicator();
				}
				else
				{
					refresher.Refreshing = false;
				}
			}

			if (_viewModel != null && _viewModel.Success)
			{
				foreach (var card in _viewModel.Result)
				{
					if (card.ServiceCodes == null)
					{
						card.ServiceCodes = new List<int>();
					}
				}

				if (ShowOnlyCardsEligibleForRaysReplacement)
				{
					_viewModel.Result.RemoveAll(x => !x.IsEligibleForRaysCard);
				}

				var listViewItems = ViewUtilities.ConvertCardListViewItems(_viewModel.Result);

				if (_selectedCards != null && _selectedCards.Count > 0)
				{
					for (int i = 0; i < listViewItems.Count; i++)
					{
						var bankCard = (BankCard)listViewItems[i].Data;

						foreach (var card in _selectedCards)
						{
							if (card.CardAccountNumber == bankCard.CardAccountNumber)
							{
								listViewItems[i].IsChecked = true;
								break;
							}
						}
					}
				}

				_cardListAdapter = new CardListAdapter(this, Resource.Layout.TwoStringsWithCheckBoxListViewItem, listViewItems, Resource.Id.lblItem1Text, Resource.Id.lblItem2Text, Resource.Id.checkbox);
				_cardListAdapter.SelectedItems = _selectedCards;
                _cardListAdapter.SingleSelection = SingleSelection;
				ListAdapter = _cardListAdapter;
			}
			else
			{
				_viewModel = null;
			}
		}

		private void Submit()
		{
			var itemsSelected = new List<BankCard>();

			var selectedCards = ((CardListAdapter)ListAdapter).SelectedItems;

			itemsSelected.AddRange(selectedCards);

			var intent = new Intent();

			var json = JsonConvert.SerializeObject(itemsSelected);
			intent.PutExtra("SelectedCards", json);

			intent.RemoveExtra("CheckedCards");

			SetResult(Result.Ok, intent);
			Finish();
		}
	}
}