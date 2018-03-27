using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication
{
	[DataContract]
	public class GetChangePasswordInformationResponse : StatusResponse
	{
		[DataMember]
		public string ConcisePasswordInstructions { get; set; }
		[DataMember]
		public string CompletePasswordInstructions { get; set; }
		[DataMember]
		public string PasswordRegex { get; set; }
	}
}