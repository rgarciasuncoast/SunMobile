using System;
using UIKit;

namespace SunMobile.iOS.Common
{
	public partial class CustomPickerViewController : BaseViewController
	{
		public CustomPickerViewModel<string> ViewModel { get; set; }
		public string SelectItem { get; set; }
		public event Action<string> ItemSelected = delegate{};

		public CustomPickerViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			pickerView.Model = ViewModel;
			pickerView.ShowSelectionIndicator = true;

			if (!string.IsNullOrEmpty(SelectItem))
			{
				var index = ViewModel.Items.IndexOf(SelectItem);
				pickerView.Select(index, 0, false);
			}

			var rightButton = new UIBarButtonItem("Select", UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			rightButton.Clicked += (sender, e) => 
			{
				ItemSelected(ViewModel.SelectedItem);
				NavigationController.PopViewController(false);
			};
		}
	}
}