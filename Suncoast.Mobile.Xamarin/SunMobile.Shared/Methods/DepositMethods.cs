using System.Threading.Tasks;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor.Enums;
using SunBlock.DataTransferObjects.Mobile;
using SunBlock.DataTransferObjects.RemoteDeposits;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Utilities.Web;

namespace SunMobile.Shared.Methods
{
    public class DepositMethods : SunBlockServiceBase
    {
		public Task<IsRemoteDepositsEnabledResponse> IsRemoteDepositsEnabled(IsRemoteDepositsEnabledRequest request, object view)
        {       
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockRemoteDepositsServiceUrl + "IsRemoteDepositsEnabled";
            var response = PostToSunBlock<IsRemoteDepositsEnabledResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;        
        }

        /*
		public Task<GetMemberRemoteDepositsInfoResponse> GetMemberRemoteDepositsInfo(GetMemberRemoteDepositsInfoRequest request, object view)
        {       
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockRemoteDepositsServiceUrl + "GetMemberRemoteDepositsInfo";
            var response = PostToSunBlock<GetMemberRemoteDepositsInfoResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;        
        }
        */

        public Task<GetMemberRemoteDepositsInfoResponse> GetMemberRemoteDepositsInfo(GetMemberRemoteDepositsInfoRequest request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockRemoteDepositsServiceUrl + "v2/GetMemberRemoteDepositsInfo";
            var response = PostToSunBlock<GetMemberRemoteDepositsInfoResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

		public Task<MobileStatusResponse<DepositCheckResponse>> DepositCheck(MobileDeviceVerificationRequest<DepositCheckRequest> request, object view, object navigationController)
        {       
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v2/DepositCheck";
			var response = PostToSunBlock<MobileStatusResponse<DepositCheckResponse>>(url, request, SessionSettings.Instance.SunBlockToken, view, false, OutOfBandTransactionTypes.RemoteDeposit, navigationController);

            return response;        
        }

		public Task<RemoteDepositTransactionsViewModel> RetrieveRemoteDepositTransactions(RemoteDepositTransactionRequest request, object view)
		{       
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockRemoteDepositsServiceUrl + "RetrieveRemoteDepositTransactions";
			var response = PostToSunBlock<RemoteDepositTransactionsViewModel>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;        
		}

        public bool ValidateDepositRequest(DepositCheckRequest request)
        {
            bool returnValue = true;

            if (string.IsNullOrEmpty(request.DepositAccountNumber))
            {
                returnValue = false;
            }

            if (request.AmountInCents <= 0)
            {
                returnValue = false;
            }

            if (string.IsNullOrEmpty(request.FrontImageBase64))
            {
                returnValue = false;
            }

            if (string.IsNullOrEmpty(request.BackImageBase64))
            {
                returnValue = false;
            }

            return returnValue;
        }

		public Task<SetMemberRemoteDepositsInfoResponse> SetMemberRemoteDepositsInfo(SetMemberRemoteDepositsInfoRequest request, object view)
        {       
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockRemoteDepositsServiceUrl + "SetMemberRemoteDepositsInfo";
            var response = PostToSunBlock<SetMemberRemoteDepositsInfoResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;        
        }

        public bool ValidateSetMemberRemoteDepositsInfoRequest(SetMemberRemoteDepositsInfoRequest request)
        {
            bool returnValue = true;

			if ((string.IsNullOrEmpty(request.PhoneNumber) || !request.SmsAlertsEnabled) && string.IsNullOrEmpty(request.Email))
			{
				returnValue = false;
			}            

			if (!request.AgreedToTerms)
			{
				returnValue = false;
			}

            return returnValue;
        }
    }
}