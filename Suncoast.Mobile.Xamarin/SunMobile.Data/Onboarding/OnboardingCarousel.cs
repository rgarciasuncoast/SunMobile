using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBoarding
{
	[DataContract]
	public class OnboardingCarousel
	{
		[DataMember]
		public string ChannelType { get; set; } // SunBlock.DataTransferObjects.OnBoarding.OnBoardingChannelTypes - Mobile, Sunnet

		[DataMember]
		public string Version { get; set; } // yyyyMMdd for sunnet

		[DataMember]
		public List<OnboardingCarouselItem> CarouselItems { get; set; }
	}
}