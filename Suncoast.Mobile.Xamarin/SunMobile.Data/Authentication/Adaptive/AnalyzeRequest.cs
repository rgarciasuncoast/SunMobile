using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Culture;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive
{
	[DataContract]
	public class AnalyzeRequest : Session.SessionRequest, ILanguage
	{
		[DataMember]
		public string Language { get; set; } //Language Types enum
	}
}