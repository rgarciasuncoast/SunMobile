using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Correspondence;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums;
using SunBlock.DataTransferObjects.Products;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Utilities.Web;

namespace SunMobile.Shared.Cards
{
	public class CardMethods : SunBlockServiceBase
	{
		public Task<GetTravelNotificationInfoResponse> GetTravelNotificationInfo(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetTravelNotificationInfo";
			var response = PostToSunBlock<GetTravelNotificationInfoResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> SubmitTravelNotifications(SubmitTravelNotificationsRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/SubmitTravelNotifications";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse<List<BankCard>>> CardList(CardListRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/CardList";
			var response = PostToSunBlock<StatusResponse<List<BankCard>>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<CardImageResponse> GetCardImages(CardImageRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetCardImages";
			var response = PostToSunBlock<CardImageResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

        public Task<StatusResponse> RequestCustomCard(CustomCardRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/RequestCustomCard";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public string GetCardDisplayName(BankCard card)
		{
			var returnValue = string.Empty;

			if (!string.IsNullOrWhiteSpace(card.CardAccountNumber) && card.CardAccountNumber.Length >= 4)
			{
				returnValue = "xxxx-" + card.CardAccountNumber.Substring(card.CardAccountNumber.Length - 4, 4);

				var cardType = string.Empty;

				switch (card.CardType)
				{
					case CardTypes.CreditCard:
						cardType = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "17783198-589D-44DC-A3F8-40A0E32522B9", "Credit Card");
						returnValue += " - " + cardType;
						break;
					case CardTypes.ProprietaryCard:
						cardType = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "F02EAA6F-CB04-491A-8252-440DBAF9F856", "ATM Card");
						returnValue += " - " + cardType;
						break;
					case CardTypes.VisaDebitCard:
						cardType = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "F74D32D8-EFCA-4798-BF46-05175CFACE02", "Check Card");
						returnValue += " - Visa " + cardType;
						break;
				}
			}

			return returnValue;
		}

		public async Task<bool> CheckRaysCardEligibility(object view)
		{
			bool returnValue = false;

			try
			{				
				var request = new CardListRequest
				{
					ExcludeAtmCards = true,
					IncludeClosedCards = false,
				};

				var response = await CardList(request, view);

				if (response != null && response.Success)
				{
                    var cardList = response.Result.Where(x => x.IsEligibleForRaysCard).ToList();
					returnValue = cardList.Count > 0;

					if (!returnValue)
					{
                        var message = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "91e273e8-03bf-4fdd-aee0-57bbeabf0d49", "You don't have any cards eligible to be replaced with a Tampa Bay Rays Card.");
                        await AlertMethods.Alert(view, "SunMobile", message, "OK");
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "CardMethods:CheckRaysCardEligibility");
			}

			return returnValue;
		}

		public bool ValidateSubmitTravelNotificationsRequest(SubmitTravelNotificationsRequest request)
		{
			bool returnValue = true;

			if (request.MemberId <= 0)
			{
				returnValue = false;
			}

			if (request.Cards.Count <= 0)
			{
				returnValue = false;
			}

			if (string.IsNullOrEmpty(request.Locations))
			{
				returnValue = false;
			}

			if (string.IsNullOrEmpty(request.StartDate))
			{
				returnValue = false;
			}

			if (string.IsNullOrEmpty(request.EndDate))
			{
				returnValue = false;
			}

			return returnValue;
		}
	}
}