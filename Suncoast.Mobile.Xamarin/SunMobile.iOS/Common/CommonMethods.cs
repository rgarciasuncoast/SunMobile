using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using SunMobile.iOS.Profile;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Common
{
	public static class CommonMethods
	{
		public static void AddBottomToolbar(UIViewController viewController)
		{
			try 
			{
				if (SessionSettings.Instance.IsAuthenticated && SessionSettings.Instance.ShowPasswordReminder && !viewController.GetType().ToString().Contains("CameraViewController"))
				{
					bool isToolBarSet = false;

					var infoButton = new UIBarButtonItem(UIImage.FromBundle("toolbar_info"), UIBarButtonItemStyle.Plain, (s, e) =>
					{
						var passwordReminderTableViewController = AppDelegate.StoryBoard.InstantiateViewController("PasswordReminderTableViewController") as PasswordReminderTableViewController;
						viewController.NavigationController.PushViewController(passwordReminderTableViewController, true);
					});

					var spacerButton = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 };

					/*
					var logoutButton = new UIBarButtonItem(UIImage.FromBundle("toolbar_signout"), UIBarButtonItemStyle.Plain, async (s, e) =>
					{
						var response = await AlertMethods.Alert(viewController.View, "Confirm", "Are you sure you want to Logout", "Yes, Logout", "No, Cancel");

						if (response == "Yes, Logout")
						{
							AppDelegate.MenuNavigationController.SignOut();
						}
					});				
					*/

					if (!isToolBarSet)
					{
						viewController.SetToolbarItems(new [] 
						{	
							spacerButton,
							infoButton					
						}, false);

						viewController.NavigationController.SetToolbarHidden(false, false);
					}
				}
				else
				{
					viewController.NavigationController.SetToolbarHidden(true, false);
				}
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "CommonMethods:AddBottomToolbar");
			}
		}

		public static void SetNavigationMenuButton(UIViewController viewController)
		{
            try
            {
                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad && (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight))
                {
                    var hasBackButton = viewController.NavigationController.ViewControllers.Length;

                    if (hasBackButton < 2)
                    {
                        var blankButtonItem = new UIBarButtonItem("", UIBarButtonItemStyle.Plain, null);
                        viewController.NavigationItem.LeftBarButtonItem = blankButtonItem;
                    }
                }
                else
                {
                    var contentsButton = AppDelegate.DefaultDetailView.NavigationItem.LeftBarButtonItem;
                    viewController.NavigationItem.LeftItemsSupplementBackButton = true;
                    viewController.NavigationItem.LeftBarButtonItem = contentsButton;
                }
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "CommonMethods:SetNavigationMenuButton");
            }
		}

		public static void HideBottomNavigationToolbarIfViewToolbarExists(UIViewController viewController)
		{
			foreach (var subView in viewController.View.Subviews)
			{
				if (subView is UIToolbar)
				{
					viewController.NavigationController.SetToolbarHidden(true, true);
					break;
				}
			}
		}

		public static void CreateTextViewWithPlaceHolder(UITextView sourceTextView, string placeHolderText)
		{
			sourceTextView.Text = placeHolderText;
			sourceTextView.TextColor = UIColor.LightGray;

			sourceTextView.Started += (sender, e) =>
			{
				if (sourceTextView.Text == placeHolderText)
				{
					sourceTextView.Text = string.Empty;
					sourceTextView.TextColor = UIColor.Black;
				}

				sourceTextView.BecomeFirstResponder();
			};

			sourceTextView.Ended += (sender, e) =>
			{
				if (sourceTextView.Text == string.Empty)
				{
					sourceTextView.Text = placeHolderText;
					sourceTextView.TextColor = UIColor.LightGray;
				}

				sourceTextView.ResignFirstResponder();
			};
		}

		public static UIPickerView CreateDropDownFromTextField(UITextField sourceTextField, List<string> items, float fontSize = 17f)
		{
			sourceTextField.ShouldChangeCharacters = (textField, range, replacementString) => false;
			var imageView = new UIImageView(UIImage.FromBundle("downArrow.png"));
			sourceTextField.RightView = imageView;
			sourceTextField.RightView.ContentMode = UIViewContentMode.ScaleAspectFit;
			sourceTextField.RightView.ClipsToBounds = true;
			sourceTextField.RightViewMode = UITextFieldViewMode.Always;
			// Hide the cursor
			sourceTextField.ValueForKey(new NSString(@"textInputTraits")).SetValueForKey(UIColor.Clear,new NSString(@"insertionPointColor"));

			// Add the picker
			var picker = new UIPickerView();
			picker.Model = new ListPickerViewModel<string>(items, fontSize);
			picker.ShowSelectionIndicator = true;

			var toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Default;
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			var doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s,e) =>
			{
				var pickerItem = picker.Model as ListPickerViewModel<string>;
				sourceTextField.Text = pickerItem.SelectedItem; 
				sourceTextField.ResignFirstResponder();
			});

			toolbar.SetItems(new [] {doneButton}, true);

			sourceTextField.InputView = picker;
			sourceTextField.InputAccessoryView = toolbar;

			sourceTextField.TouchDown += (sender, e) => 
			{
				var field = (UITextField)sender;
				var index = items.FindIndex(a => a == field.Text);
				picker.Select(index, 0, true);
			};

			return picker;
		}

		public static void RemoveDropDownFromTextField(UITextField sourceTextField)
		{
			sourceTextField.ShouldChangeCharacters = (textField, range, replacementString) => true;
			sourceTextField.RightView = null;
			sourceTextField.ValueForKey(new NSString(@"textInputTraits")).SetValueForKey(UIColor.Black, new NSString(@"insertionPointColor"));
			sourceTextField.InputView = null;
			sourceTextField.InputAccessoryView = null;
		}

		public static UIPickerView CreateDropDownFromTextFieldWithDelegate(UITextField sourceTextField, List<string> items, Action<string> onChange, float fontSize = 17f)
		{
            var picker = new UIPickerView();

            try
            {
                sourceTextField.ShouldChangeCharacters = (textField, range, replacementString) => false;
                var imageView = new UIImageView(UIImage.FromBundle("downArrow.png"));
                sourceTextField.RightView = imageView;
                sourceTextField.RightView.ContentMode = UIViewContentMode.ScaleAspectFit;
                sourceTextField.RightView.ClipsToBounds = true;
                sourceTextField.RightViewMode = UITextFieldViewMode.Always;
                // Hide the cursor
                sourceTextField.ValueForKey(new NSString(@"textInputTraits")).SetValueForKey(UIColor.Clear, new NSString(@"insertionPointColor"));

                // Add the picker			
                picker.Model = new ListPickerViewModel<string>(items, fontSize);
                picker.ShowSelectionIndicator = true;

                var toolbar = new UIToolbar();
                toolbar.BarStyle = UIBarStyle.Default;
                toolbar.Translucent = true;
                toolbar.SizeToFit();

                var doneButtonText = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "7486a25e-86cb-45fd-8ffa-13ae76aa95f9", "Done");
                var doneButton = new UIBarButtonItem(doneButtonText, UIBarButtonItemStyle.Done, (s, e) =>
                {
                    var pickerItem = picker.Model as ListPickerViewModel<string>;
                    sourceTextField.Text = pickerItem.SelectedItem;
                    sourceTextField.ResignFirstResponder();

                    onChange(sourceTextField.Text);
                });

                toolbar.SetItems(new[] { doneButton }, true);

                sourceTextField.InputView = picker;
                sourceTextField.InputAccessoryView = toolbar;

                sourceTextField.TouchDown += (sender, e) =>
                {
                    var field = (UITextField)sender;
                    var index = items.FindIndex(a => a == field.Text);
                    picker.Select(index, 0, true);
                };
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "CommonMethods:CreateDropDownFromTextFieldWithDelegate");
            }

			return picker;
		}

		public static void PopToRootIfOniPad()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{
				AppDelegate.MenuNavigationController.PopToRootViewController(false);
			}
		}

        public static void SizeToFitVertically(UILabel label)
        {
            float maxHeight = 10000;

            var size = label.Text.StringSize(label.Font, new CGSize(label.Frame.Size.Width, maxHeight), UILineBreakMode.WordWrap);
            var labelFrame = label.Frame;
            labelFrame.Size = new CGSize(label.Frame.Size.Width, size.Height);
            label.Frame = labelFrame;
        }

		public static void SizeLabelToMaxNumberOfLines(UILabel label, int maxNumberOfLines)
		{
			const int LINE_HEIGHT = 19;

			label.LineBreakMode = UILineBreakMode.WordWrap;
			label.Lines = 0;
			var size = label.SizeThatFits(new CGSize(label.Frame.Size.Width, LINE_HEIGHT * maxNumberOfLines));
			var labelFrame = label.Frame;
			var rect = labelFrame.Size;
			rect.Height = size.Height;
			labelFrame.Size = rect;
			label.Frame = labelFrame;

			var lines = (int)(labelFrame.Height / LINE_HEIGHT) + 1;

			if (lines > maxNumberOfLines)
			{
				lines = maxNumberOfLines;
			}

			label.Lines = lines;
		}
	}
}