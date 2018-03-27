using System.Runtime.Serialization;
namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    //TODO: do not use as display text
    [DataContract]
    public enum AccountTypes
    {
        [EnumMember]
        Shares,
        [EnumMember]
        Loans,
        [EnumMember]
        CreditCards,
        [EnumMember]
        Mortgages,
        [EnumMember]
        ShareCertificates,
        [EnumMember]
        IRAs,
        [EnumMember]
        ShareDrafts,
        [EnumMember]
        Certificates,
        [EnumMember]
        MoneyMarkets,
        [EnumMember]
        Clubs
    }
}
