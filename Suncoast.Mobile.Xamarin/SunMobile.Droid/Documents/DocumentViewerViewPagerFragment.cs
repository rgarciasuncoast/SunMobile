using System.Collections.Generic;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Text;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunMobile.Shared.Sharing;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid.Documents
{
	public class DocumentViewerViewPagerFragment : BaseFragment
	{
		public List<DocumentCenterFile> Files { get; set; }
		private ViewPager viewPager;
		private LinearLayout dotsLayout;
		private ImageView btnPrint;
		private ImageView btnShare;
		private TextView[] _dots;
		private int _currentPage;
		private string _tempFullFileName;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.DocumentViewerViewPager, null);
			RetainInstance = true;

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			((MainActivity)Activity).SetActionBarTitle("Document Viewer");

			viewPager = Activity.FindViewById<ViewPager>(Resource.Id.viewPager);
			viewPager.PageSelected += (sender, e) =>
			{
				OnPageSelected(e.Position);
			};

			dotsLayout = Activity.FindViewById<LinearLayout>(Resource.Id.viewPagerCountDots);

			btnPrint = Activity.FindViewById<ImageView>(Resource.Id.btnPrint);
			btnPrint.Click += (sender, e) =>
			{
				Print();
			};

			btnShare = Activity.FindViewById<ImageView>(Resource.Id.btnShare);
			btnShare.Click += (sender, e) =>
			{
				Share();
			};

			_dots = new TextView[Files.Count];

			for (int i = 0; i < Files.Count; i++)
			{
				_dots[i] = new TextView(Activity);
				#pragma warning disable CS0618 // Type or member is obsolete
				_dots[i].Text = Html.FromHtml("&#8226;").ToString();
				#pragma warning restore CS0618 // Type or member is obsolete
				_dots[i].TextSize = 30;
				dotsLayout.AddView(_dots[i]);
			}

			ShowActivityIndicator();

			viewPager.Adapter = new DocumentViewerViewPageAdapter(Activity.SupportFragmentManager, Files);

			HideActivityIndicator();

			OnPageSelected(0);
		}

		public void OnPageSelected(int position)
		{
			for (int i = 0; i < Files.Count; i++)
			{
				_dots[i].SetTextColor(Color.Black);
			}

			_dots[position].SetTextColor(Color.White);

			_currentPage = position;
		}

		private void Print()
		{
			Sharing.Print(Activity, ((DocumentViewerContentFragment)((DocumentViewerViewPageAdapter)viewPager.Adapter).GetItem(_currentPage)).GetWebView());
		}

		private void Share()
		{
			var currentFragment = ((DocumentViewerContentFragment)((DocumentViewerViewPageAdapter)viewPager.Adapter).GetItem(_currentPage));
            _tempFullFileName = Sharing.Share(Activity, currentFragment.File.FileName, currentFragment.FileBytes, currentFragment.File.MimeType);
		}

		public override void OnStop()
		{
			IsolatedStorage.DeleteFile(_tempFullFileName);

			base.OnStop();
		}
	}
}