using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class LoginResponse : StatusResponse
    {
        [DataMember]
        public string LoginState { get; set; }
        [DataMember]
        public DateTime PasswordChangedDate { get; set; }
    }
}
