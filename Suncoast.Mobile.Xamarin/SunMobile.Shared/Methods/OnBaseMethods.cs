using SunMobile.Shared.Utilities.Web;
using SunBlock.DataTransferObjects.OnBase;
using System.Threading.Tasks;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Shared.Methods
{
	public class OnBaseMethods : SunBlockServiceBase
	{
		public Task<CheckImagesResponse> GetCheckImages(CheckImagesRequest request, object view)
		{       
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetCheckImages";
			var response = PostToSunBlock<CheckImagesResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;        
		}
	}
}