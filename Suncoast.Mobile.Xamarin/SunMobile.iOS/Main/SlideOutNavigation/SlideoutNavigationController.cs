using System;
using UIKit;
using CoreGraphics;

namespace SunMobile.iOS.Main
{
	public class SlideoutNavigationController : UIViewController
	{
		private readonly static Action EmptyAction = () => { };
		public bool CanClose { get; set; }
		public bool IsOpen { get; private set; }
		public float OpenAnimationDuration { get; set; }
		public float VelocityTrigger { get; set; }
		protected UIView ContainerView { get; private set; }
		public bool PanGestureEnabled { get; set; }
		public bool EnableInteractivePopGestureRecognizer { get; set; }
		public bool ShadowEnabled { get; set; }
		protected UIViewAnimationOptions AnimationOption { get; set; }
		protected float SlideHalfwayOffset { get; set; }

		private UIViewController _mainViewController;
		private MenuNavigationController _menuViewController;
		private UITapGestureRecognizer _tapGesture;
		private UIPanGestureRecognizer _panGesture;
		private nfloat _panTranslationX;
		//private float _slideHandleHeight;
		private float _menuWidth;
		private SlideHandle _slideHandle;
		private bool _isTabletInLandscape;

		public float MenuWidth
		{
			get { return _menuWidth; }
			set
			{
				_menuWidth = value;
				if (_menuViewController != null)
				{
					var frame = _menuViewController.View.Frame;
					frame.Width = value; 
					_menuViewController.View.Frame = frame;
				}
			}
		}

		public SlideHandle SlideHandle
		{
			get { return _slideHandle; }
			set
			{
				_slideHandle = value;

				/*
				if (value == SlideHandle.None)
				{
					_slideHandleHeight = 0;
				}
				else if (value == SlideHandle.NavigationBar)
				{
					_slideHandleHeight = 44f + 20f;
				}
				else if (value == SlideHandle.Full)
				{
					_slideHandleHeight = float.MaxValue;
				}
				*/
			}
		}

		public MenuNavigationController MenuViewController
		{
			get { return _menuViewController; }
			set 
			{ 
				if (IsViewLoaded)
				{
					SetMenuViewController(value, false);
				}
				else
				{
					_menuViewController = value;
				}
			}
		}

		public UIViewController MainViewController
		{
			get { return _mainViewController; }
			set 
			{
				if (IsViewLoaded)
				{
					SetMainViewController(value, false);
				}
				else
				{
					_mainViewController = value;
				}
			}
		}

		public SlideoutNavigationController()
		{
			OpenAnimationDuration = 0.3f;
			PanGestureEnabled = true;
			AnimationOption = UIViewAnimationOptions.CurveEaseInOut;
			SlideHandle = SlideHandle.Full;
			EnableInteractivePopGestureRecognizer = true;
			SlideHalfwayOffset = 120f;
			VelocityTrigger = 800f;
			MenuWidth = 290f;
			ShadowEnabled = true;
			CanClose = true;

			ContainerView = new UIView();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			IsOpen = true;

			var containerFrame = View.Bounds;
			containerFrame.X = View.Bounds.Width;
			ContainerView.Frame = containerFrame;
			ContainerView.BackgroundColor = UIColor.White;
			ContainerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

			View.BackgroundColor = UIColor.White;

			_tapGesture = new UITapGestureRecognizer();
			_tapGesture.AddTarget (() => Close(true));
			_tapGesture.NumberOfTapsRequired = 1;

			_panGesture = new UIPanGestureRecognizer 
			{
				Delegate = new PanDelegate(this),
				MaximumNumberOfTouches = 1,
				MinimumNumberOfTouches = 1
			};

			_panGesture.AddTarget (() => Pan(ContainerView));
			ContainerView.AddGestureRecognizer(_panGesture);

			if (_menuViewController != null)
			{
				SetMenuViewController(_menuViewController, false);
			}

			if (_mainViewController != null)
			{
				SetMainViewController(_mainViewController, false);
			}

			if (ShadowEnabled)
			{
				DrawShadow();
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			SetOrientation();
		}

		private void DrawShadow()
		{
			if (ShadowEnabled)
			{
				ContainerView.Layer.ShadowOffset = new CGSize(-5, 0);
				ContainerView.Layer.ShadowPath = UIBezierPath.FromRect(ContainerView.Bounds).CGPath;
				ContainerView.Layer.ShadowRadius = 3.0f;
				ContainerView.Layer.ShadowColor = UIColor.Black.CGColor;
			}
			else
			{
				ContainerView.Layer.ShadowOffset = new CGSize(-1, 0);
				ContainerView.Layer.ShadowPath = UIBezierPath.FromRect(ContainerView.Bounds).CGPath;
				ContainerView.Layer.ShadowRadius = 1.0f;
				ContainerView.Layer.ShadowColor = UIColor.Black.CGColor;
			}
		}

		public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate(toInterfaceOrientation, duration);

			CanClose = true;
			Close(false);
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);

			SetOrientation();
		}

		private void SetOrientation()
		{
			_isTabletInLandscape = (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad && (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight));

			if (_isTabletInLandscape)
			{
				PanGestureEnabled = false;
				CanClose = false;
				Open(false);

				ShadowEnabled = false;
				DrawShadow();
			}
			else
			{
				PanGestureEnabled = true;
				CanClose = true;
				Close(false);

				ShadowEnabled = true;
				DrawShadow();
			}
		}

		private void Animate(UIView mainView, nfloat percentage)
		{
			if (percentage > 1)
			{
				percentage = 1;
			}

			if (ShadowEnabled)
			{
				if (percentage <= 0)
				{
					mainView.Layer.ShadowOpacity = 0;
				}
				else
				{
					ContainerView.Layer.ShadowOpacity = 0.3f;
				}
			}

			if (_isTabletInLandscape)
			{
				var x = View.Bounds.X + (MenuWidth * percentage);
				mainView.Frame = new CGRect(x, mainView.Frame.Y, View.Frame.Width - x, View.Frame.Height);
			}
			else
			{
				var x = View.Bounds.X + (MenuWidth * percentage);
				mainView.Frame = new CGRect(x, mainView.Frame.Y, View.Frame.Width, View.Frame.Height);
			}			
		}

		private void Pan(UIView view)
		{
			if (CanClose)
			{
				if (_panGesture.State == UIGestureRecognizerState.Began)
				{
					if (!IsOpen)
					{
						if (_menuViewController != null)
						{
							_menuViewController.ViewWillAppear(true);
						}
					}
				} 
				else if (_panGesture.State == UIGestureRecognizerState.Changed)
				{
					_panTranslationX = _panGesture.TranslationInView(View).X;
					nfloat total = MenuWidth;
					nfloat numerator = IsOpen ? MenuWidth + _panTranslationX : _panTranslationX;
					nfloat percentage = numerator / total;

					if (percentage < 0)
					{
						percentage = 0;
					}

					Action animation = () => Animate(ContainerView, percentage);
					UIView.Animate(0.01f, 0, UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.AllowUserInteraction, animation, EmptyAction);
				} 
				else if (_panGesture.State == UIGestureRecognizerState.Ended || _panGesture.State == UIGestureRecognizerState.Cancelled)
				{
					nfloat velocity = _panGesture.VelocityInView(View).X;
					nfloat total = MenuWidth;
					nfloat numerator = IsOpen ? MenuWidth + _panTranslationX : _panTranslationX;
					nfloat percentage = numerator / total;
					var animationTime = Math.Min(1 / (Math.Abs(velocity) / 100), OpenAnimationDuration);

					if (IsOpen)
					{
						if (percentage > .66f && velocity > -VelocityTrigger)
						{
							Action animation = () => Animate(ContainerView, 1);
							UIView.Animate(OpenAnimationDuration, 0, AnimationOption, animation, EmptyAction);
						}
						else
						{
							Close(true, (nfloat)animationTime);
						}
					} 
					else
					{
						if (percentage < .33f && velocity < VelocityTrigger)
						{
							Action animation = () => Animate(ContainerView, 0);
							UIView.Animate(OpenAnimationDuration, 0, AnimationOption, animation, EmptyAction);
						}
						else
						{
							Open(true, (nfloat)animationTime);
						}
					}
				}
			}
		}

		public void ToggleMenu(bool animated)
		{
			if (IsOpen)
			{
				Close(animated);
                MenuViewController.MenuTableViewController.RemoveAccessabilityLabels();
			}
			else
			{
				Open(animated);
                MenuViewController.MenuTableViewController.SetAccessabilityLabels();
			}
		}

		public void Open(bool animated)
		{
			Open(animated, OpenAnimationDuration);
		}

		private void Open(bool animated, nfloat animationTime)
		{
			if (IsOpen)
			{
				return;
			}

			if (_menuViewController != null)
			{
				_menuViewController.ViewWillAppear(animated);
			}

			Action animation = () => Animate(ContainerView, 1);

			Action completion = () =>
			{
				IsOpen = true;

				//ContainerView.AddGestureRecognizer(_tapGesture);

				if (_menuViewController != null)
				{
					_menuViewController.ViewDidAppear(animated);
				}
			};

			if (_menuViewController?.CurrentViewController != null)
			{
				_menuViewController.CurrentViewController.View.UserInteractionEnabled = _isTabletInLandscape;
			}

			/*
			if (ContainerView.Subviews.Length > 0)
			{
				ContainerView.Subviews[0].UserInteractionEnabled = _isTabletInLandscape;
			}
			*/

			if (animated)
			{
				UIView.Animate(animationTime, 0, AnimationOption, animation, completion);
			}
			else
			{
				animation();
				completion();
			}
		}

		public void OpenAndDenyClose(bool animated)
		{
			CanClose = false;
			Open(animated, OpenAnimationDuration);
		}

		public void Close(bool animated)
		{
			if (CanClose)
			{
				Close(animated, OpenAnimationDuration);
			}
		}

		private void Close(bool animated, nfloat animationTime)
		{
			if (!IsOpen)
			{
				return;
			}

			if (_menuViewController != null)
			{
				_menuViewController.ViewWillDisappear(animated);
			}

			Action animation = () => Animate(ContainerView, 0);

			Action completion = () =>
			{
				IsOpen = false;

				if (_menuViewController?.CurrentViewController != null)
				{
					_menuViewController.CurrentViewController.View.UserInteractionEnabled = true;
				}

				/*
				if (ContainerView.Subviews.Length > 0)
				{
					ContainerView.Subviews[0].UserInteractionEnabled = true;
				}
				*/

				//ContainerView.RemoveGestureRecognizer(_tapGesture);

				if (_menuViewController != null)
				{
					_menuViewController.ViewDidDisappear(animated);
				}
			};

			if (animated)
			{
				UIView.Animate(animationTime, 0, AnimationOption, animation, completion);
			}
			else
			{
				animation();
				completion();
			}
		}

		public void SetMainViewController(UIViewController viewController, bool animated)
		{
			// This will only happen once...
			if (ContainerView.Superview == null)
			{
				var containerFrame = View.Bounds;
				containerFrame.X = View.Bounds.Width;
				ContainerView.Frame = containerFrame;

				View.AddSubview(ContainerView);
				var updatedMenuFrame = new CGRect(View.Bounds.Location, new CGSize(MenuWidth, View.Bounds.Height));
				UIView.Animate(OpenAnimationDuration, 0, UIViewAnimationOptions.BeginFromCurrentState | AnimationOption,
               	() => MenuViewController.View.Frame = updatedMenuFrame, null);

				if (_menuViewController != null)
				{
					_menuViewController.View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
				}
			}

			AddChildViewController(viewController);
			viewController.View.Frame = ContainerView.Bounds;
			ContainerView.AddSubview(viewController.View);

			if (_mainViewController != null && viewController != _mainViewController)
			{
				_mainViewController.RemoveFromParentViewController();
				_mainViewController.View.RemoveFromSuperview();
				_mainViewController.DidMoveToParentViewController(null);
			}

			Close(animated);
			_mainViewController = viewController;
		}

		public void SetMenuViewController(MenuNavigationController viewController, bool animated)
		{
			AddChildViewController(viewController);
			var resizing = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			var width = View.Bounds.Width;

			if (MainViewController != null)
			{
				width = MenuWidth;
				resizing = UIViewAutoresizing.FlexibleHeight;
			}

			viewController.View.Frame = new CGRect(View.Bounds.Location, new CGSize(width, View.Bounds.Height));
			viewController.View.AutoresizingMask = resizing;
			View.InsertSubview(viewController.View, 0);

			if (_menuViewController != null && viewController != _menuViewController)
			{
				_menuViewController.RemoveFromParentViewController();
				_menuViewController.View.RemoveFromSuperview();
				_menuViewController.DidMoveToParentViewController(null);
			}

			_menuViewController = viewController;
		}

		public void SetCurrentViewController(UIViewController viewController)
		{
			_menuViewController.SetCurrentViewController(viewController);
		}

		public override bool ShouldAutorotate()
		{
			return _menuViewController.ShouldAutorotate();         
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{            
			return _menuViewController.GetSupportedInterfaceOrientations();
		}

		private class PanDelegate : UIGestureRecognizerDelegate
		{
			private readonly SlideoutNavigationController _controller;

			public PanDelegate(SlideoutNavigationController controller)
			{
				_controller = controller;
			}

			public override bool ShouldBegin(UIGestureRecognizer recognizer)
			{
				if (!_controller.PanGestureEnabled)
				{
					return false;
				}

				if (_controller.IsOpen)
				{
					return true;
				}

				var rec = (UIPanGestureRecognizer)recognizer;
				var velocity = rec.VelocityInView(_controller.ContainerView);

				return Math.Abs(velocity.X) > Math.Abs(velocity.Y);
			}

			public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
			{
				if (!_controller.PanGestureEnabled)
				{
					return false;
				}

				if (_controller.IsOpen)
				{
					return true;
				}

				var locationInView = touch.LocationInView(_controller.ContainerView);

				if (locationInView.X <= 80)
				{
					return true;
				}

				return false;
			}
		}
	}
}