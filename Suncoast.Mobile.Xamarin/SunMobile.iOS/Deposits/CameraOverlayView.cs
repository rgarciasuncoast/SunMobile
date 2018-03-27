using System;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace SunMobile.iOS.Deposits
{
    public class CameraOverlayView : UIView
    {		
		public CGRect ParentFrame { get; set; }
		public RectangleF PreviewRect { get; set; }

		public CameraOverlayView(CGRect frame) : base(frame)
        { 			
			ParentFrame = frame;
            BackgroundColor = UIColor.Clear;
        }

        public override void Draw(CGRect rect)
        {
			base.Draw(rect);            

			// A standard check is  6" x 2¾" for a ratio of 2.18 width to height
			const double checkRatio = 2.18;

			nfloat viewWidth;
			nfloat viewHeight;

			// Get Dimensions
			if (ParentFrame.Width > ParentFrame.Height)
			{
				viewWidth = ParentFrame.Width;
				viewHeight = ParentFrame.Height;
			}
			else
			{
				viewWidth = ParentFrame.Height;
				viewHeight = ParentFrame.Width;
			}

			ParentFrame = new CGRect(0, 0, viewWidth, viewHeight);
			//ParentFrame = new CGRect(0, 0, viewWidth, viewHeight);

			var previewWidth = viewWidth * .95f;
			var previewHeight = previewWidth / checkRatio;

			if (previewHeight > viewHeight)
			{
				previewHeight = viewHeight * .95f;
			}

			var tempxLeft = ((viewWidth - previewWidth) / 2);
			var tempxRight = viewWidth - tempxLeft;
			var tempyTop = ((viewHeight - previewHeight) / 2);
			var tempyBottom = viewHeight - tempyTop;

			// Flip 180 degrees
			var xLeft = tempyBottom;
			var xRight = tempyTop;
			var yTop = tempxLeft;
			var yBottom = tempxRight;

			PreviewRect = new RectangleF((float)tempxLeft, (float)tempyTop, (float)(tempxRight - tempxLeft), (float)(tempyBottom - tempyTop));

            using (CGContext context = UIGraphics.GetCurrentContext())
            {
				context.SetLineWidth(2);
                context.SetFillColor(new CGColor(0, 0, 0, 0));
				UIColor.White.SetStroke();

                var path = new CGPath();
                
                path.AddLines(new []
                {
					new CGPoint(xLeft, yTop),
					new CGPoint(xLeft, yBottom),
					new CGPoint(xRight, yBottom),
					new CGPoint(xRight, yTop),
					new CGPoint(xLeft, yTop)
                });                

                context.AddPath(path);
                context.DrawPath(CGPathDrawingMode.FillStroke);
            }
        }
    }
}