using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile
{
	[DataContract]
	public class PayloadMessage
	{
		[DataMember(Name = "message")]
		public string Payload { get; set; }
		[DataMember(Name = "deviceIpAddress")]
		public string IpAddress { get; set; }
		[DataMember(Name = "transactionId")]
		public string TransactionId { get; set; }
		[DataMember]
		public string DateLastChanged { get; set; } //yyyyMMdd
	}
}