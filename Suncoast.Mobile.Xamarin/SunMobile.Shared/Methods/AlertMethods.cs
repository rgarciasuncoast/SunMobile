using System;
using System.Threading.Tasks;
using SunMobile.Shared.Utilities.General;


#if __IOS__ || __ANDROID__
#endif

#if __IOS__
using System.Drawing;
using UIKit;
using CoreGraphics;
#endif

#if __ANDROID__
using Android.App;
using Android.Content;
using System.Collections.Generic;
using Android.Widget;
using SunMobile.Shared.Culture;
using Android.Views;
using Android.Runtime;
#endif

namespace SunMobile.Shared.Methods
{
	public static class AlertMethods
	{
#if __IOS__

		public static Task<string> Alert(object viewObject, string title = "SunMobile", string message = "", params string[] buttons)
		{
			var view = (UIView)viewObject;

			GeneralUtilities.CloseKeyboard(view);

			var tcs = new TaskCompletionSource<string>();

			var alert = new UIAlertView();

			alert.Title = title;
			alert.Message = message;

			foreach (string s in buttons)
			{
				alert.AddButton(s);
			}

			alert.Clicked += (sender, e) =>
			tcs.TrySetResult(buttons[e.ButtonIndex]);

			alert.Show();

			return tcs.Task;
		}

        public static Task<string> PopoverActionSheet(UIViewController parentViewController, string title, string message, bool hasCancelbutton, object viewToPresentFrom, params string[] buttons)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.ActionSheet);

            GeneralUtilities.CloseKeyboard(parentViewController);

            var tcs = new TaskCompletionSource<string>();

            foreach (string s in buttons)
            {
                alert.AddAction(UIAlertAction.Create(s, UIAlertActionStyle.Default, (UIAlertAction obj) => tcs.TrySetResult(s)));
            }

            if (hasCancelbutton)
            {
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (UIAlertAction obj) => tcs.TrySetResult("Cancel")));
            }

            if (alert.PopoverPresentationController != null)
            {
                if (viewToPresentFrom != null)
                {
                    if (viewToPresentFrom is UIBarButtonItem)
                    {
                        alert.PopoverPresentationController.BarButtonItem = (UIBarButtonItem)viewToPresentFrom;
                    }
                    else
                    {
                        alert.PopoverPresentationController.SourceView = (UIView)viewToPresentFrom;
                    }
                }
                else
                {
                    alert.PopoverPresentationController.SourceView = parentViewController.View;
                }
            }

            parentViewController.PresentViewController(alert, true, null);    
    
            return tcs.Task;
        }

		public static UIView ShowActivityIndicator(UIView view, bool dimBackground = true)
		{
			GeneralUtilities.CloseKeyboard(view);

			var overlayView = new UIView(view.Bounds);

			view.AddSubview(overlayView);

			var frame = view.Bounds;

			// derive the center x and y
			nfloat centerX = frame.Width / 2;
			nfloat centerY = frame.Height / 2;

			UIActivityIndicatorView activitySpinner;

			//	if (dimBackground)
			//	{
			overlayView.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0xAF);
			activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
			//	}
			/*
				else
				{
					overlayView.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0x4F);
					activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
				}
			*/

			overlayView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

			activitySpinner.Frame = new CGRect
			(
				centerX - (activitySpinner.Frame.Width / 2),
				centerY - activitySpinner.Frame.Height - 20,
				activitySpinner.Frame.Width,
				activitySpinner.Frame.Height
			);

			activitySpinner.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			overlayView.AddSubview(activitySpinner);
			activitySpinner.StartAnimating();

			return overlayView;
		}

		public static void HideActivityIndicator(UIView overlayView)
		{
			overlayView.RemoveFromSuperview();
		}

#endif

#if __ANDROID__

		public static Task<string> Alert(object contextObject, string title = "SunMobile", string message = "", params string[] buttons)
		{
			var context = (Context)contextObject;

			var tcs = new TaskCompletionSource<string>();

			try
			{
				GeneralUtilities.CloseKeyboard(context);

				var alert = new AlertDialog.Builder(context);				
				alert.SetTitle(title);
				alert.SetMessage(message);
                alert.SetItems(buttons, (object sender, DialogClickEventArgs e) => tcs.TrySetResult(buttons[e.Which]));

                if (buttons.Length > 0)
                {
                    alert.SetPositiveButton(buttons[0], (s, ev) =>
                        tcs.TrySetResult(buttons[0]));
                }

                if (buttons.Length > 1)
                {
                    alert.SetNegativeButton(buttons[1], (s, ev) =>
                        tcs.TrySetResult(buttons[1]));
                }

                if (buttons.Length > 2)
                {
                    for (int i = 2; i < buttons.Length; i++)
                    {
                        alert.SetNeutralButton(buttons[i], (s, ev) =>
                        tcs.TrySetResult(buttons[i]));
                    }
                }

                alert.Create();
				alert.Show();
			}
			catch(Exception ex)
			{
				Logging.Logging.Log(ex, "AlertMethods:Alert");
			}

			return tcs.Task;
		}

        public static Task<string> ActionSheet(object contextObject, string title = "SunMobile", params string[] buttons)
        {
            var context = (Context)contextObject;

            var tcs = new TaskCompletionSource<string>();

            try
            {
                GeneralUtilities.CloseKeyboard(context);

                var alert = new AlertDialog.Builder(context);
                //alert.SetTitle(title);        
                alert.SetItems(buttons, (object sender, DialogClickEventArgs e) => tcs.TrySetResult(buttons[e.Which]));
                alert.Create();
                alert.Show();
            }
            catch (Exception ex)
            {
                Logging.Logging.Log(ex, "AlertMethods:ActionSheet");
            }

            return tcs.Task;
        }

        public static Task<Tuple<string, bool>> AlertWithCheckBox(object contextObject, string title, string message, string checkBoxText, bool isCheckBoxRequired, bool checkBoxHidden, params string[] buttons)
		{
			var context = (Context)contextObject;
			var activity = (Activity)contextObject;
            var tcs = new TaskCompletionSource<Tuple<string, bool>>();

			try
			{
				GeneralUtilities.CloseKeyboard(context);

				AlertDialog alertDialog = null;
				var alertView = activity.LayoutInflater.Inflate(Droid.Resource.Layout.AlertDialogWithCheckbox, null);
				var alertDialogBuilder = new AlertDialog.Builder(context);
				alertDialogBuilder.SetView(alertView);

				var txtTitle = alertView.FindViewById<TextView>(Droid.Resource.Id.txtTitle);
				txtTitle.Text = title;
				var txtMessage = alertView.FindViewById<TextView>(Droid.Resource.Id.txtMessage);
				txtMessage.Text = message;
				var checkBoxConfirm = alertView.FindViewById<CheckBox>(Droid.Resource.Id.checkBoxConfirm);
				checkBoxConfirm.Text = checkBoxText;

                if (checkBoxHidden)
                {
                    checkBoxConfirm.Visibility = ViewStates.Gone;
                }

				checkBoxConfirm.CheckedChange += (sender, e) =>
				{
					if (alertDialog != null)
					{
						var button = alertDialog.GetButton((int)DialogButtonType.Positive);

                        if (isCheckBoxRequired)
                        {
                            button.Enabled = e.IsChecked;
                        }
					}
				};

				if (buttons.Length > 0)
				{
					alertDialogBuilder.SetPositiveButton(buttons[0], (s, e) => 
                        tcs.TrySetResult(new Tuple<string, bool>(buttons[0], checkBoxConfirm.Checked)));
				}

				if (buttons.Length > 1)
				{
					alertDialogBuilder.SetNegativeButton(buttons[1], (s, e) =>
                        tcs.TrySetResult(new Tuple<string, bool>(buttons[1], checkBoxConfirm.Checked)));
				}

				if (buttons.Length > 2)
				{
					alertDialogBuilder.SetNeutralButton(buttons[2], (s, e) =>
                        tcs.TrySetResult(new Tuple<string, bool>(buttons[2], checkBoxConfirm.Checked)));
				}

				alertDialogBuilder.Create();
				alertDialog = alertDialogBuilder.Show();

				var btnSubmit = alertDialog.GetButton((int)DialogButtonType.Positive);

                btnSubmit.Enabled = !isCheckBoxRequired;
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "AlertMethods:Alert");
			}

			return tcs.Task;
		}

        public static ProgressBar ShowProgressBar(Context context, ProgressBar progressBar)
		{
			try
			{
				GeneralUtilities.CloseKeyboard(context);

                var layout = (ViewGroup)((Activity)context).FindViewById(Android.Resource.Id.Content).RootView;

                if (progressBar == null)
                {
                    progressBar = new ProgressBar(context, null, Android.Resource.Attribute.ProgressBarStyleLarge);
                    progressBar.Indeterminate = true;
                    var layoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                    var relativeLayout = new RelativeLayout(context);
                    relativeLayout.SetGravity(GravityFlags.Center);
                    relativeLayout.AddView(progressBar);
                    layout.AddView(relativeLayout, layoutParams);
				}
                else
                {
                    progressBar.Visibility = ViewStates.Visible;
                }

				var view = ((Activity)context).FindViewById(Android.Resource.Id.Content);

				if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBeanMr2)
				{
					var dim = new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.Black);
					dim.SetBounds(0, 0, layout.Width, layout.Height);
					dim.SetAlpha(130);
					var overlay = view.Overlay;
					overlay.Add(dim);					
				}
				else
				{
					view.Alpha = 0.75f;
				}
			}			
			catch { }

			return progressBar;
		}

        public static void HideProgressBar(Context context, ProgressBar progressBar)
        {
            try
            {
                if (progressBar != null)
                {
                    progressBar.Visibility = ViewStates.Invisible;

                    var view = ((Activity)context).FindViewById(Android.Resource.Id.Content);

                    if (view != null)
                    {
                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBeanMr2)
                        {
                            var overlay = view.Overlay;

                            if (overlay != null)
                            {
                                overlay.Clear();
                            }
                        }
                        else
                        {
                            view.Alpha = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Logging.Log(ex, "AlertMethods:HideProgressPar");
            }
        }

		public static ProgressDialog ShowActivityIndicator(Context context, string title = "SunMobile", string message = "Loading...")
		{
			ProgressDialog progressDialog = null;

			try 
			{
				GeneralUtilities.CloseKeyboard(context);

                if (message == "Loading...")
                {
                    message = CultureTextProvider.GetMobileResourceText("7845DFA9-A8BB-4F47-81A8-7244D2AC41C4", "6A655E2B-2EF9-4028-8AD0-D45049222F5D", "Loading...");
                }

				progressDialog = new ProgressDialog(context);
				progressDialog.SetTitle(title);
				progressDialog.SetMessage(message);
				progressDialog.Show();
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch{}

			return progressDialog;
		}

		public static void HideActivityIndicator(ProgressDialog progressDialog)
		{
			try 
			{
				if (progressDialog != null)
				{
					progressDialog.Dismiss();
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch{}
		}

		public static void DismissActivityIndicator(ProgressDialog progressDialog)
		{
			try
			{
				if (progressDialog != null)
				{
					progressDialog.Dismiss();
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch { }
		}

#endif
	}
}