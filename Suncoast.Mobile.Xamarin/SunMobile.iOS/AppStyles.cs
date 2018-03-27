using System;
using UIKit;

namespace SunMobile.iOS
{
    public static class AppStyles
    {
		public static UIColor SuncoastOrange = UIColor.FromRGB(0xf5, 0x8d, 0x31);
		public static UIColor SuncoastBlue = UIColor.FromRGB(0x1f, 0xaf, 0xcc);
		public static UIColor BarTintColor = UIColor.FromRGB(0x20, 0xa6, 0xf8);
		public static UIColor TintColor = UIColor.FromRGB(0x20, 0xa6, 0xf8);
		public static UIColor TitleBarItemTintColor = UIColor.White;        
		public static UIColor MenuBackgroundColor = UIColor.White;
		public static UIColor MenuTextColor = UIColor.FromRGB(0x1f, 0x1f, 0x1f);
		public static UIColor MenuBackgroundColorOrange = UIColor.FromRGB(0xe4, 0x5f, 0x24);
		public static UIColor MenuBackgroundColorGray = UIColor.FromRGB(0x39, 0x3d, 0x40);
		public static UIColor TableHeaderBackgroundColor = UIColor.FromRGB(0x92, 0x92, 0x92);
		public static UIColor TableHeaderTextColor = UIColor.White;
		public static UIColor DepositsHeaderBackgroundColor = UIColor.FromRGB(0x92, 0x92, 0x92);
		public static UIColor LoansHeaderBackgroundColor = UIColor.FromRGB(0x92, 0x92, 0x92);
		public static UIColor CreditCardsHeaderBackgroundColor = UIColor.FromRGB(0x92, 0x92, 0x92);
		public static UIColor RegularAccountsBackgroundColor = UIColor.FromRGB(0xff, 0xff, 0xff);
		public static UIColor SecondaryAccountsBackgroundColor = UIColor.FromRGB(0xff, 0xff, 0xe0);
		public static UIColor JointAccountsBackgroundColor = UIColor.FromRGB(0xd4, 0xeb, 0xff);
		public static UIColor ButtonColor = UIColor.FromRGB(0x30, 0xa7, 0xdf);
		public static UIColor ButtonDisabledColor = UIColor.Gray;
		public static UIColor SlideOutMenuBackgroundColorGray = UIColor.FromRGB(0x39, 0x3d, 0x40);

		public static UIFont ThemeFontBold(int size)
		{
			return UIFont.FromName("Roboto-Bold", size);
		}

		public static UIFont ThemeFontRegular(int size)
		{
			return UIFont.FromName("Roboto-Regular", size);
		}

		public static UIFont ThemeFontLight(int size)
		{
			return UIFont.FromName("Roboto-Light", size);
		}

		public static UIFont ThemeFontMedium(int size)
		{
			return UIFont.FromName("Roboto-Medium", size);
		}

		public static void SetViewBorder(UIView view, bool roundedCorners)
		{
			if (roundedCorners)
			{            
				view.Layer.CornerRadius = 5;
				view.Layer.MasksToBounds = true;
			}

			view.Layer.BorderColor = TintColor.CGColor;
			view.Layer.BorderWidth = 0.5f;
		}

		public static UIColor ColorFromHexString(string hexValue, float alpha = 1.0f)
		{
			var colorString = hexValue.Replace ("#", "");

			if (alpha > 1.0f) 
			{
				alpha = 1.0f;
			} 
			else if (alpha < 0.0f) 
			{
				alpha = 0.0f;
			}

			float red, green, blue;

			switch (colorString.Length) 
			{
				case 3 : // #RGB
				{
					red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
					green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
					blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
					return UIColor.FromRGBA(red, green, blue, alpha);
				}
				case 6 : // #RRGGBB
				{
					red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
					green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
					blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
					return UIColor.FromRGBA(red, green, blue, alpha);
				}
				default :
					throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB", hexValue));
			}
		}
    }
}