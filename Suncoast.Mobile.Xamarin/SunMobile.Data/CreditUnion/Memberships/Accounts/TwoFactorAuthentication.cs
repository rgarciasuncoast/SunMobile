using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    public class TwoFactorAuthentication
    {
        [DataMember]
        public Guid SessionId { get; set; }
        
        [DataMember]
        public Guid UserId { get; set; }
        
        [DataMember]
        [SensitiveData]
        public string Code { get; set; }
        
        [DataMember]
        public DateTime DateCreated { get; set; }
        
        [DataMember]
        public string ExpirationMinutes { get; set; }
        
        [DataMember]
        public int Attempts { get; set; }
    }
}
