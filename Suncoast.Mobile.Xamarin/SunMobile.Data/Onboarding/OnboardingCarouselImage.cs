using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBoarding
{
    [DataContract]
    public class OnboardingCarouselImage
    {
        [DataMember]
		public string PictureType { get; set; } // SunBlock.DataTransferObjects.OnBoarding.OnboardingPictureTypes - Standard, Retina, RetinaPlus

		[DataMember]
		public string OnboardingPictureUrl { get; set; }
	}
}