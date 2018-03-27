using System;
using System.Collections.Generic;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts;
using SunMobile.iOS.Common;
using SunMobile.Shared.Data;
using SunMobile.Shared.Methods;

namespace SunMobile.iOS.Profile
{
	public partial class AlertSettingTableViewController : BaseTableViewController
	{
		public AccountSpecificAlertModel Model { get; set; }
		public event Action<AccountSpecificAlertModel> ItemChanged = delegate{};
		private List<AlertSetting> _itemsChanged;
		private bool _isDirty;

		public AlertSettingTableViewController(IntPtr handle) : base(handle)
		{
			_itemsChanged = new List<AlertSetting>();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Title = Model.DisplayText;

			LoadSettings();
		}

		public override async void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			if (_isDirty && (IsMovingFromParentViewController || IsBeingDismissed))
			{
				ShowActivityIndicator();

				var methods = new MessagingMethods();
				await methods.SaveAlertSettings(_itemsChanged, Model, View);

				HideActivityIndicator();

				ItemChanged(Model);
			}
		}

		private void LoadSettings()
		{	
			if (Model != null)
			{
				var methods = new MessagingMethods();
				var listViewItems = methods.LoadAlertSettingsFromModel(Model);

				var tableViewSource = new AlertSettingTableViewSource(listViewItems);

				tableViewSource.ItemSelected += item =>
				{
					var controller = AppDelegate.StoryBoard.InstantiateViewController("AlertSettingDetailTableViewController") as AlertSettingDetailTableViewController;
					controller.Model = (AvailableBalanceThresholdAlertModel)item.Data;

					controller.ItemChanged += async setting =>
					{
						_isDirty = true;

						// Update the model
						Model.AvailableBalaceThresholdAlertSettings.Enabled = setting.Value;
						Model.AvailableBalaceThresholdAlertSettings.ThreshHoldAmount = setting.Amount;

						LoadSettings();

						// Update the database
						var request = new AvailableBalanceThresholdSettingsUpdateRequest 
						{
							Suffix = Model.AccountId,
							AccountSettingType = Model.AccountSettingType,
							Value = setting.Value,
							ThresholdAmount = setting.Amount
						};

						await methods.UpdateAvailableBalanceAlertSettings(request, View);
					};

					NavigationController.PushViewController(controller, true);
				};

				tableViewSource.ItemsChanged += async items =>
				{
					_itemsChanged = items;	
					_isDirty = _itemsChanged.Count > 0;

					if (_isDirty)
					{
						await methods.SaveAlertSettings(_itemsChanged, Model, View, false);
					}
				};

				tableViewMain.Source = tableViewSource;
				tableViewMain.ReloadData();
			}
		}
	}
}