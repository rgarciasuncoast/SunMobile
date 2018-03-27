using System;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using System.Collections.Generic;
using SunBlock.DataTransferObjects.Products;

namespace SunMobile.Droid.Cards
{
	public class OrderRaysCardPagerAdapter : FragmentStatePagerAdapter, ViewPager.IOnPageChangeListener
	{		
		private int _lastPage;
		private LinearLayout _current = null;
		private LinearLayout _next = null;
		private LinearLayout _previous = null;
		private LinearLayout _nextnext = null;
        private Fragment _context;
		private FragmentManager _fragmentManager;		
        private List<CardInfo> _cards;
		
		public OrderRaysCardPagerAdapter(Fragment context, FragmentManager fragmentManager, List<CardInfo> cards) : base(fragmentManager)
		{
			_fragmentManager = fragmentManager;
			_context = context;
            _cards = cards;			
			_lastPage = _cards.Count;
		}

        public override int GetItemPosition(Java.Lang.Object objectValue)
        {
            return PagerAdapter.PositionNone;
        }

		public override Fragment GetItem(int position)
		{
            var curFragment = OrderRaysCardImageFragment.NewInstance(_context, _cards[position].CardImage);
			_current = GetRootView(position);
			_next = GetRootView(position + 1);
			_previous = GetRootView(position - 1);

			return curFragment;
		}

		public override int Count
		{
			get { return _cards.Count; }
		}

		void ViewPager.IOnPageChangeListener.OnPageScrollStateChanged(int state)
		{
		}

		void ViewPager.IOnPageChangeListener.OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
		{			
			if (positionOffset >= 0f && positionOffset <= 1f)
			{
				positionOffset = positionOffset * positionOffset;
				_current = GetRootView(position);
				_next = GetRootView(position + 1);
				_previous = GetRootView(position - 1);
				_nextnext = GetRootView(position + 2);				
			}			
		}

		void ViewPager.IOnPageChangeListener.OnPageSelected(int position)
		{
			_lastPage = position;
		}

		private LinearLayout GetRootView(int position)
		{
			LinearLayout layout;

			try
			{
				layout = (LinearLayout)_fragmentManager.FindFragmentByTag(GetFragmentTag(position)).View.FindViewById(Resource.Id.root);
			}
			catch (Exception)
			{
				return null;
			}

			if (layout != null)
			{
				return layout;
			}

			return null;
		}

		private string GetFragmentTag(int position)
		{
			//return "android:switcher:" + _context.viewPager.Id + ":" + position;
            return "android:switcher:" + position;
		}
	}
}