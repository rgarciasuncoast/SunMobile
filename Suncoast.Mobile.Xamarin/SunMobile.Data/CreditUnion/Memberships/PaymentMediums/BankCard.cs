using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums
{
	[DataContract]
	public class BankCard
	{
		/// <summary>
		/// Gets or sets the card type.
		/// </summary>
		[DataMember]
		public CardTypes CardType
		{ get; set; }


		/// <summary>
		/// Gets or sets the card type as seen by the Host.
		/// </summary>
		[DataMember]
		public int CardHostType
		{ get; set; }

		/// <summary>
		/// Gets or sets the card holders name.
		/// </summary>
		[DataMember]
		public string CardHolderName
		{ get; set; }

		/// <summary>
		/// Gets or sets the card capture code.
		/// </summary>
		/// <example>
		/// ReturnCard = 0,
		/// CaptureCard = 1
		/// </example>
		[DataMember]
		public CardCaptureCodes CardCaptureCode
		{ get; set; }

		/// <summary>
		/// Gets or sets the cards member number.
		/// </summary>
		[DataMember]
		public Int32 CardMemberNumber
		{ get; set; }

		/// <summary>
		/// Gets or sets the card status.
		/// </summary>
		/// <example>
		/// NotActive = 0,
		/// Open = 1,
		/// Lost = 2,
		/// Stolen = 3,
		/// Restricted = 4,
		/// Expired = 8,
		/// Closed = 9 
		///</example>
		[DataMember]
		public CardStatus CardStatus
		{ get; set; }

		/// <summary>
		/// Gets or sets the Suffix1.
		/// </summary>
		[DataMember]
		public string Suffix1
		{ get; set; }

		/// <summary>
		/// Gets or sets the account type for suffix 1
		/// </summary>
		[DataMember]
		public string Suffix1Type
		{ get; set; }

		/// <summary>
		/// Gets or sets the Suffix2.
		/// </summary>
		[DataMember]
		public string Suffix2
		{ get; set; }

		/// <summary>
		/// Gets or sets the account type for suffix 2
		/// </summary>
		[DataMember]
		public string Suffix2Type
		{ get; set; }

		/// <summary>
		/// Gets or sets the Suffix3.
		/// </summary>
		[DataMember]
		public string Suffix3
		{ get; set; }

		/// <summary>
		/// Gets or sets the account type for suffix 3
		/// </summary>
		[DataMember]
		public string Suffix3Type
		{ get; set; }

		/// <summary>
		/// Gets or sets the Suffix3.
		/// </summary>
		[DataMember]
		public string Suffix4
		{ get; set; }

		/// <summary>
		/// Gets or sets the account type for suffix 4
		/// </summary>
		[DataMember]
		public string Suffix4Type
		{ get; set; }

		/// <summary>
		/// Gets or sets the VIPCode.
		/// </summary>
		[DataMember]
		public string VIPCode
		{ get; set; }

		/// <summary>
		/// Gets or sets the Card Account Number.
		/// </summary>
		[DataMember]
		public string CardAccountNumber
		{ get; set; }

		/// <summary>
		/// Gets or sets the CardCount.
		/// </summary>
		[DataMember]
		public Int32 CardCount
		{ get; set; }

		/// <summary>
		/// Gets or sets the StockCode.
		/// </summary>
		/// <example>
		/// NotDefined,
		/// ATMCardAccountNumber,
		/// VISAStockBlank,
		/// VISAStock,
		/// VISAGold
		/// </example>
		[DataMember]
		public CardStockCodes StockCode
		{ get; set; }

		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public bool IsValid { get; set; }

		[DataMember]
		public bool IsChecklessChecking { get; set; }

		[DataMember]
		public bool IsCompromised { get; set; }

		[DataMember]
		public bool IsEligibleForRaysCard { get; set; }

		[DataMember]
		public List<int> ServiceCodes { get; set; }

		public void SetDisplayName()
		{
			if (string.IsNullOrWhiteSpace(CardAccountNumber) || CardAccountNumber.Length < 4)
				return;
			DisplayName = "xxxx-xxxx-xxxx-" + CardAccountNumber.Substring(CardAccountNumber.Length - 4, 4);

			switch (CardType)
			{
				case CardTypes.CreditCard:
					DisplayName += " - Credit Card";
					break;
				case CardTypes.ProprietaryCard:
					DisplayName += " - ATM Card";
					break;
				case CardTypes.VisaDebitCard:
					DisplayName += " - Visa Check Card";
					break;


			}
		}
	}
}
