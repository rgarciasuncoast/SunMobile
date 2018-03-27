using Android.OS;
using Android.Views;
using SunMobile.Shared.Data;

namespace SunMobile.Droid.Accounts.SubAccounts
{
	public class SubAccountsFundingFragment : SubAccountsBaseContentFragment, ISubAccountsView
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SubAccountsFundingView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			ClearAll();
		}

		private void ClearAll()
		{
		}

		public string Validate()
		{
			return string.Empty;
		}
	}
}