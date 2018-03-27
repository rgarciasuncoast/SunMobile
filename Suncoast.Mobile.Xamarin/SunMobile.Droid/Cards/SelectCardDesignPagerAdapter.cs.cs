/*
﻿using System;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Widget;

namespace SunMobile.Droid.Cards
{
	public class SelectCardDesignPagerAdapter : FragmentStatePagerAdapter, ViewPager.IOnPageChangeListener
	{
		public static int first_page;
		public static float big_scale;
		public static float small_scale;
		private bool swipedLeft = false;
		private int lastPage = 9;
		private LinearLayout cur = null;
		private LinearLayout next = null;
		private LinearLayout prev = null;
		//private MyLinearLayout prevprev = null;
		private LinearLayout nextnext = null;
		private SelectCardDesignFragment context;
		private Android.Support.V4.App.FragmentManager fm;
		private float scale;
		private bool IsBlured;
		private static float minAlpha = 0.6f;
		private static float maxAlpha = 1f;
		private static float minDegree = 60.0f;
		//private int counter=0;

		public static float getMinDegree()
		{
			return minDegree;
		}
		public static float getMinAlpha()
		{
			return minAlpha;
		}
		public static float getMaxAlpha()
		{
			return maxAlpha;
		}

		public SelectCardDesignPagerAdapter(SelectCardDesignFragment context, FragmentManager fm, int firstPage, float bigScale, float smallScale) : base(fm)
		{
			this.fm = fm;
			this.context = context;
			first_page = firstPage;
			big_scale = bigScale;
			small_scale = smallScale;
		}

		public override Fragment GetItem(int position)
		{
			if (position == first_page)
            {
				scale = big_scale;
            }
            else
			{
				scale = small_scale;
				IsBlured = true;
			}

			Console.WriteLine("position =========== " + position.ToString());
			var curFragment = SelectCardDesignPagerFragment.newInstance(context, position, scale, IsBlured);
			cur = getRootView(position);
			next = getRootView(position + 1);
			prev = getRootView(position - 1);

			return curFragment;
		}

		public override int Count
		{
			get
			{
				return 10;
			}
		}

		void ViewPager.IOnPageChangeListener.OnPageScrollStateChanged(int state)
		{
			//throw new NotImplementedException ();
		}

		void ViewPager.IOnPageChangeListener.OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
		{
			if (positionOffset >= 0f && positionOffset <= 1f)
			{
				positionOffset = positionOffset * positionOffset;
				cur = getRootView(position);
				next = getRootView(position + 1);
				prev = getRootView(position - 1);
				nextnext = getRootView(position + 2);

				if (cur != null)
				{
					ViewHelper.SetAlpha(cur, maxAlpha - 0.5f * positionOffset);
				}
				if (next != null)
				{
					ViewHelper.SetAlpha(next, minAlpha + 0.5f * positionOffset);
				}
				if (prev != null)
				{
					ViewHelper.SetAlpha(prev, minAlpha + 0.5f * positionOffset);
				}

				if (nextnext != null)
				{
					ViewHelper.SetAlpha(nextnext, minAlpha);
					ViewHelper.SetRotationY(nextnext, -minDegree);
				}
				
                if (cur != null)
				{
					ViewHelper.SetRotationY(cur, 0);
				}

				if (next != null)
				{
					ViewHelper.SetRotationY(next, -minDegree);
				}
				
                if (prev != null)
				{
					ViewHelper.SetRotationY(prev, minDegree);
				}

				//To animate it properly we must understand swipe direction
			    // this code adjusts the rotation according to direction.
			    
				if (swipedLeft)
				{
					if (next != null)
						ViewHelper.SetRotationY(next, -minDegree + minDegree * positionOffset);
					if (cur != null)
						ViewHelper.SetRotationY(cur, 0 + minDegree * positionOffset);
				}
				else
				{
					if (next != null)
						ViewHelper.SetRotationY(next, -minDegree + minDegree * positionOffset);
					if (cur != null)
					{
						ViewHelper.SetRotationY(cur, 0 + minDegree * positionOffset);
					}
				}
			}
			if (positionOffset >= 1f)
			{
				ViewHelper.SetAlpha(cur, maxAlpha);
			}
		}

		void ViewPager.IOnPageChangeListener.OnPageSelected(int position)
		{
			if (lastPage <= position)
			{
				swipedLeft = true;
			}
			else if (lastPage > position)
			{
				swipedLeft = false;
			}
			lastPage = position;
		}


		private LinearLayout getRootView(int position)
		{
			LinearLayout ly;
			try
			{
				ly = (LinearLayout)fm.FindFragmentByTag(this.getFragmentTag(position)).View.FindViewById(Resource.Id.root);
			}
			catch (Exception e)
			{
				return null;
			}
			if (ly != null)
				return ly;
			return null;
		}

		private String getFragmentTag(int position)
		{

			return "android:switcher:" + context.viewPager.Id + ":" + position;
		}
	}
}
*/