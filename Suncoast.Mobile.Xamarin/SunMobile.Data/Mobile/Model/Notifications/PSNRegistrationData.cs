using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.Notifications
{
	[DataContract]
	public class PSNRegistrationData
	{

		[DataMember]
		public string PlatformSpecificHandle { get; set; }
	}
}