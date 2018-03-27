using System;
using Foundation;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public partial class AlertsTableViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("AlertsTableViewCell");
		public static readonly UINib Nib;

		static AlertsTableViewCell()
		{
			Nib = UINib.FromName("AlertsTableViewCell", NSBundle.MainBundle);
		}

		public AlertsTableViewCell(IntPtr handle) : base(handle)
		{
		}
	}
}