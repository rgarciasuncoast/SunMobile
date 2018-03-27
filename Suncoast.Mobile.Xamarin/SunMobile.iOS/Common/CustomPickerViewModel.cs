using System;
using System.Collections.Generic;
using UIKit;

namespace SunMobile.iOS.Common
{
	public class CustomPickerViewModel<TItem> : UIPickerViewModel
	{
		public TItem SelectedItem { get; private set; }

		List<TItem> _items;
		public List<TItem> Items
		{
			get { return _items; }
			set { _items = value; Selected(null, 0, 0); }
		}

		public CustomPickerViewModel()
		{
		}

		public CustomPickerViewModel(List<TItem> items)
		{
			Items = items;
		}

		public override nint GetRowsInComponent(UIPickerView picker, nint component)
		{
			if (NoItem())
			{
				return 1;
			}

			return Items.Count;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			if (NoItem(Convert.ToInt32(row)))
			{
				return "";
			}

			var item = Items[Convert.ToInt32(row)];

			return GetTitleForItem(item);
		}

		public override void Selected(UIPickerView picker, nint row, nint component)
		{
			if (NoItem(Convert.ToInt32(row)))
			{
				SelectedItem = default(TItem);
			} 
			else
			{
				SelectedItem = Items[Convert.ToInt32(row)];
			}
		}

		public override nint GetComponentCount(UIPickerView picker)
		{
			return 1;
		}

		public virtual string GetTitleForItem(TItem item)
		{
			return item.ToString();
		}

		bool NoItem(int row = 0)
		{
			return Items == null || row >= Items.Count;
		}
	}
}