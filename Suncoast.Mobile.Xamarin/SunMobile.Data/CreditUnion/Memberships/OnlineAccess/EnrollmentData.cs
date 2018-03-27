using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class EnrollmentData
    {
        [DataMember]
        public bool IsEnrolled { get; set; }

        [DataMember]
        public DateTime LastEnrollmentChange { get; set; }

        [DataMember]
        public string EnrollmentArgument { get; set; }

    }
}
