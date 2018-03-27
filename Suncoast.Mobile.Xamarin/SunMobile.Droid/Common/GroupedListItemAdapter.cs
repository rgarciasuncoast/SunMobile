using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Common
{
    public class GroupedListItemAdapter : BaseAdapter
    {
        private Dictionary<string, IAdapter> sections = new Dictionary<string, IAdapter>();
        private ArrayAdapter<string> headers;
        private const int TYPE_SECTION_HEADER = 0;

        public GroupedListItemAdapter(Context context)
        {
            headers = new ArrayAdapter<string>(context, Resource.Layout.GroupListHeader);
        }

        public void AddSection(string section, IAdapter adapter)
        {
            headers.Add(section);
            sections.Add(section, adapter);
        }

        public ListViewItem GetListViewItem(int position)
        {
            foreach (var section in sections.Keys) 
            {
                var adapter = sections[section];
                int size = adapter.Count + 1;

                if (position < size)
                {
                    return ((GenericListAdapter)adapter).GetListViewItem(position - 1);
                }

                position -= size;
            }

            return null;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            foreach (var section in sections.Keys) 
            {
                var adapter = sections[section];
                int size = adapter.Count + 1;

                if (position == 0)
                {
                    return section;
                }

                if (position < size)
                {
                    return adapter.GetItem(position - 1);
                }

                position -= size;
            }

            return null;
        }

        public override int Count 
        {
            get 
            { 
                return sections.Values.Sum(adapter => adapter.Count + 1); 
            }
        }

        public override int ViewTypeCount 
        {
            get 
            {
                return 1 + sections.Values.Sum(adapter => adapter.ViewTypeCount);
            }
        }

        public override int GetItemViewType(int position)
        {
            int type = 1;

            foreach (var section in sections.Keys) 
            {
                var adapter = sections[section];
                int size = adapter.Count + 1;

                // check if position inside this section
                if (position == 0)
                {
                    return TYPE_SECTION_HEADER;
                }

                if (position < size)
                {
                    return type + adapter.GetItemViewType(position - 1);
                }

                // otherwise jump into next section
                position -= size;
                type += adapter.ViewTypeCount;
            }

            return -1;
        }

        public override bool AreAllItemsEnabled()
        {
            return false;
        }

        public override bool IsEnabled(int position)
        {
            return (GetItemViewType(position) != TYPE_SECTION_HEADER);
        }

        public override View GetView(int position, View convertView, ViewGroup parent) 
        {
            int sectionnum = 0;

            foreach (var section in sections.Keys) 
            {
                var adapter = sections[section];
                int size = adapter.Count + 1;

                // check if position inside this section
                if (position == 0)
                {
					var view = headers.GetView(sectionnum, convertView, parent);

					switch (section) 
					{
						case "Deposits":
							view.SetBackgroundColor(AppStyles.DepositsHeaderBackgroundColor);
							break;
						case "Loans":
							view.SetBackgroundColor(AppStyles.LoansHeaderBackgroundColor);
							break;
						case "Credit Cards":
							view.SetBackgroundColor(AppStyles.CreditCardsHeaderBackgroundColor);
							break;
						default:
							view.SetBackgroundColor(AppStyles.BarTintColor);
							break;					
					}

					return view;
                }

                if (position < size)
                {
                    return adapter.GetView(position - 1, convertView, parent);
                }

                // otherwise jump into next section
                position -= size;
                sectionnum++;
            }

            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }
    }
}