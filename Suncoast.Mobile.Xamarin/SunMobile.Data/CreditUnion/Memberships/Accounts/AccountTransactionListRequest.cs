using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class AccountTransactionListRequest
    {
		[DataMember]
		public string MemberId { get; set; }
        [DataMember]
        public string Suffix { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
    }
}
