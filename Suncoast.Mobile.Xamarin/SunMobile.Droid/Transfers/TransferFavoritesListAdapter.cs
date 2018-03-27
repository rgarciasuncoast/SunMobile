using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using System;
using SunMobile.Droid.Common;

namespace SunMobile.Droid.Transfers
{
    public class TransferFavoritesListAdapter : BaseAdapter, IDraggableListAdapter
    {
        public event Action ItemsChanged = delegate {};
        public bool EditMode { get; set; }
        public int MobileCellPosition { get; set; }
        public bool DragEnabled { get; set; }
        private Activity _activity;
        private readonly List<TransferFavorite> _model;

        public TransferFavoritesListAdapter(Activity activity, List<TransferFavorite> model)
        {
            _activity = activity;
            _model = model;
            MobileCellPosition = int.MinValue;
            DragEnabled = false;
        }

        public override int Count
        {
            get { return _model.Count; }
        }

        public List<TransferFavorite> GetModel()
        {
            return _model;
        }

        public TransferFavorite GetFavorite(int position)
        {
            return _model[position];
        }

       public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row;

            if (convertView == null)
            {
                row = _activity.LayoutInflater.Inflate(Resource.Layout.AccountTransferFavoritesListViewItem, null);
            }
            else
            {
                row = convertView;
            }

            var item = _model[position];

            var lblTitle = row.FindViewById<TextView>(Resource.Id.txtTitle);
            var imgDelete = row.FindViewById<ImageView>(Resource.Id.imgDelete);
            var imgDrag = row.FindViewById<ImageView>(Resource.Id.imgDrag);

            lblTitle.Text = item.FriendlyFavoriteName;
            imgDelete.Tag = item.Id.ToString();
            imgDelete.Visibility = EditMode ? ViewStates.Visible : ViewStates.Gone;         
            imgDrag.Visibility = DragEnabled ? ViewStates.Visible : ViewStates.Gone;

            imgDelete.Click += (sender, e) => 
            {
                foreach (var favorite in _model)
                {
                    if (favorite.Id.ToString() == imgDelete.Tag.ToString())
                    {
                        _model.Remove(favorite);
                        ItemsChanged();
                        NotifyDataSetChanged();
                        break;
                    }
                }
            };

            row.Visibility = MobileCellPosition == position ? ViewStates.Invisible : ViewStates.Visible;
            row.TranslationY = 0;

            return row;
        }

        public void SwapItems(int from, int to)
        {
            var oldValue = _model[from];
            _model[from] = _model[to];
            _model[to] = oldValue;
            MobileCellPosition = to;
            ItemsChanged();
            NotifyDataSetChanged();
        }
    }
}