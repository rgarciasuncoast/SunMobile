using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor
{
	[DataContract]
	public class OutOfBandVerificationResponse
	{
		[DataMember]
		public string VerificationState { get; set; } //OutOfBandVerificationStates enum
		[DataMember]
		public int AttemptsRemaining { get; set; }
	}
}