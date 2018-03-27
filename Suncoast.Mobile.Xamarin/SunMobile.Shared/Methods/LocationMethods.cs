using System.Collections.Generic;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects.GeoLocator;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Utilities.Web;

namespace SunMobile.Shared.Methods
{
	public class LocationMethods : SunBlockServiceBase
	{
		public Task<List<LocationInfo>> GetLocations(Query request, object view)
		{    	
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockAnalyzeUrl + "v2/Locations";
			var response = PostToSunBlock<List<LocationInfo>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}
	}
}