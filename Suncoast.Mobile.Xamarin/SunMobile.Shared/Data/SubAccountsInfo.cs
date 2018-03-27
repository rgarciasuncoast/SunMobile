namespace SunMobile.Shared.Data
{
    public class SubAccountsInfo
    {
		public int CurrentPage { get; set; }
		public bool IsFunded { get; set; }
		public string MemberFullName { get; set; }
		public string FundingSuffix { get; set; }
		public decimal FundingAmount { get; set; }
		public bool CreateDebitCard { get; set; }
		public bool EnrollInEstatements { get; set; }
		public int CardServiceCode { get; set; }
    }
}