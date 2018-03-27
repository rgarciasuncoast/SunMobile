using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.App;

namespace SunMobile.Droid.Common
{
	public class ToastDialogFragment : DialogFragment
	{
		TextView txtMessage;
		ImageButton btnClose;

		string _message;

		public ToastDialogFragment(string message)
		{
			_message = message;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
			var view = inflater.Inflate(Resource.Layout.ToastDialog, container, true);

			var attributes = Dialog.Window.Attributes;
			attributes.Gravity = GravityFlags.Bottom;
			Dialog.Window.Attributes = attributes;

			txtMessage = view.FindViewById<TextView>(Resource.Id.txtMessage);
			txtMessage.Text = _message;

			btnClose = view.FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnClose.Click += (sender, e) => Dismiss();

			return view;
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