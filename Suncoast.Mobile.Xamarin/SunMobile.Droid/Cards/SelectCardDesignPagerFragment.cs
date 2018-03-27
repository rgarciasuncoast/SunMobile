/*
﻿using Android.OS;
using Android.Views;
using Android.Widget;

namespace SunMobile.Droid.Cards
{
	public class SelectCardDesignPagerFragment : Android.Support.V4.App.Fragment
	{
		public SelectCardDesignPagerFragment()
		{
		}

		public static Android.Support.V4.App.Fragment newInstance(SelectCardDesignFragment context, int pos, float scale, bool IsBlured)
		{
			Bundle b = new Bundle();
			b.PutInt("pos", pos);
			b.PutFloat("scale", scale);
			b.PutBoolean("IsBlured", IsBlured);
			var myf = new SelectCardDesignPagerFragment();
			return Android.Support.V4.App.Fragment.Instantiate(context.Context, myf.Class.Name, b);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			if (container == null)
			{
				return null;
			}

			LinearLayout l = (LinearLayout)inflater.Inflate(Resource.Layout.CardPagerFragmentView, container, false);

			int pos = this.Arguments.GetInt("pos");

			TextView tv = (TextView)l.FindViewById(Resource.Id.viewID);
			tv.Text = "Position = " + pos;

			LinearLayout root = (LinearLayout)l.FindViewById(Resource.Id.root);
			float scale = this.Arguments.GetFloat("scale");
			bool isBlured = this.Arguments.GetBoolean("IsBlured");
			if (isBlured)
			{
				ViewHelper.SetAlpha(root, SelectCardDesignPagerAdapter.getMinAlpha());
				ViewHelper.SetRotationY(root, SelectCardDesignPagerAdapter.getMinDegree());
			}
			return l;
		}
	}
}
*/