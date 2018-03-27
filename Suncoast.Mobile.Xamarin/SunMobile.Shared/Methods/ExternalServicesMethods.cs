using System;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.ExternalServices;
using SunBlock.DataTransferObjects.Geezeo;
using SunMobile.Shared.Data;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Utilities.Web;

namespace SunMobile.Shared.Methods
{
    public class ExternalServicesMethods : SunBlockServiceBase
    {
        public Task<IsMemberEnrolledInSunMoneyResponse> IsMemberEnrolledInSunMoney(IsMemberEnrolledInSunMoneyRequest request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v2/IsMemberEnrolledInSunMoney";
            var response = PostToSunBlock<IsMemberEnrolledInSunMoneyResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<SetMemberSunMoneyEnrollmentResponse> SetMemberSunMoneyEnrollment(SetMemberSunMoneyEnrollmentRequest request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v2/SetMemberSunMoneyEnrollment";
            var response = PostToSunBlock<SetMemberSunMoneyEnrollmentResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<RetrieveGeezeoSingleSignOnResponse> RetrieveGeezeoSingleSignOn(RetrieveGeezeoSingleSignOnRequest request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v2/RetrieveGeezeoSingleSignOn";
            var response = PostToSunBlock<RetrieveGeezeoSingleSignOnResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<LoanApplicationSsoResponse> RetrieveLoanApplicationSingleSignOn(LoanApplicationSsoRequest request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/RetrieveLoanApplicationSingleSignOn";
            var response = PostToSunBlock<LoanApplicationSsoResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<StatusResponse<string>> GetTrueCarUrl(object request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetTrueCarUrl";
            var response = PostToSunBlock<StatusResponse<string>>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<StatusResponse<string>> GetSuncoastRealtyUrl(object request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetSuncoastRealtyUrl";
            var response = PostToSunBlock<StatusResponse<string>>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public async Task<string> GetLoanCenterUrl(LoanCenterTypes loanType, object view, object viewController = null)
        {
            var returnValue = string.Empty;

            try
            {

                switch (loanType)
                {
                    case LoanCenterTypes.ApplyForLoan:
                        #if __IOS__
                        ((UIKit.UIViewController)viewController).Title = "Apply For A Loan";
                        #else
                        ((Droid.MainActivity)view).SetActionBarTitle("Apply For A Loan");
                        #endif

                        var retrieveLoanAppSsoRequest = new LoanApplicationSsoRequest
                        {
                            MemberId = GeneralUtilities.GetMemberIdAsInt()
                        };

                        var retrieveLoanAppSsoResponse = await RetrieveLoanApplicationSingleSignOn(retrieveLoanAppSsoRequest, view);

                        if (retrieveLoanAppSsoResponse?.LoanAppUrl != null)
                        {
                            returnValue = retrieveLoanAppSsoResponse.LoanAppUrl;
                        }

                        break;

                    case LoanCenterTypes.CarLoan:
                        #if __IOS__
                        ((UIKit.UIViewController)viewController).Title = "Find a Car";
                        #else
                        ((Droid.MainActivity)view).SetActionBarTitle("Find a Car");
                        #endif

                        var trueCarResponse = await GetTrueCarUrl(null, view);

                        if (trueCarResponse != null && trueCarResponse.Success && !string.IsNullOrEmpty(trueCarResponse.Result))
                        {
                            returnValue = trueCarResponse.Result;
                        }

                        break;

                    case LoanCenterTypes.HomeLoan:
                        #if __IOS__
                        ((UIKit.UIViewController)viewController).Title = "Buy A Home";
                        #else
                        ((Droid.MainActivity)view).SetActionBarTitle("Buy A Home");
                        #endif

                        var realtyResponse = await GetSuncoastRealtyUrl(null, view);

                        if (realtyResponse != null && realtyResponse.Success && !string.IsNullOrEmpty(realtyResponse.Result))
                        {
                            returnValue = realtyResponse.Result;
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Logging.Logging.Log(ex, "ExternalServicesMethods:GetLoanCetnerUrl");
            }

            return returnValue;
        }
    }
}