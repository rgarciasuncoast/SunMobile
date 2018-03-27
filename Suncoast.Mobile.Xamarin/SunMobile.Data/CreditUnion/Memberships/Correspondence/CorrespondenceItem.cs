using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Collections;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Correspondence
{
    [DataContract]
    public class CorrespondenceItem
    {
        [DataMember]
        public string ItemType { get; set; }

        [DataMember]
        public string CorrespondenceName { get; set; }

        [DataMember]
        public string TextualContent { get; set; }

        [DataMember]
        public string HtmlContent { get; set; }

        [DataMember]
        public string LastUserAction { get; set; }

        [DataMember]
        public bool AcceptanceOptional { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public NameValueCollection Data { get; set; }

        [DataMember]
        public bool IsSuccessful { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }


    }
}
