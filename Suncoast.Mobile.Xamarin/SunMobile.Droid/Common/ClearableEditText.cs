using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace SunMobile.Droid.Common
{
	public class ClearableEditText : EditText, View.IOnTouchListener, View.IOnFocusChangeListener
	{
		ClearButtonLocation loc = ClearButtonLocation.RIGHT;
		Drawable xD;
		Listener listener;
		IOnTouchListener l;
		IOnFocusChangeListener f;

		public interface Listener
		{
			void didClearText();
		}

		protected ClearableEditText(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
			init();
		}

		public ClearableEditText(Context context) : this(context, null)
		{
		}

		public ClearableEditText(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
		}

		public ClearableEditText(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
			init();
		}

		public void setListener(Listener listener)
		{
			this.listener = listener;
		}

		public void setIconLocation(ClearButtonLocation loc)
		{
			this.loc = loc;
			initIcon();
		}

		public override void SetOnTouchListener(IOnTouchListener l)
		{
			this.l = l;
		}

		public void SetOnFocusChangeListener(IOnFocusChangeListener f)
		{
			this.f = f;
		}

		public bool OnTouch(View v, MotionEvent e)
		{
			if (getDisplayedDrawable() != null)
			{
				int x = (int)e.GetX();
				int y = (int)e.GetY();
				int left = (loc == ClearButtonLocation.LEFT) ? 0 : Width - PaddingRight - xD.IntrinsicWidth;
				int right = (loc == ClearButtonLocation.LEFT) ? PaddingLeft + xD.IntrinsicWidth : Width;
				bool tappedX = x >= left && x <= right && y >= 0 && y <= (Bottom - Top);
				if (tappedX)
				{
					if (e.Action == MotionEventActions.Up)
					{
						Text = "";
						if (listener != null)
						{
							listener.didClearText();
						}
					}
					return true;
				}
			}
			return l != null && l.OnTouch(v, e);
		}

		public void OnFocusChange(View v, bool hasFocus)
		{
			if (hasFocus)
			{
				setClearIconVisible(!string.IsNullOrEmpty(Text));
			}
			else
			{
				setClearIconVisible(false);
			}

			if (f != null)
			{
				f.OnFocusChange(v, hasFocus);
			}
		}

		protected override void OnTextChanged(ICharSequence text, int start, int lengthBefore, int lengthAfter)
		{
			if (IsFocused)
			{
				setClearIconVisible(!string.IsNullOrEmpty(Text));
			}
		}

		public override void SetCompoundDrawables(Drawable left, Drawable top, Drawable right, Drawable bottom)
		{
			base.SetCompoundDrawables(left, top, right, bottom);
			initIcon();
		}

		void init()
		{
			base.SetOnTouchListener(this);
			OnFocusChangeListener = this;
			initIcon();
			setClearIconVisible(false);
		}

		void initIcon()
		{
			xD = null;
			if (loc != null)
			{
				xD = GetCompoundDrawables()[loc.idx];
			}
			if (xD == null)
			{
				//xD = Resources.GetDrawable(Resource.Drawable.icosent);
			}

			xD.Bounds = new Android.Graphics.Rect(0, 0, xD.IntrinsicWidth, xD.IntrinsicHeight);
			int min = PaddingTop + xD.IntrinsicHeight + PaddingBottom;

			if (SuggestedMinimumHeight < min)
			{
				SetMinimumHeight(min);
			}
		}

		Drawable getDisplayedDrawable()
		{
			return (loc != null) ? GetCompoundDrawables()[loc.idx] : null;
		}

		protected void setClearIconVisible(bool visible)
		{
			Drawable[] cd = GetCompoundDrawables();
			Drawable displayed = getDisplayedDrawable();
			bool wasVisible = (displayed != null);
			if (visible != wasVisible)
			{
				Drawable x = visible ? xD : null;
				SetCompoundDrawables((loc == ClearButtonLocation.LEFT) ? x : cd[0], cd[1], (loc == ClearButtonLocation.RIGHT) ? x : cd[2], cd[3]);
			}
		}
	}

	public class ClearButtonLocation
	{
		public static readonly ClearButtonLocation LEFT = new ClearButtonLocation(0);
		public static readonly ClearButtonLocation RIGHT = new ClearButtonLocation(2);
		public readonly int idx;

		ClearButtonLocation(int idx)
		{
			this.idx = idx;
		}

		public static IEnumerable<ClearButtonLocation> Values {
			get {
				yield return LEFT;
				yield return RIGHT;
			}
		}
	}
}
