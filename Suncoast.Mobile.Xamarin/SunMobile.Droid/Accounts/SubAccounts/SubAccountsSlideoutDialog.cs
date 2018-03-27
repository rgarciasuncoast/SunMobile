using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.App;
using System;
using Android.Views.Animations;

namespace SunMobile.Droid.Accounts.SubAccounts
{
	public class SubAccountsSlideoutDialog : DialogFragment
	{		
        public event Action<bool> StartSubAccounts = delegate {};
        private ImageView imgSmartChecking;
		private Button btnClose;
        private Button btnOpenRocketChecking;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            var view = inflater.Inflate(Resource.Layout.SubAccountsSlideoutDialog, container, true);

			var attributes = Dialog.Window.Attributes;
			attributes.Gravity = GravityFlags.Bottom;
			Dialog.Window.Attributes = attributes;

            imgSmartChecking = view.FindViewById<ImageView>(Resource.Id.imgSmartChecking);
            imgSmartChecking.Visibility = ViewStates.Gone;

			btnOpenRocketChecking = view.FindViewById<Button>(Resource.Id.btnOpenRocketChecking);
			btnOpenRocketChecking.Click += (sender, e) => 
            {
                CloseSlideout();
                StartSubAccounts(true);
                Dismiss();  
            };

            btnClose = view.FindViewById<Button>(Resource.Id.btnClose);
            btnClose.Click += (sender, e) =>
            {
                CloseSlideout();
                Dismiss();
            };

            OpenSlideout();

			return view;
		}

		private void OpenSlideout()
		{
            imgSmartChecking.Visibility = ViewStates.Visible;
			ScaleView(imgSmartChecking, .1f, 1f);			
		}

		private void CloseSlideout()
		{
			ScaleView(imgSmartChecking, 1, .1f);
		}

		public void ScaleView(View view, float startScale, float endScale)
		{
			Animation anim = new ScaleAnimation(
				1f, 1f, // Start and end values for the X axis scaling
				startScale, endScale, // Start and end values for the Y axis scaling
				Dimension.RelativeToSelf, 0f, // Pivot point of X scaling
				Dimension.RelativeToSelf, 1f); // Pivot point of Y scaling
			anim.FillAfter = true; // Needed to keep the result of the animation
			anim.Duration = 250;
			view.StartAnimation(anim);
		}

		public override void OnResume()
		{
			// Auto size the dialog based on it's contents
			Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

			// Make sure there is no background behind our view
			Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

			// Disable standard dialog styling/frame/theme: our custom view should create full UI
			SetStyle(DialogFragmentStyle.NoFrame, Android.Resource.Style.Theme);

			base.OnResume();
		}
	}
}