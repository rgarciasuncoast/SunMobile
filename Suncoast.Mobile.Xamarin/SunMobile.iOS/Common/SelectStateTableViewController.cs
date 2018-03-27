using System;
using System.Collections.Generic;
using SunMobile.Shared.States;
using UIKit;

namespace SunMobile.iOS.Common
{
	public partial class SelectStateTableViewController : BaseTableViewController
	{
		public bool AllowMultipleSelection { get; set; }
		public event Action<List<string>> StatesSelected = delegate{};
		private List<string> _viewModel;
		private List<string> _selectedStates;

		public SelectStateTableViewController(IntPtr handle) : base(handle)
		{			
			_selectedStates = new List<string>();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var leftButton = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
			leftButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetLeftBarButtonItem(leftButton, false);
			leftButton.Clicked += (sender, e) => NavigationController.PopViewController(true);

			var rightButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			leftButton.Clicked += (sender, e) => 
			{
				StatesSelected(_selectedStates);
				NavigationController.PopViewController(true);
			};

			LoadStates();
		}

		private void LoadStates()
		{
			if (_viewModel == null) 
			{
				_viewModel = new List<string>(USStates.USStateList.Values);
			}

			if (_viewModel != null)
			{
				var tableViewSource = new StringListTableViewSource(_viewModel);

				tableViewSource.ItemsSelected += items =>
				{				
					_selectedStates = items;	
				};

				mainTableView.Source = tableViewSource;
				mainTableView.ReloadData();
			}
			else
			{
				_viewModel = null;
			}
		}
	}
}