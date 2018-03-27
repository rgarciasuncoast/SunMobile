using System;
using Foundation;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public partial class AccountSettingsTableViewCell : UITableViewCell
	{
        public static readonly NSString Key = new NSString("AccountSettingsTableViewCell");
		public static readonly UINib Nib;

        static AccountSettingsTableViewCell()
		{
            Nib = UINib.FromName("AccountSettingsTableViewCell", NSBundle.MainBundle);
		}

        public AccountSettingsTableViewCell(IntPtr handle) : base(handle)
		{
		}
	}
}