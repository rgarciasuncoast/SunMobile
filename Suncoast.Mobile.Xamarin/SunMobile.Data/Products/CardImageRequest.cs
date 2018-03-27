using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Products
{
	[DataContract]
	public class CardImageRequest
	{
		[DataMember]
		public string CardType { get; set; } // Debit or Credit Enum String From CardImageTypes

		[DataMember]
		public bool JustRaysCards { get; set; }
	}
}