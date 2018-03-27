using SunBlock.DataTransferObjects.Attributes;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Products
{
	[DataContract]
	public class CardImageResponse : StatusResponse
	{
		[DataMember]
		public List<CardInfo> CardImages { get; set; }
	}

	[DataContract]
	public class CardInfo
	{
		[DataMember]
		public string CardName { get; set; }

		[DataMember]
		public string CardType { get; set; } // Debit or Credit Enum String From CardImageTypes

		[DataMember]
		public byte[] CardImage { get; set; }

		[DataMember]
		[Queryable]
		public bool IsRaysCard { get; set; }

		[DataMember]
		public string ServiceCode { get; set; }
	}
}
