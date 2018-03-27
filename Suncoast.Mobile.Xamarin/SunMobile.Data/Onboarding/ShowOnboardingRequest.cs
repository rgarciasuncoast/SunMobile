using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBoarding
{
	[DataContract]
	public class ShowOnboardingRequest
	{
		[DataMember]
		public string MemberId { get; set; }

		[DataMember]
		public string Version { get; set; } // yyyyMMdd, only required to set as shown
	}
}