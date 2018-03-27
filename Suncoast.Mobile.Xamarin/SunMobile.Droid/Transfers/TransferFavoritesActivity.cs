using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunMobile.Droid.Common;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;

namespace SunMobile.Droid.Transfers
{
    [Activity(Label = "TransferFavoritesActivity")]
    public class TransferFavoritesActivity : BaseListActivity
    {
        private TextView btnEdit;
        private ImageButton btnClose;
        private List<TransferFavorite> _favoritesList;
        private bool _updateModel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetupView(Resource.Layout.AccountTransferFavoritesListView);

            ListView.ItemClick += (sender, e) =>
            {
                if (!((TransferFavoritesListAdapter)ListAdapter).EditMode)
                {
                    var favorite = ((TransferFavoritesListAdapter)ListAdapter).GetFavorite(e.Position);
                    var intent = new Intent();
                    var json = JsonConvert.SerializeObject(favorite);
                    intent.PutExtra("Favorite", json);
                    SetResult(Result.Ok, intent);
                    Finish();
                }
            };

            btnEdit = FindViewById<TextView>(Resource.Id.btnEdit);
            btnEdit.Click += (sender, e) => EditFavorites();

            btnClose = FindViewById<ImageButton>(Resource.Id.btnClose);
            btnClose.Click += (sender, e) => Finish();

            LoadFavorites();
        }

        private async void EditFavorites()
        {
            if (btnEdit.Text == "Edit")
            {
                ((TransferFavoritesListAdapter)ListAdapter).EditMode = true;
                ((TransferFavoritesListAdapter)ListAdapter).DragEnabled = true;
                ((TransferFavoritesListAdapter)ListAdapter).NotifyDataSetChanged();
                btnEdit.Text = "Save";
            }
            else
            {
                if (_updateModel)
                {
                    await UpdateFavorites();
                }

                Finish();
            }
        }

        private async void LoadFavorites()
        {
            try
            {
                if (_favoritesList == null)
                {
                    ShowActivityIndicator();

                    var methods = new TransferMethods();
                    var response = await methods.GetTransferFavorites(null, this);

                    if (response?.Result != null)
                    {
                        _favoritesList = response.Result;
                    }

                    HideActivityIndicator();
                }

                if (_favoritesList != null)
                {
                    var listAdapter = new TransferFavoritesListAdapter(this, _favoritesList);

                    listAdapter.ItemsChanged += () =>
                    {
                        _updateModel = true;
                    };

                    ListAdapter = listAdapter;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "TransferFavoritesActivity:LoadFavorites");
            }
        }

        private async Task UpdateFavorites()
        {
            try
            {
                _favoritesList = ((TransferFavoritesListAdapter)ListAdapter).GetModel();

                ShowActivityIndicator();

                var methods = new TransferMethods();
                var response = await methods.SetTransferFavorites(_favoritesList, this);

                HideActivityIndicator();
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "TransferFavoritesActivity:UpdateFavorites");
            }
        }
    }
}