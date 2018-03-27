using Android.Content;
using Android.Util;
using Android.Views;
using Android.Graphics;
using Point = System.Drawing.Point;

namespace SunMobile.Droid.Deposits
{
	public class CameraOverlayView : View
	{
		public Point LeftTop { get; set; }
		public Point RightTop { get; set; }
		public Point LeftBottom { get; set; }
		public Point RightBottom { get; set; }
		public string HelpText { get; set; }

		public CameraOverlayView(Context context) : base(context)
		{
		}

		public CameraOverlayView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public CameraOverlayView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			canvas.Save();

			// A standard check is  6" x 2¾" for a ratio of 2.18 width to height or .458 height to width
			const float checkRatio = 2.18f;

			// Get Dimensions
			float canvasWidth = (float)canvas.Width;
			float canvasHeight = (float)canvas.Height;
			float xLeft;
			float xRight;
			float yTop;
			float yBottom;
			float previewWidth;
			float previewHeight;

			if (canvasWidth > canvasHeight)
			{
				previewWidth = canvasWidth * .90f;
				previewHeight = previewWidth / checkRatio;

				if (previewHeight > canvasHeight)
				{
					previewHeight = canvasHeight * .90f;
				}

				xLeft = ((canvasWidth - previewWidth) / 2);
				xRight = canvasWidth - xLeft;
				yTop = ((canvasHeight - previewHeight) / 2);
				yBottom = canvasHeight - yTop;
			}
			else
			{
				previewHeight = canvasHeight * .90f;
				previewWidth = previewHeight / checkRatio;

				if (previewWidth > canvasWidth)
				{
					previewWidth = canvasWidth * .90f;
				}

				xLeft = ((canvasWidth - previewWidth) / 2);
				xRight = canvasWidth - xLeft;
				yTop = ((canvasHeight - previewHeight) / 2);
				yBottom = canvasHeight - yTop;
			}

			LeftTop = new Point((int)xLeft, (int)yTop);
			LeftBottom = new Point((int)xLeft, (int)yBottom);
			RightTop = new Point((int)xRight, (int)yTop);
			RightBottom = new Point((int)xRight, (int)yBottom);

			var paint = new Paint();
			paint.SetStyle(Paint.Style.Fill);

			// Lines
			paint.Color = Color.White;
			var pts = new float[4];
			pts[0] = xLeft;
			pts[1] = yTop;
			pts[2] = xLeft;
			pts[3] = yBottom;
			canvas.DrawLines(pts, paint);
			pts[0] = xLeft;
			pts[1] = yBottom;
			pts[2] = xRight;
			pts[3] = yBottom;
			canvas.DrawLines(pts, paint);
			pts[0] = xRight;
			pts[1] = yBottom;
			pts[2] = xRight;
			pts[3] = yTop;
			canvas.DrawLines(pts, paint);
			pts[0] = xRight;
			pts[1] = yTop;
			pts[2] = xLeft;
			pts[3] = yTop;
			canvas.DrawLines(pts, paint);

			// Draw Text
			paint.Color = Color.White;
			paint.AntiAlias = true;
			paint.SetShadowLayer(2f, 8f, 8f, Color.Black);
			int pixel = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 28, Resources.DisplayMetrics);
			paint.TextSize = pixel;

			var bounds = new Rect();

			if (!string.IsNullOrEmpty(HelpText))
			{
				paint.GetTextBounds(HelpText, 0, HelpText.Length, bounds);
				canvas.DrawText(HelpText, ((canvasWidth - (float)bounds.Width()) / 2), ((canvasHeight - bounds.Height()) / 2), paint);
			}

			canvas.Restore();
		}

		// Calls the onDraw method
		public void SetInvalidate()
		{
			Invalidate();
		}
	}
}