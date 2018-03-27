using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocuSign
{
    [DataContract]
    public enum SignerTypes
    {
        [EnumMember]
        NotDefined = 0,

        [EnumMember]
        Primary = 1,

        [EnumMember]
        Spouse = 2,

        [EnumMember]
        CoSigner = 3,

        [EnumMember]
        Joint = 4,

        [EnumMember]
        AuthorizedUser = 5,

        [EnumMember]
        Withdrawn = 6

    }

    [DataContract]
    public class SignerInformation
    {
        [DataMember]
        public string Ssn { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public SignerTypes SignerType { get; set; }
        [DataMember]
        public string AccessCode { get; set; }
        [DataMember]
        public int ClientUserId { get; set; }
    }
}