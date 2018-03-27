using System.Runtime.Serialization;

namespace SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account
{
	[DataContract]
	public class FlagOption
	{
		[DataMember]
		public bool Value { get; set; }

		[DataMember]
		public string FlagType { get; set; } //FlagTypes enum
	}
}