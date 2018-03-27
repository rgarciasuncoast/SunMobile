using System.Collections.Generic;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor.Enums;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Utilities.Web;

namespace SunMobile.Shared.Methods
{
	public class TransferMethods : SunBlockServiceBase
	{
		public Task<MobileStatusResponse> Transfer(TransferExecuteRequest request, object view, object navigationController)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v2/TransferExecute";
			var response = PostToSunBlock<MobileStatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view, false, OutOfBandTransactionTypes.HighRiskTransfer, navigationController);

			return response;
		}

        public Task<StatusResponse<List<TransferFavorite>>> GetTransferFavorites(object request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetTransferFavorites";
            var response = PostToSunBlock<StatusResponse<List<TransferFavorite>>>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<StatusResponse> SetTransferFavorites(List<TransferFavorite> request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/SetTransferFavorites";
            var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

		public bool ValidateTransferRequest(TransferExecuteRequest request)
		{
			bool returnValue = true;

			if (request.Amount <= 0) 
			{
				returnValue = false;
			}

			if (string.IsNullOrEmpty(request.Destination.Suffix)) 
			{
				returnValue = false;
			}

			if (string.IsNullOrEmpty(request.Source.Suffix)) 
			{
				returnValue = false;
			}

			return returnValue;
		}
	}
}