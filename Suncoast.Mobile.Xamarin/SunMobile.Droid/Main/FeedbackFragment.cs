using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid.Main
{
	public class FeedbackFragment : BaseFragment
	{
		public event Action<string> Completed = delegate {};
		private Button btnSignMeUp;
		private Button btnRemindMeLater;
		private Button btnNoThanks;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.FeedbackView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			SetupView();
		}

		public override void SetupView()
		{
			base.SetupView();

			((MainActivity)Activity).SetActionBarTitle("Suncoast Feedback");

			btnSignMeUp = Activity.FindViewById<Button>(Resource.Id.btnSignMeUp);
			btnSignMeUp.Click += (sender, e) => SignMeUp();
			btnRemindMeLater = Activity.FindViewById<Button>(Resource.Id.btnRemindMeLater);
			btnRemindMeLater.Click += (sender, e) => RemindMeLater();
			btnNoThanks = Activity.FindViewById<Button>(Resource.Id.btnNoThanks);
			btnNoThanks.Click += (sender, e) => NoThanks();
		}

		private void SignMeUp()
		{
			try
			{
				RetainedSettings.Instance.ShowFeedback = false;
				var url = SessionSettings.Instance.GetStartupSettings["FeedbackUrl"];
				var uri = Android.Net.Uri.Parse(url);
				var intent = new Intent(Intent.ActionView, uri);
				StartActivity(intent);
				Completed("success");
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "FeedbackFragment:SignMeUp");
			}
		}

		private void RemindMeLater()
		{
			Completed("success");
		}

		private void NoThanks()
		{
			RetainedSettings.Instance.ShowFeedback = false;
			Completed("success");
		}
	}
}