using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class TransferFavorite
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public TransferTarget Source { get; set; }
        [DataMember]
        public TransferTarget Destination { get; set; }
        [DataMember]
        public string SourceAccountDescription { get; set; }
        [DataMember]
        public decimal SourceBalance { get; set; }
        [DataMember]
        public decimal SourceAvailableBalance { get; set; }
        [DataMember]
        public string DestinationAccountDescription { get; set; }
        [DataMember]
        public decimal DestinationBalance { get; set; }
        [DataMember]
        public decimal DestinationAvailableBalance { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string LastEightOfAtmDebitCard { get; set; }
        [DataMember]
        public bool IsJoint { get; set; }
        [DataMember]
        public int SortOrder { get; set; }
        [DataMember]
        public string FriendlyFavoriteName { get; set; }
        [DataMember]
        public bool IsDirty { get; set; }
    }
}
