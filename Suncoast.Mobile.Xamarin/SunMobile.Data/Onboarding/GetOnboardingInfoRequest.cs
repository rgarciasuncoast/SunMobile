using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBoarding
{
	[DataContract]
	public class GetOnboardingInfoRequest
	{
		[DataMember]
		public string Version { get; set; }

		[DataMember]
		public string PictureType { get; set; } // SunBlock.DataTransferObjects.OnBoarding.OnboardingPictureTypes - Standard, Retina, RetinaPlus
	}
}