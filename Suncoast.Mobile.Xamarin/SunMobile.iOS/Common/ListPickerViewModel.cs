using UIKit;
using System.Collections.Generic;
using System;
using System.Drawing;

namespace SunMobile.iOS.Common
{
	public class ListPickerViewModel<TItem> : UIPickerViewModel
	{
		private IList<TItem> _items;
		private float _fontSize;
		public TItem SelectedItem { get; private set; }
		public IList<TItem> Items
		{
			get { return _items; }
			set { _items = value; Selected(null, 0, 0); }
		}

		public ListPickerViewModel()
		{
			_fontSize = 17f;
		}

		public ListPickerViewModel(IList<TItem> items, float fontSize = 17f)
		{
			Items = items;
			_fontSize = fontSize;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return NoItem() ? 1 : Items.Count;

		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			var label = new UILabel(new RectangleF(0, 0, (float)pickerView.Frame.Width, 20f));
			label.TextColor = UIColor.Black;
			label.Font = UIFont.SystemFontOfSize(_fontSize);
			label.TextAlignment = UITextAlignment.Center;
			label.Text = Items[(int)row].ToString();

			return label;
		}

		public override string GetTitle(UIPickerView pickerView, nint row, nint component)
		{
			if (NoItem((int)row))
			{
				return "";
			}

			var item = Items[(int)row];

			return GetTitleForItem(item);
		}

		public override void Selected(UIPickerView pickerView, nint row, nint component)
		{
			SelectedItem = NoItem((int)row) ? default(TItem) : Items[(int)row];
		}

		public override nint GetComponentCount(UIPickerView pickerView)
		{
			return 1;
		}

		public virtual string GetTitleForItem(TItem item)
		{
			return item.ToString();
		}

		private bool NoItem(int row = 0)
		{
			return Items == null || row >= Items.Count;
		}
	}
}