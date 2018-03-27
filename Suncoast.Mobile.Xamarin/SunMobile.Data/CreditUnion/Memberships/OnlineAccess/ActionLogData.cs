using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;
using SunBlock.DataTransferObjects.Collections;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class ActionLogData
    {
        [DataMember]
        public Guid Id { get; set; }

        [Queryable]
        [SensitiveData]
        [DataMember]
        public string MemberId { get; set; }

        [Queryable]     
        [DataMember]
        public DateTime ActionDate { get; set; }

        [DataMember]
        public string HttpMethod { get; set; }

        [DataMember]
        [SensitiveData]
        public string ServerIpAddress { get; set; }

        [Queryable]
        [DataMember]
        public string ClientIpAddress { get; set; }
       
        [DataMember]
        public string ClientBrowserHeader { get; set; }

        [Queryable]
        [DataMember]
        public Guid SessionGuid { get; set; }

        [Queryable]
        [DataMember]
        public string LogType { get; set; }
        
        [DataMember]
        public NameValueCollection RequestInputValues { get; set; }
        
        [DataMember]
        public List<Exception> Exceptions{ get; set; }
        
        [DataMember]
        public List<string> CustomLogs{ get; set; }

        [DataMember]
        public NameValueCollection ModelStateErrors { get; set; }

        [DataMember]
        public string ResponseOutputValues { get; set; }

        [DataMember]
        public string Result { get; set; }

        [DataMember]
        public string HttpStatusCode { get; set; }
    }
}
