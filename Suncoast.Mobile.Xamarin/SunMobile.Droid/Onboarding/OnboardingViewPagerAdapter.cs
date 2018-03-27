using System.Collections.Generic;
using Android.Support.V4.App;
using SunBlock.DataTransferObjects.OnBoarding;

namespace SunMobile.Droid.Onboarding
{
	public class OnboardingViewPagerAdapter : FragmentStatePagerAdapter
	{
		private List<Fragment> _fragments { get; set; }
		private List<OnboardingCarouselItem> _carouselItems;

		public OnboardingViewPagerAdapter(FragmentManager fragmentManager, List<OnboardingCarouselItem> carouselItems, OnboardingViewPagerFragment parent) : base(fragmentManager)
		{
			_carouselItems = carouselItems;

			_fragments = new List<Fragment>();

			foreach (var carouselItem in _carouselItems)
			{
				var onboardingViewContentFragment = new OnboardingViewContentFragment();
				onboardingViewContentFragment.CarouselItem = carouselItem;
				onboardingViewContentFragment.Parent = parent;
				_fragments.Add(onboardingViewContentFragment);
			}
		}

		public override int Count 
		{
			get 
			{
				return _carouselItems.Count;
			}
		}

		public override Fragment GetItem(int position)
		{
			return _fragments[position];
		}
	}
}