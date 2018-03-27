using System;
using System.Drawing;
using Foundation;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using UIKit;

namespace SunMobile.iOS.Accounts.SubAccounts
{
    public class SubAccountsBaseContentViewController : UIViewController
    {
        public int PageIndex { get; set; }
        public bool IsConfirmationPage { get; set; }
        protected UIView ActivityIndicator;
        protected UIView ViewToCenterOnKeyboardShown;
        protected static readonly string cultureViewId = "C717F806-AEDA-4F25-A916-4FA0FC3EA842";

        public SubAccountsBaseContentViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            try
            {
                SetCultureConfiguration();
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "SubAccountsBaseContentViewController:ViewDidLoad");
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            try
            {
                ParentViewController.ParentViewController.NavigationItem.SetHidesBackButton(true, false);
            }
            catch { }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            HideActivityIndicator();
        }

        public virtual void SetCultureConfiguration()
        {
        }

        public void ShowActivityIndicator()
        {
            try
            {
                ActivityIndicator = AlertMethods.ShowActivityIndicator(NavigationController.View);
            }
            catch { }
        }

        public void HideActivityIndicator()
        {
            try
            {
                AlertMethods.HideActivityIndicator(ActivityIndicator);
            }
            catch { }
        }

        public virtual bool HandlesKeyboardNotifications()
        {
            return false;
        }

        protected virtual void RegisterForKeyboardNotifications()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        protected virtual UIView KeyboardGetActiveView()
        {
            return View.FindFirstResponder();
        }

        private void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded) return;

            //Check if the keyboard is becoming visible
            var visible = notification.Name == UIKeyboard.WillShowNotification;

            //Start an animation, using values from the keyboard
            UIView.BeginAnimations("AnimateForKeyboard");
            UIView.SetAnimationBeginsFromCurrentState(true);
            UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
            UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

            //Pass the notification, calculating keyboard height, etc.
            bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
            var keyboardFrame = visible
                                    ? UIKeyboard.FrameEndFromNotification(notification)
                                    : UIKeyboard.FrameBeginFromNotification(notification);

            OnKeyboardChanged(visible, (float)(landscape ? keyboardFrame.Width : keyboardFrame.Height));

            //Commit the animation
            UIView.CommitAnimations();
        }

        protected virtual void OnKeyboardChanged(bool visible, float keyboardHeight)
        {
            var activeView = ViewToCenterOnKeyboardShown ?? KeyboardGetActiveView();

            if (activeView == null)
            {
                return;
            }

            var scrollView = activeView.FindSuperviewOfType(View, typeof(UIScrollView)) as UIScrollView;

            if (scrollView == null)
            {
                return;
            }

            if (!visible)
            {
                RestoreScrollPosition(scrollView);
            }
            else
            {
                CenterViewInScroll(activeView, scrollView, keyboardHeight);
            }
        }

        protected virtual void CenterViewInScroll(UIView viewToCenter, UIScrollView scrollView, float keyboardHeight)
        {
            var contentInsets = new UIEdgeInsets(0.0f, 0.0f, keyboardHeight, 0.0f);
            scrollView.ContentInset = contentInsets;
            scrollView.ScrollIndicatorInsets = contentInsets;

            // Position of the active field relative isnside the scroll view
            var relativeFrame = viewToCenter.Superview.ConvertRectToView(viewToCenter.Frame, scrollView);

            bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
            var spaceAboveKeyboard = (landscape ? scrollView.Frame.Width : scrollView.Frame.Height) - keyboardHeight;

            // Move the active field to the center of the available space
            //var offset = relativeFrame.Y - (spaceAboveKeyboard - viewToCenter.Frame.Height) / 2;
            var offset = relativeFrame.Y - (spaceAboveKeyboard - viewToCenter.Frame.Height);
            scrollView.ContentOffset = new PointF(0, (float)offset);
        }

        protected virtual void RestoreScrollPosition(UIScrollView scrollView)
        {
            scrollView.ContentInset = UIEdgeInsets.Zero;
            scrollView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
        }

        protected void DismissKeyboardOnBackgroundTap()
        {
            // Add gesture recognizer to hide keyboard
            var tap = new UITapGestureRecognizer { CancelsTouchesInView = false };
            tap.AddTarget(() => View.EndEditing(true));
            View.AddGestureRecognizer(tap);
        }
    }

    public static class ViewExtensions
    {
        /// <summary>
        /// Find the first responder in the <paramref name="view"/>'s subview hierarchy
        /// </summary>
        /// <param name="view">
        /// A <see cref="UIView"/>
        /// </param>
        /// <returns>
        /// A <see cref="UIView"/> that is the first responder or null if there is no first responder
        /// </returns>
        public static UIView FindFirstResponder(this UIView view)
        {
            if (view.IsFirstResponder)
            {
                return view;
            }
            foreach (UIView subView in view.Subviews)
            {
                var firstResponder = subView.FindFirstResponder();
                if (firstResponder != null)
                    return firstResponder;
            }
            return null;
        }

        /// <summary>
        /// Find the first Superview of the specified type (or descendant of)
        /// </summary>
        /// <param name="view">
        /// A <see cref="UIView"/>
        /// </param>
        /// <param name="stopAt">
        /// A <see cref="UIView"/> that indicates where to stop looking up the superview hierarchy
        /// </param>
        /// <param name="type">
        /// A <see cref="Type"/> to look for, this should be a UIView or descendant type
        /// </param>
        /// <returns>
        /// A <see cref="UIView"/> if it is found, otherwise null
        /// </returns>
        public static UIView FindSuperviewOfType(this UIView view, UIView stopAt, Type type)
        {
            if (view.Superview != null)
            {
                if (type.IsAssignableFrom(view.Superview.GetType()))
                {
                    return view.Superview;
                }

                if (view.Superview != stopAt)
                    return view.Superview.FindSuperviewOfType(stopAt, type);
            }

            return null;
        }
    }
}