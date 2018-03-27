using System.Runtime.Serialization;

namespace SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account
{
    [DataContract]
    public class UpdateProfileRequest
    {
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Address1 { get; set; }
        [DataMember]
        public string Address2 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
        [DataMember]
        public string HomePhone { get; set; }
        [DataMember]
        public string CellPhone { get; set; }
        [DataMember]
        public string WorkPhone { get; set; }   
        [DataMember]
        public string Email { get; set; }
    }
}