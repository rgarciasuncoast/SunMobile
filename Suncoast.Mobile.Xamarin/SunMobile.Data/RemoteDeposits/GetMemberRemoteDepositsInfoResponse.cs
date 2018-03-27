using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.RemoteDeposits
{
	[DataContract]
	public class GetMemberRemoteDepositsInfoResponse : StatusResponse
	{
		[DataMember]
		public bool IsMemberQualified { get; set; }
		[DataMember]
		public string QualificationMessage { get; set; }
		[DataMember]
		public bool IsMemberEnrolled { get; set; }
		[DataMember]
		public string EnrollmentAgreement { get; set; }
		[DataMember]
		public bool IsMemberSuncoastEmployee { get; set; }
		[DataMember]
		public decimal DailyLimitAmount { get; set; }
		[DataMember]
		public List<string> RemoteDepositSuffixes { get; set; }
		[DataMember]
		public List<string> ExcludedCamera2Devices { get; set; }
	}
}