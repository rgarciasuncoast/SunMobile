using System.Threading.Tasks;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.OnBoarding;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Utilities.Web;

namespace SunMobile.Shared
{
	public class OnboardingMethods : SunBlockServiceBase
	{
		public Task<StatusResponse<OnboardingCarousel>> GetOnboardingInfo(GetOnboardingInfoRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockAnalyzeUrl + "v2/GetOnboardingInfo";
			var response = PostToSunBlock<StatusResponse<OnboardingCarousel>>(url, request, @"", view);

			return response;
		}
	}
}