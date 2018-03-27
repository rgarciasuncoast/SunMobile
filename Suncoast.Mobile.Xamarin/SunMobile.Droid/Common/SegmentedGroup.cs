using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace SunMobile.Droid.Common
{
	public class SegmentedGroup : RadioGroup
	{
		int _marginDp;
		float _cornerRadius;
		int _tintConlor;
		int _selectedTintColor = Color.White;
		Resources resources;
		LayoutSelector _layoutSelector;

		// Added to fix orientation detection
		IWindowManager windowManager;
		SurfaceOrientation currentRotation;

		public SegmentedGroup(Context context) : base(context)
		{
			Setup();
		}

		// Reads the attributes from the layout
		private void initAttrs(IAttributeSet attrs)
		{
			TypedArray typedArray = Context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.SegmentedGroup, 0, 0);

			try
			{
				_marginDp = (int) typedArray.GetDimension(Resource.Styleable.SegmentedGroup_sc_border_width, Resources.GetDimension(Resource.Dimension.radio_button_stroke_border));
				_cornerRadius = typedArray.GetDimension(Resource.Styleable.SegmentedGroup_sc_corner_radius, Resources.GetDimension(Resource.Dimension.radio_button_conner_radius));
				#pragma warning disable CS0618 // Type or member is obsolete
				_tintConlor = typedArray.GetColor(Resource.Styleable.SegmentedGroup_sc_tint_color, Resources.GetColor(Resource.Color.radio_button_selected_color));
				_selectedTintColor = typedArray.GetColor(Resource.Styleable.SegmentedGroup_sc_checked_text_color, Resources.GetColor(Android.Resource.Color.White));
				#pragma warning restore CS0618 // Type or member is obsolete
			}
			finally
			{
				typedArray.Recycle();
			}
		}

		public SegmentedGroup(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			initAttrs(attrs);
			Setup ();
		}

		void Setup()
		{
			windowManager = Context.GetSystemService (Context.WindowService).JavaCast<IWindowManager> ();
			currentRotation = windowManager.DefaultDisplay.Rotation;
			resources = Resources;
			#pragma warning disable CS0618 // Type or member is obsolete
			_tintConlor = Resources.GetColor (Resource.Color.radio_button_selected_color);
			#pragma warning restore CS0618 // Type or member is obsolete
			_marginDp = (int)Resources.GetDimension (Resource.Dimension.radio_button_stroke_border);
			_cornerRadius = Resources.GetDimension (Resource.Dimension.radio_button_conner_radius);
			_layoutSelector = new LayoutSelector (_cornerRadius, currentRotation, resources, this);
		}

		protected override void OnFinishInflate()
		{
			base.OnFinishInflate();
			// Use holo light for default
			UpdateBackground();
		}

		public void SetTintColor(int tintColor)
		{
			_tintConlor = tintColor;
			UpdateBackground();
		}

		public void SetTintColor(int tintColor, int checkedTextColor)
		{
			_tintConlor = tintColor;
			_selectedTintColor = checkedTextColor;
			UpdateBackground();
		}

		public void UpdateBackgroundExternal()
		{
			Setup();
			UpdateBackground();
		}

		public void UpdateBackground()
		{
			int count = base.ChildCount;

			for (int i = 0; i < count; i++) 
			{
				View child = GetChildAt(i);
				UpdateBackground(child);

				// If this is the last view, don't set LayoutParams
				if (i == count - 1)
				{
					var iParams = (LayoutParams)child.LayoutParameters;
					var laParams = new LayoutParams(iParams.Width, iParams.Height, iParams.Weight);
					laParams.SetMargins(0, 0, _marginDp, 0);
					child.LayoutParameters = laParams;
					break;
				}

				var initParams = (LayoutParams) child.LayoutParameters;
				var lParams = new LayoutParams(initParams.Width, initParams.Height, initParams.Weight);
				lParams.SetMargins(0, 0, -_marginDp, 0);
				child.LayoutParameters = lParams;
			}
		}

		private void UpdateBackground(View view)
		{
			int mChecked = _layoutSelector.GetSelected();
			int mUnchecked = _layoutSelector.GetUnselected();

			// Set text color
			int statePressed = Android.Resource.Attribute.StatePressed;
			int stateChecked = Android.Resource.Attribute.StateChecked;
			int[][] stateListArray = {
				new int[] {statePressed},
				new int[] {-statePressed, -stateChecked},
				new int[] {-statePressed, stateChecked}
			};

			var colorStateList = new ColorStateList(stateListArray,
				new int[]
				{
					Color.Gray, _tintConlor, _selectedTintColor
				});

			((Button)view).SetTextColor(colorStateList);

			// Redraw with tint color
			#pragma warning disable CS0618 // Type or member is obsolete
			Drawable checkedDrawable = Resources.GetDrawable(mChecked).Mutate();
			Drawable uncheckedDrawable = Resources.GetDrawable(mUnchecked).Mutate();
			#pragma warning restore CS0618 // Type or member is obsolete
			((GradientDrawable) checkedDrawable).SetColor(_tintConlor);
			((GradientDrawable) checkedDrawable).SetStroke(_marginDp, new Color(_tintConlor));
			((GradientDrawable) uncheckedDrawable).SetStroke(_marginDp,  new Color(_tintConlor));

			// Set proper radius
			((GradientDrawable) checkedDrawable).SetCornerRadii(_layoutSelector.GetChildRadii(view));
			((GradientDrawable) uncheckedDrawable).SetCornerRadii(_layoutSelector.GetChildRadii(view));

			// Create drawable
			var stateListDrawable = new StateListDrawable();
			stateListDrawable.AddState(new []{-Android.Resource.Attribute.StateChecked}, uncheckedDrawable);
			stateListDrawable.AddState(new []{Android.Resource.Attribute.StateChecked}, checkedDrawable);

			// Set button background
			if ((int) Android.OS.Build.VERSION.SdkInt >= 16)
			{
				view.Background = stateListDrawable;
			}
			else
			{
				#pragma warning disable 618
				view.SetBackgroundDrawable(stateListDrawable);
				#pragma warning restore 618
			}
		}
	}

	/*
	 * This class is used to provide the proper layout based on the view.
	 * Also provides the proper radius for corners.
	 * The layout is the same for each selected left/top middle or right/bottom button.
	 * float tables for setting the radius via Gradient.setCornerRadii are used instead
	 * of multiple xml drawables.
	 */
	class LayoutSelector
	{
		// Reference to Segmented Group, allows this selector to get its child views
		private SegmentedGroup sg;

		private int children;
		private int child;
		private readonly int SELECTED_LAYOUT = Resource.Drawable.radio_checked;
		private readonly int UNSELECTED_LAYOUT = Resource.Drawable.radio_unchecked;

		private float r; // this is the radios read by attributes or xml dimens

		//private readonly float r1 = TypedValue.ApplyDimension(TypedValue.COMPLEX_UNIT_DIP, 0.1f, Resources.DisplayMetrics()); // 0.1 dp to px
		private readonly float r1;

		private readonly float[] rLeft; // left radio button
		private readonly float[] rRight; // right radio button
		private readonly float[] rMiddle; // middle radio button
		private readonly float[] rDefault; // default radio button
		private float[] radii; // result radii float table

		public LayoutSelector(float cornerRadius, SurfaceOrientation sOrientation, Resources resources, SegmentedGroup segmentedGroup)
		{
			sg = segmentedGroup;

			r1 = (0.1f) * resources.DisplayMetrics.Density; // 0.1 dp to px

			children = -1; // Init this to force setChildRadii() to enter for the first time.
			child = -1; // Init this to force setChildRadii() to enter for the first time.
			r = cornerRadius;
			rLeft = new float[] { r, r, r1, r1, r1, r1, r, r};
			rRight = new float[] { r1, r1, r, r, r, r, r1, r1};
			rMiddle = new float[] { r1, r1, r1, r1, r1, r1, r1, r1};
			rDefault = new float[] { r, r, r, r, r, r, r, r};
		}

		private int GetChildren()
		{
			return sg.ChildCount;
		}

		private int GetChildAtIndex(View view)
		{
			return sg.IndexOfChild(view);
		}

		private void SetChildRadii(int newChildren, int newChild)
		{
			// If same values are passed, just return. No need to update anything
			if (children == newChildren && child == newChild)
				return;

			// Set the new values
			children = newChildren;
			child = newChild;

			// if there is only one child provide the default radio button
			if (children == 1) 
			{
				radii = rDefault;
			} 
			else if (child == 0) 
			{
				// left
				radii = rLeft;
			} 
			else if (child == children - 1) 
			{
				// right
				radii = rRight;
			} 
			else 
			{
				radii = rMiddle;
			}
		}

		/* Returns the seleted layout id based on view */
		public int GetSelected()
		{
			return SELECTED_LAYOUT;
		}

		/* Returns the unselected layout id based on view */
		public int GetUnselected()
		{
			return UNSELECTED_LAYOUT;
		}

		/* Returns the radii float table based on view for Gradient.setRadii() */
		public float[] GetChildRadii(View view)
		{
			int newChildren = GetChildren();
			int newChild = GetChildAtIndex(view);
			SetChildRadii (newChildren, newChild);
			return radii;
		}
	}
}