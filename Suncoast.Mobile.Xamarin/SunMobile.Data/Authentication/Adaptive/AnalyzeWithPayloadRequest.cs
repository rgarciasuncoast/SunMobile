using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile;
using SunBlock.DataTransferObjects.Culture;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
	[DataContract]
	public class AnalyzeWithPayloadRequest : Session.SessionRequest, ILanguage
	{
		[DataMember]
		public PayloadMessage Payload { get; set; }
		[DataMember]
		public string Language { get; set; } //Language Types enum
	}
}