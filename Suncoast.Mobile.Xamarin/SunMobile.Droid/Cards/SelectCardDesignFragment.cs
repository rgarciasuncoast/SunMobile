/*
using System;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;

namespace SunMobile.Droid.Cards
{
	public class SelectCardDesignFragment : BaseFragment
	{
		public static int LOOPS = 10;
		public static int FIRST_PAGE = 1;
		public static float BIG_SCALE = 1.0f;
		public static float SMALL_SCALE = 0.8f;
		public static float DIFF_SCALE = BIG_SCALE - SMALL_SCALE;

		public SelectCardDesignPagerAdapter adapter;
		public ViewPager viewPager;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.SelectCardDesignView, null);
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

			((MainActivity)Activity).SetActionBarTitle("Select a Card Design");

			viewPager = Activity.FindViewById<ViewPager>(Resource.Id.cardViewPager);
			adapter = new SelectCardDesignPagerAdapter(this, this.ChildFragmentManager, FIRST_PAGE, BIG_SCALE, SMALL_SCALE);
			viewPager.Adapter = adapter;
			viewPager.SetOnPageChangeListener(adapter);
			viewPager.SetCurrentItem (FIRST_PAGE,true);
			viewPager.OffscreenPageLimit = 3;
			viewPager.PageMargin = Convert.ToInt32 (GetString(170));//Resource.String.pagermargin
		}
	}
}
*/