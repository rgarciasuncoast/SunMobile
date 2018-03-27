using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor.Enums;
using SunBlock.DataTransferObjects.BillPay.V2;
using SunBlock.DataTransferObjects.CreditUnion.HolidaySchedule;
using SunBlock.DataTransferObjects.Mobile;
using SunMobile.Shared.Utilities.Dates;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Utilities.Web;

namespace SunMobile.Shared.Methods
{
	public class BillPayMethods : SunBlockServiceBase
	{
        public Task<StatusResponse<Payee>> AddPayee(Payee request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockBillPayServiceUrl + "AddPayee";
            var response = PostToSunBlock<StatusResponse<Payee>>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<MobileStatusResponse<StatusResponse<Payment>>> AddPayment(MobileDeviceVerificationRequest<Payment> request, object view, object navigationController)
        {
            request.Request.DeliverBy = request.Request.DeliverBy.LocalToEasternToUtc();
            request.Request.SendOn = request.Request.SendOn.LocalToEasternToUtc();

            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockBillPayServiceUrl + "AddPayment";
            var response = PostToSunBlock<MobileStatusResponse<StatusResponse<Payment>>>(url, request, SessionSettings.Instance.SunBlockToken, view, false, OutOfBandTransactionTypes.BillPayment, navigationController);

            return response;
        }

        public Task<StatusResponse<List<Payment>>> CancelPayment(Payment request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockBillPayServiceUrl + "CancelPayment";
            var response = PostToSunBlock<StatusResponse<List<Payment>>>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<StatusResponse<List<Payment>>> GetHistoryPayments(PaymentSearchRequest request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockBillPayServiceUrl + "GetHistoryPayments";
            var response = PostToSunBlock<StatusResponse<List<Payment>>>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<List<Holiday>> GetHolidays(object request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockBillPayServiceUrl + "GetHolidays";
            var response = PostToSunBlock<List<Holiday>>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<StatusResponse<List<Payee>>> GetPayees(object request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockBillPayServiceUrl + "GetPayees";
            var response = PostToSunBlock<StatusResponse<List<Payee>>>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<StatusResponse<List<Frequency>>> GetPaymentFrequencies(object request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockBillPayServiceUrl + "GetPaymentFrequencies";
            var response = PostToSunBlock<StatusResponse<List<Frequency>>>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<StatusResponse<List<Payment>>> GetPendingPayments(object request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockBillPayServiceUrl + "GetPendingPayments";
            var response = PostToSunBlock<StatusResponse<List<Payment>>>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<SunBlock.DataTransferObjects.BillPay.IsMemberEnrolledInBillPayResponse> IsMemberEnrolledInBillPay(object request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockBillPayServiceUrl + "IsMemberEnrolledInBillPay";
            var response = PostToSunBlock<SunBlock.DataTransferObjects.BillPay.IsMemberEnrolledInBillPayResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<StatusResponse<Payee>> UpdatePayee(object request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockBillPayServiceUrl + "UpdatePayee";
            var response = PostToSunBlock<StatusResponse<Payee>>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<MobileStatusResponse<StatusResponse<Payment>>> UpdatePayment(MobileDeviceVerificationRequest<Payment> request, object view, object navigationController)
        {
            request.Request.DeliverBy = request.Request.DeliverBy.LocalToEasternToUtc();
            request.Request.SendOn = request.Request.SendOn.LocalToEasternToUtc();

            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockBillPayServiceUrl + "UpdatePayment";
            var response = PostToSunBlock<MobileStatusResponse<StatusResponse<Payment>>>(url, request, SessionSettings.Instance.SunBlockToken, view, false, OutOfBandTransactionTypes.BillPayment, navigationController);

            return response;
        }

		public Task<StatusResponse<bool>> DoesPayeeHavePendingPayments(DoesPayeeHavePendingPaymentsRequest request, object view)
		{    	
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockBillPayServiceUrl + "DoesPayeeHavePendingPayments";
			var response = PostToSunBlock<StatusResponse<bool>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

        public Task<SunBlock.DataTransferObjects.BillPay.MemberBillPayEnrollementResponse> UpdateBillPayEnrollment(SunBlock.DataTransferObjects.BillPay.MemberBillPayEnrollementRequest request, object view)
		{    	
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "UpdateBillPayEnrollment";
			var response = PostToSunBlock<SunBlock.DataTransferObjects.BillPay.MemberBillPayEnrollementResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}		

		public async Task<List<DateTime>> GetHolidaysAsList(object request, object view)
		{
			var holidays = SessionSettings.Instance.Holidays;

		    if (holidays?.Count == 0)
		    {
		        var response = await GetHolidays(request, view);

		        if (response != null)
		        {
		            holidays.AddRange(response.Select(holiday => holiday.DateObserved.Date));
		        }

                SessionSettings.Instance.Holidays = holidays;
		    }

		    return holidays;
		}		

		public bool ValidatePaymentRequest(Payment request)
		{
			bool returnValue = true;

            if (string.IsNullOrEmpty(request.SourceAccount))
			{
	    		returnValue = false;
			}

            if (request.MemberPayeeId <= 0)
			{
				returnValue = false;
			}

			if (request.Amount <= 0)
			{
				returnValue = false;
			}

            if (request.SendOn == DateTime.MinValue)
			{
				returnValue = false;
			}

            if (request.IsRecurring && request.RemainingPayments <= 0 && !request.FrequencyIndefinite)
            {
                returnValue = false;
            }

			return returnValue;
		}       

        public async Task<DateTime> GetFirstPossibleDeliveryDate(bool isRecurring, string paymentMethod, object View)
        {
            DateTime returnValue;
            int businessDays;
            int days = 0;
            int count = 1;

            if (string.IsNullOrEmpty(paymentMethod))
            {
                paymentMethod = string.Empty;
            }

            //returnValue = DateTime.Today.AddDays(1);

            //if (isRecurring)
            //{
                var holidays = await GetHolidaysAsList(null, View);

                businessDays = isRecurring ? paymentMethod.ToLower() == "electronic" ? 3 : 7 : 1;

                while (days < businessDays)
                {
                    var testDate = DateTime.Today.AddDays(count);

                    if (testDate.DayOfWeek != DayOfWeek.Saturday && testDate.DayOfWeek != DayOfWeek.Sunday && !holidays.Contains(testDate))
                    {
                        days++;
                    }

                    count++;
                }

                returnValue = DateTime.Today.AddDays(count - 1);
            //}

            return returnValue;
        }

        public async Task<DateTime> CalculateDeliverByDate(DateTime sendOnDate, string paymentMethod, object View)
        {
            DateTime returnValue;
            int businessDays;
            int days = 0;
            int count = 1;

            if (string.IsNullOrEmpty(paymentMethod))
            {
                paymentMethod = string.Empty;
            }

            businessDays = paymentMethod.ToLower() == "electronic" ? 2 : 6;
            var holidays = await GetHolidaysAsList(null, View);

            while (days < businessDays)
            {
                var testDate = sendOnDate.AddDays(count);

                if (testDate.DayOfWeek != DayOfWeek.Saturday && testDate.DayOfWeek != DayOfWeek.Sunday && !holidays.Contains(testDate))
                {
                    days++;
                }

                count++;
            }

            returnValue = sendOnDate.AddDays(count - 1);

            return returnValue;
        }

        public async Task<DateTime> CalculateInitialSendOnDate(DateTime deliveryByDate, string paymentMethod, object View)
        {
            DateTime returnValue;
            int businessDays;
            int days = 0;
            int count = 1;

            if (string.IsNullOrEmpty(paymentMethod))
            {
                paymentMethod = string.Empty;
            }

            businessDays = paymentMethod.ToLower() == "electronic" ? 2 : 6;
            var holidays = await GetHolidaysAsList(null, View);

            while (days < businessDays)
            {
                var testDate = deliveryByDate.AddDays(count * -1);

                if (testDate.DayOfWeek != DayOfWeek.Saturday && testDate.DayOfWeek != DayOfWeek.Sunday && !holidays.Contains(testDate))
                {
                    days++;
                }

                count++;
            }

            returnValue = deliveryByDate.AddDays((count - 1) * -1);

            return returnValue;
        }


        public bool CanPaymentBeEditedOrCanceled(Payment payment)
        {
            var returnValue = false;

            if (payment.StatusCode == 100 && payment.SendOn > DateTime.Today)
            {
                returnValue = true;
            }

            return returnValue;
        }

        public async Task<Tuple<List<Frequency>, string>> GetFrequencies(Payment paymentToEdit, object View)
        {
            var returnValue = new Tuple<List<Frequency>, string>(new List<Frequency>(), string.Empty);
                
            try
            {        
                var frequencies = await GetPaymentFrequencies(null, View);

                if (frequencies != null && frequencies.Success && frequencies.Result != null)
                {
                    var frequenciesList = frequencies.Result;
                    var singlePaymentString = frequenciesList[0].FrequencyDescription;

                    returnValue = new Tuple<List<Frequency>, string>(frequenciesList, singlePaymentString);

                    if (paymentToEdit != null && paymentToEdit.Frequency != returnValue.Item2) 
                    {
                        returnValue.Item1.RemoveAt(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Logging.Log(ex, "BillPaySchedulePaymentFragment:GetFrequencies");
            }

            return returnValue;
        }      
	}
}