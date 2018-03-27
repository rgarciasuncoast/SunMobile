using System;
using System.Collections.Generic;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.iOS.Common;
using SunMobile.Shared.Cards;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using UIKit;

namespace SunMobile.iOS.Cards
{
	public partial class SelectCardsViewController : BaseViewController, ICultureConfigurationProvider
	{
        public bool SingleSelection { get; set; }
        public bool ShowOnlyCardsEligibleForRaysReplacement { get; set; }
        public event Action<List<BankCard>> CardsSelected = delegate { };
		private StatusResponse<List<BankCard>> _viewModel;
		private List<BankCard> _selectedCards;
		private UIRefreshControl _refreshControl;

		public SelectCardsViewController(IntPtr handle) : base(handle)
		{
			_selectedCards = new List<BankCard>();
		}

		public override void ViewDidLoad()
		{
			try
			{
				base.ViewDidLoad();				

				var leftButton = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
				leftButton.TintColor = AppStyles.TitleBarItemTintColor;
				NavigationItem.SetLeftBarButtonItem(leftButton, false);
				leftButton.Clicked += (sender, e) => NavigationController.PopViewController(true);

                if (!SingleSelection)
                {
                    var rightButton = new UIBarButtonItem(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "7486a25e-86cb-45fd-8ffa-13ae76aa95f9", "Done"), UIBarButtonItemStyle.Plain, null);
                    rightButton.TintColor = AppStyles.TitleBarItemTintColor;
                    NavigationItem.SetRightBarButtonItem(rightButton, false);
                    rightButton.Clicked += (sender, e) => Submit();
                }

				_refreshControl = new UIRefreshControl();
				mainTableView.AddSubview(_refreshControl);
				_refreshControl.ValueChanged += Refresh;

				// Hides the remaining rows.
				mainTableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

				LoadCards();
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectAccountViewController:ViewDidLoad");
			}
		}

		private void Refresh(object sender, EventArgs e)
		{
			_viewModel = null;
			LoadCards();
		}

		private void Submit()
		{
			if (CardsSelected != null)
			{
				CardsSelected(_selectedCards);
			}

			NavigationController.PopViewController(true);
		}

		private async void LoadCards()
		{
			var methods = new CardMethods();

			if (_viewModel == null)
			{
				if (!_refreshControl.Refreshing)
				{
					ShowActivityIndicator();
				}

				var cardListRequest = new CardListRequest
				{
					ExcludeAtmCards = true
				};

				_viewModel = await methods.CardList(cardListRequest, View);

				if (!_refreshControl.Refreshing)
				{
					HideActivityIndicator();
				}
				else
				{
					_refreshControl.EndRefreshing();
				}
			}

			if (_viewModel != null && _viewModel.Success)
			{
                if (ShowOnlyCardsEligibleForRaysReplacement)
                {
                    _viewModel.Result.RemoveAll(x => !x.IsEligibleForRaysCard);
                }
                
                var tableViewSource = new CardsTableViewSource(_viewModel.Result, SingleSelection);

				tableViewSource.ItemsSelected += items =>
				{
					_selectedCards = items;

                    if (SingleSelection)
                    {
                        Submit();
                    }
				};

				mainTableView.Source = tableViewSource;
				mainTableView.ReloadData();
			}
			else
			{
				_viewModel = null;
			}
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "1e9837a3-f890-4b1e-94f0-2699e849674b", "Cards");
		}
	}
}