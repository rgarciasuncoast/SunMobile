using SunMobile.Shared.Views;

namespace SunMobile.Shared.Data
{
	public class AnyMemberInfo
	{
		public bool IsAnyMember { get; set; }
		public string Account { get; set; }
		public string Suffix { get; set; }
		public string AccountType { get; set; }
		public bool IsJoint { get; set; }
		public string LastName { get; set; }
		public ListViewItem Item { get; set; }
	}
}