using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Authentication.Adaptive;
using SunBlock.DataTransferObjects.UserInterface.MVC;

namespace SunBlock.DataTransferObjects.Mobile.Model.Authentication
{
	[DataContract]
	public class AnalyzeViewModel : ViewContext
	{
		[DataMember]
		public AnalyzeResponse AnalyzeResponse { get; set; }

		[DataMember]
		public string LogTransactionId { get; set; }

		[DataMember]
		public string PermId { get; set; }

		[DataMember]
		public string PermanentId { get; set; }
	}
}