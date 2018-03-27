using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;
using SunBlock.DataTransferObjects.Mobile;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
using SunBlock.DataTransferObjects.Mobile.Model.OnBase;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Utilities.Web;
using SunMobile.Shared.Views;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor.Enums;
using SunMobile.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Products;

#if __IOS__
using SunMobile.iOS.Common;
#endif

#if __ANDROID__
using SunMobile.Droid;
using SunBlock.DataTransferObjects.Products;
#endif

namespace SunMobile.Shared.Methods
{
	public class AccountMethods : SunBlockServiceBase
    {
		public Task<AccountListTextViewModel> AccountList(AccountListRequest request, object view)
        {       
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/AccountList";
            var response = PostToSunBlock<AccountListTextViewModel>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;        
        }

		public Task<AccountListTextViewModel> RemoteDepositsAccountList(RemoteDepositsAccountListRequest request, object view)
        {       
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/RemoteDepositsAccountList";
            var response = PostToSunBlock<AccountListTextViewModel>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;        
        }

		public Task<AccountListTextViewModel> BillPaySourceAccountList(AccountListRequest request, object view)
        {       
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/BillPaySourceAccountList";
            var response = PostToSunBlock<AccountListTextViewModel>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;        
        }

		public Task<AccountListTextViewModel> TransferSourceAccountList(TransferSourceAccountListRequest request, object view)
        {       
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/TransferSourceAccountList";
            var response = PostToSunBlock<AccountListTextViewModel>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;        
        }

		public Task<AccountListTextViewModel> TransferTargetAccountList(TransferTargetAccountListRequest request, object view)
        {       
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/TransferTargetAccountList";
            var response = PostToSunBlock<AccountListTextViewModel>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;        
        }

		public Task<SubAccountsFundingAccountListResponse> SubAccountsFundingAccountList(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/SubAccountsFundingAccountList";
			var response = PostToSunBlock<SubAccountsFundingAccountListResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<TransactionListTextViewModel> AccountTransactionList(AccountTransactionListRequest request, object view)
        {       
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/AccountTransactionList";																				     
            var response = PostToSunBlock<TransactionListTextViewModel>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;        
        }

		public Task<NextTransactionResponse> NextAccountTransactionList(NextTransactionRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/NextAccountTransactionList";
			var response = PostToSunBlock<NextTransactionResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<MobileStatusResponse> GetRocketAccountsInformation(MobileDeviceVerificationRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetRocketAccountsInformation";
			var response = PostToSunBlock<MobileStatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

        // TODO: Remove this methods after iOS MFA is fixed.
        public Task<MobileStatusResponse<RocketCheckingResponse>> CreateRocketChecking(MobileDeviceVerificationRequest<RocketCheckingRequest> request, object view)
        {           
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/CreateRocketChecking";
            var response = PostToSunBlock<MobileStatusResponse<RocketCheckingResponse>>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }   

        public Task<MobileStatusResponse<RocketCheckingResponse>> CreateRocketChecking(MobileDeviceVerificationRequest<RocketCheckingRequest> request, object view, object viewToRUnAfterValidation)
		{			
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/CreateRocketChecking";
            var response = PostToSunBlock<MobileStatusResponse<RocketCheckingResponse>>(url, request, SessionSettings.Instance.SunBlockToken, view, false, OutOfBandTransactionTypes.RocketAccounts, null, viewToRUnAfterValidation);

			return response;
		}		

		public Task<MemberInformation> GetMemberInformation(MemberInformationRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetMemberInformation";
			var response = PostToSunBlock<MemberInformation>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse<string>> GetMemberEmailAddress(MemberInformationRequest request, object view)
		{       
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetMemberEmailAddress";																				     
			var response = PostToSunBlock<StatusResponse<string>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;        
		}

		public Task<MobileStatusResponse<TransactionDisputeInformationResponse>> GetTransactionDisputeInformation(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetTransactionDisputeInformation";
			var response = PostToSunBlock<MobileStatusResponse<TransactionDisputeInformationResponse>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse<string>> StoreAndScanDocument(StoreAndScanDocumentRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/StoreAndScanDocument";
			var response = PostToSunBlock<StatusResponse<string>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<MobileStatusResponse<List<TransactionDisputeHistoryItem>>> GetTransactionDisputeHistory(GetTransactionDisputeHistoryRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetTransactionDisputeHistory";
			var response = PostToSunBlock<MobileStatusResponse<List<TransactionDisputeHistoryItem>>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<MobileStatusResponse<string>> TransactionDisputeHistoryItemExists(TransactionDisputeHistoryItem request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/TransactionDisputeHistoryItemExists";
			var response = PostToSunBlock<MobileStatusResponse<string>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<UnityFormResponse> SubmitTransactionDispute(SubmitTransactionDisputeRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v2/SubmitTransactionDispute";
			var response = PostToSunBlock<UnityFormResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

        public Task<StatusResponse> UpdateProfileInformation(UpdateProfileRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/UpdateProfileInformation";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

        public Task<PayoffQuoteResponse> GetPayoffQuote(PayoffQuoteRequest request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetPayoffQuote";
            var response = PostToSunBlock<PayoffQuoteResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

        public Task<StatusResponse> EmailPayoffQuote(PayoffQuoteEmailRequest request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/EmailPayoffQuote";
            var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

		public void AddAnyMemberToAccountsViewModel(AccountListTextViewModel accountsViewModel)
		{
			var headerSection = new SunBlock.DataTransferObjects.UserInterface.MVC.HeaderSectionTextView<SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.Account>();
			headerSection.Items = new System.Collections.ObjectModel.Collection<SunBlock.DataTransferObjects.UserInterface.MVC.ItemSectionTextView<SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.Account>>();
			headerSection.HeaderText = "Other Accounts";
			var itemSection = new SunBlock.DataTransferObjects.UserInterface.MVC.ItemSectionTextView<SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.Account>();
			itemSection.ItemDescription = "Select Another Account";
			itemSection.ListItems = new SunBlock.DataTransferObjects.UserInterface.MVC.ListItemTextView();
			itemSection.ListItems.DisplayFieldNames = new System.Collections.ObjectModel.Collection<string>();
			itemSection.ListItems.DisplayFieldValues = new System.Collections.ObjectModel.Collection<string>();
			itemSection.ListItems.DisplayFieldNames.Add("");
			itemSection.ListItems.DisplayFieldNames.Add("");
			itemSection.ListItems.DisplayFieldValues.Add("");
			itemSection.ListItems.DisplayFieldValues.Add("");
			itemSection.Data = new SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.Account();
			itemSection.Data.AccountType = "Other Accounts";
			headerSection.Items.Add(itemSection);

			accountsViewModel.AccountSections.Add(headerSection);
		}

		public async Task<bool> HasTransactionAlreadyBeenDisputed(Transaction transaction, int memberId, object view)
		{
			var returnValue = false;

			try
			{
				var request = new TransactionDisputeHistoryItem
				{
					MemberId = memberId,
					DateOfTransaction = transaction.TransactionDate.ToString("yyyyMMdd"),
					TransactionId = transaction.SequenceNumber.ToString()
				};

				#if __IOS__
				var context = ((BaseViewController)view).View;
				#endif

				#if __ANDROID__
				var context = ((BaseListFragment)view).Activity;
				#endif

				var response = await TransactionDisputeHistoryItemExists(request, context);

				if (response != null && response.Success)
				{
					if (!string.IsNullOrEmpty(response.Result))
					{
						returnValue = true;
                        var message = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "932CA4A2-A082-4EB2-980C-FC725434EF9F", "Transaction disputed");
						var dateDisputed = DateTime.ParseExact(response.Result, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                        await AlertMethods.Alert(context, "SunMobile", $"{message} {dateDisputed}.", CultureTextProvider.OK());

						#if __IOS__
						//((BaseViewController)view).NavigationController.PopViewController(true);
						#endif

						#if __ANDROID__
						//NavigationService.NavigatePop();
						#endif
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "AccountMethods:HasTransactionAlreadyBeenDisputed");
			}

			return returnValue;
		}

		public GetDisputeInfoResponse GetDisputeInfo(Transaction transaction, string memberId, bool isCreditCard)
		{
			var returnValue = new GetDisputeInfoResponse { AllowDispute = false, DisputeType = "cardholder" };

			try
			{
				//Credit Card
				if (isCreditCard)
				{
					returnValue.AllowDispute = transaction.IsDisputable;
					returnValue.DisputeType = "cardholder";
				}

				//ACH
				else if (transaction.ActionCode == "W" && transaction.SourceCode == "E")
				{
					returnValue.AllowDispute = true;
					returnValue.DisputeType = "ach";
				}

				//ATM - Withdrawal or Deposit into an ATM
				else if ((transaction.ActionCode == "W" || transaction.ActionCode == "D") && transaction.SourceCode == "A" && transaction.MerchantName.ToUpper().IndexOf("SUNCOAST CREDIT UNION", StringComparison.Ordinal) >= 0)
				{
					returnValue.AllowDispute = true;
					returnValue.DisputeType = "atm";
				}

				//Cardholder - Non SCU ATM
				else if ((transaction.ActionCode == "W" || transaction.ActionCode == "D") && transaction.SourceCode == "A" && transaction.MerchantName.ToUpper().IndexOf("SUNCOAST CREDIT UNION", StringComparison.Ordinal) < 0)
				{
					returnValue.AllowDispute = true;
					returnValue.DisputeType = "cardholder";
				}

				//Cardholder - Billpay, Debit, POS
				else if (transaction.ActionCode == "W" && (transaction.ActionCode == "A" || transaction.SourceCode == "B" || transaction.SourceCode == "G" || transaction.SourceCode == "O"))
				{
					returnValue.AllowDispute = true;
					returnValue.DisputeType = "cardholder";
				}

				//Cardholder - Fee
				else if (transaction.ActionCode == "W" && transaction.SourceCode == "F")
				{
					if (transaction.Description.ToUpper().IndexOf("ATM FEE", StringComparison.Ordinal) >= 0 ||
						transaction.Description.ToUpper().IndexOf("ATM WITHDRAW FEE", StringComparison.Ordinal) >= 0 ||
						transaction.Description.ToUpper().IndexOf("DEBIT CARD FEE", StringComparison.Ordinal) >= 0)
					{
						returnValue.AllowDispute = true;
						returnValue.DisputeType = "cardholder";
					}

					if (transaction.MerchantName.ToUpper().IndexOf("VISA INTERNATIONAL SERVICE ASSESSMENT", StringComparison.Ordinal) >= 0)
					{
						returnValue.AllowDispute = true;
						returnValue.DisputeType = "cardholder";
					}
				}

				int result;
				int.TryParse(memberId, out result);

				if (result != GeneralUtilities.GetMemberIdAsInt())
				{
					returnValue.AllowDispute = false;
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "AccountMethods:GetDisputeInfo");
			}

			return returnValue;
		}

		public List<ListViewItem> FilterSelectDisputeTransactions(List<ListViewItem> listViewItems, Transaction disputedTransaction, string memberId, bool limitToDisputedMerchant)
		{
			for (int i = 0; i < listViewItems.Count; i++)
			{
				var transaction = (Transaction)listViewItems[i].Data;

				bool includeTransaction = true;

				// Check if dispute is not allowed.
				if (!GetDisputeInfo(transaction, memberId, listViewItems[i].Item5Text == "CreditCard").AllowDispute)
				{
					includeTransaction = false;
				}

				// Check if the cardnumbers don't match
				if (!string.IsNullOrEmpty(disputedTransaction.CardNumber) && (transaction.CardNumber == null || transaction.CardNumber != disputedTransaction.CardNumber))
				{
					includeTransaction = false;
				}

				// Check if we are limiting the merchant and see if the merchants don't match
				if (limitToDisputedMerchant)
				{
					if (disputedTransaction.MerchantName != null)
					{
						if (transaction.MerchantName == null || transaction.MerchantName.Split(' ')[0] != disputedTransaction.MerchantName.Split(' ')[0])
						{
							includeTransaction = false;
						}
					}
					else
					{
						if (transaction.Description.Split(' ')[0] != disputedTransaction.Description.Split(' ')[0])
						{
							includeTransaction = false;
						}
					}
				}

				// Don't include the disputed transaction
				if (transaction.PostingDate.Date == disputedTransaction.PostingDate.Date && transaction.SequenceNumber == disputedTransaction.SequenceNumber)
				{
					includeTransaction = false;
				}

				// Don't include ATM transactions
				if (transaction.SourceCode == "A" || transaction.MerchantName != null && transaction.MerchantName.ToUpper().IndexOf("SUNCOAST CREDIT UNION", StringComparison.Ordinal) >= 0)
				{
					includeTransaction = false;
				}

				if (!includeTransaction)
				{
					listViewItems.RemoveAt(i);
					i--; //We have to reiterate this position as we have removed it from the list and the next position will fill its place
				}
			}

			return listViewItems;
		}

		public bool DoTransactionsMatch(Transaction transactionA, Transaction transactionB)
		{
			var returnValue = false;

			if (transactionA.PostingDate == transactionB.PostingDate)
			{
				// Credit cards don't have sequence numbers.
				if (transactionA.SequenceNumber != 0)
                {
					if (transactionA.SequenceNumber == transactionB.SequenceNumber)
					{
						returnValue = true;
					}
				}
                else
                {
                    if (transactionA.Description == transactionB.Description && transactionA.TransactionAmount == transactionB.TransactionAmount)
                    {
                        returnValue = true;
                    }
                }
            }

			return returnValue;
		}

		public bool ValidateAnyMemberTransfer(AnyMemberInfo request)
		{
			bool returnValue = true;

			if (string.IsNullOrEmpty(request.Account))
			{
				returnValue = false;
			}

			if (string.IsNullOrEmpty(request.Suffix))
			{
				returnValue = false;
			}

			if (string.IsNullOrEmpty(request.LastName) && !request.IsJoint)
			{
				returnValue = false;
			}

			return returnValue;
		}

		public string ValidateSubmitTransactionDisputeRequest(SubmitTransactionDisputeRequest request, bool requireAdditionalTransaction)
		{
			var returnValue = string.Empty;

			if (request.DisputeType != "ach" && request.DisputeType != "atm" && request.Reason == 0)
			{
				returnValue += "Dispute Reason must be selected.\n";
			}
			if (string.IsNullOrEmpty(request.DaytimePhone))
			{
				returnValue += "Daytime Phone Number must be filled in.\n";
			}
			if (!string.IsNullOrEmpty(request.DaytimePhone) && request.DaytimePhone.Length < 10)
			{
				returnValue += "Daytime Phone Number must be ten digits.\n";
			}
			if (string.IsNullOrEmpty(request.AddressLine1))
			{
				returnValue += "Address Line 1 must be filled in.\n";
			}
			if (string.IsNullOrEmpty(request.City))
			{
				returnValue += "City must be filled in.\n";
			}
			if (string.IsNullOrEmpty(request.UnmaskedZipCode))
			{
				returnValue += "Zip Code must be filled in.\n";
			}
			if (!string.IsNullOrEmpty(request.UnmaskedZipCode) && request.UnmaskedZipCode.Length < 5)
			{
				returnValue += "Zip Code must be five digits.\n";
			}
			if (requireAdditionalTransaction && request.AdditionalTransactions.Count < 2)
			{
				returnValue += "At least one additional transaction must be selected.\n";
			}
            if (returnValue.Length > 0)
            {
                returnValue = returnValue.Substring(0, returnValue.Length - 1);
            }

			return returnValue;
		}

		public string ValidateUpdateProfileRequest(UpdateProfileRequest request)
		{
            var returnValue = string.Empty;

			if (string.IsNullOrEmpty(request.Address1))
			{
				returnValue += "Address 1 must be filled in.\n";
			}
			if (string.IsNullOrEmpty(request.City))
			{
				returnValue += "City must be filled in.\n";
			}
			if (string.IsNullOrEmpty(request.State))
			{
				returnValue += "State must be filled in.\n";
			}
            if (string.IsNullOrEmpty(request.ZipCode) || (request.ZipCode.Length != 5 && request.ZipCode.Length != 10))
			{
				returnValue += "Zip Code must be five or nine digits.\n";
			}
            if (!StringUtilities.StringUtilities.IsValidEmail(request.Email))
			{
				returnValue += "Invalid Email.\n";
			}
			if (string.IsNullOrEmpty(request.HomePhone) && string.IsNullOrEmpty(request.WorkPhone) && string.IsNullOrEmpty(request.CellPhone))
			{
				returnValue += "At least one contact phone number is required.\n";
			}
			if (returnValue.Length > 0)
			{
				returnValue = returnValue.Substring(0, returnValue.Length - 1);
			}

			return returnValue;
		}		

        public string GetAccountDescription(SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.Account account)
        {
            var returnValue = string.Empty;

            if (account.AccountCategory == "CreditCards" && account.Suffix.Length > 12)
            {
                returnValue = "XXXX-" + account.Suffix.Substring(12);
            }
            else if (account.AccountCategory == "Loans" && account.Suffix.Length > 5)
            {
                returnValue = account.Suffix;
            }
            else
            {
                returnValue = account.MemberId + "-" + account.Suffix;
            }

            return returnValue;
        }
    }
}