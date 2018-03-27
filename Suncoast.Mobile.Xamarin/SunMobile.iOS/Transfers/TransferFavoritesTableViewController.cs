using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunMobile.iOS.Common;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using UIKit;

namespace SunMobile.iOS.Transfers
{
	public partial class TransferFavoritesTableViewController : BaseTableViewController
	{
        public event Action<TransferFavorite> Completed = delegate {};
        private List<TransferFavorite> _favoritesList;
        private bool _updateModel;

		public TransferFavoritesTableViewController(IntPtr handle) : base(handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var rightButton = new UIBarButtonItem("Edit", UIBarButtonItemStyle.Plain, null);
            rightButton.TintColor = AppStyles.TitleBarItemTintColor;
            NavigationItem.SetRightBarButtonItem(rightButton, false);
            rightButton.Clicked += (sender, e) => EditFavorites();

            // Hides the remaining rows.
            tableViewMain.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

            LoadFavorites();
        }

        private async void EditFavorites()
        {
            if (NavigationItem.RightBarButtonItem.Title == "Edit")
            {
                tableViewMain.SetEditing(!tableViewMain.Editing, true);
                NavigationItem.RightBarButtonItem.Title = "Save";
            }
            else
            {
                if (_updateModel)
                {
                    await UpdateFavorites();
                }

                NavigationController.PopViewController(true);
            }
        }

        private async void LoadFavorites()
        {
            try
            {
                ShowActivityIndicator();

                var methods = new TransferMethods();
                var response = await methods.GetTransferFavorites(null, View);

                HideActivityIndicator();

                _favoritesList = new List<TransferFavorite>();

                if (response?.Result != null)
                {
                    _favoritesList = response.Result;
                }

                var tableViewSource = new TransferFavoritesTableViewSource(_favoritesList);

                tableViewSource.ItemSelected += (favorite) =>
                {
                    Completed(favorite);
                    NavigationController.PopViewController(true);
                };

                tableViewSource.ItemsChanged += () =>
                {
                    _updateModel = true;
                };

                tableViewMain.Source = tableViewSource;
                tableViewMain.ReloadData();
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "TransferFavoritesTableViewController:LoadFavorites");
            }
        }

        private async Task UpdateFavorites()
        {
            try
            {
                _favoritesList = ((TransferFavoritesTableViewSource)tableViewMain.Source).GetModel();

                ShowActivityIndicator();

                var methods = new TransferMethods();
                var response = await methods.SetTransferFavorites(_favoritesList, View);

                HideActivityIndicator();
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "TransferFavoritesTableViewController:UpdateFavorites");
            }
        }
    }
}