using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor
{
	[DataContract]
	public class MobileDeviceVerificationRequest<T> : MobileDeviceVerificationRequest
	{
		[DataMember]
		public T Request { get; set; }
	}

	[DataContract]
	public class MobileDeviceVerificationRequest
	{
		[DataMember]
		public string Payload { get; set; }

		[DataMember]
		public string TransactionType { get; set; }

		[DataMember]
		public string MessageType { get; set; }

		[DataMember]
		public string VerificationToken { get; set; }
	}
}