using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
	[DataContract]
	public enum EDocumentTypes
	{
		[EnumMember]
		AccountEStatements,
		[EnumMember]
		AccountEStatementsSpanish,
		[EnumMember]
		CreditCardEStatements,
		[EnumMember]
		MortgageEStatements,
		[EnumMember]
		ENotices,
		[EnumMember]
		TaxDocuments,
		[EnumMember]
		SignatureCardForm,
        [EnumMember]
        CreditCardAnnualEStatements
	}
}