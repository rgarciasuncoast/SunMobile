using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile
{
	[DataContract]
	public class MobileStatusResponse
	{
		[DataMember]
		public bool Success { get; set; }
		[DataMember]
		public string ClientMessage { get; set; }
		[DataMember]
		public string FailureMessage { get; set; }
		[DataMember]
		public string InternalFailureDetails { get; set; }
		[DataMember]
		public bool OutOfBandChallengeRequired { get; set; }
		[DataMember]
		public bool CanUseAtmLastEight { get; set; }
	}

	[DataContract]
	public class MobileStatusResponse<T> : MobileStatusResponse
	{
		[DataMember]
		public T Result { get; set; }
	}
}