using Android.Content;
using Android.Support.V4.View;
using Android.Util;

namespace SunMobile.Droid.Onboarding
{
	public class OnboardingViewPager : ViewPager
	{
		PagerAdapter _pagerAdapter;
		bool _isAttached;

		public OnboardingViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public OnboardingViewPager(Context context) : base(context)
		{
		}

		public void StoreAdapter(PagerAdapter pagerAdapter)
		{
			_pagerAdapter = pagerAdapter;

			if (_isAttached && _pagerAdapter != null)
			{
				Adapter = _pagerAdapter;
			}
		}

		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();

			_isAttached = true;

			if (_pagerAdapter != null)
			{
				Adapter = _pagerAdapter;
			}
		}

		protected override void OnDetachedFromWindow()
		{
			try
			{
				_isAttached = false;
				Adapter = null;
			}
			catch { }
		}
	}
}