using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class AlertSettings
    {
        [DataMember]
        public DateTime LastUpdatedTimeUtc { get; set; }
        [DataMember]
        [SensitiveData]
        [Queryable]
        public string AlertEmail { get; set; }
        [DataMember]
        [SensitiveData]
        [Queryable]
        public string SmsPhoneNumber { get; set; }
        [DataMember]
        public SmsEmailProvider SmsProvider { get; set; }
        [DataMember]
        public bool AlertEnabled { get; set; }
        /// <summary>
        /// SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess.AlertTypes Enum
        /// </summary>
        [DataMember]
        public string AlertType { get; set; }
    }
}
