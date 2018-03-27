using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBoarding
{
	[DataContract]
	public class OnboardingCarouselItem
	{
		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string BackgroundColor { get; set; }

		[DataMember]
		public string TintColor { get; set; }

		[DataMember]
		public List<OnboardingCarouselImage> OnboardingCarouselImages { get; set; }
	}
}