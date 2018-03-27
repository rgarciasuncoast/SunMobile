using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.RocketAccounts
{
    [DataContract]
    public class RocketEligibility : StatusResponse
    {
        [DataMember]
        public bool IsCheckingEligible { get; set; }
        [DataMember]
        public bool IsFundedEligible { get; set; }
    }
}