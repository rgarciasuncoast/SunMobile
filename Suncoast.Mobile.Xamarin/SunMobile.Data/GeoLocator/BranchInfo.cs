using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.GeoLocator
{
    [DataContract]
    public class BranchInfo : Location
    {

        [DataMember]
        public int BranchId { get; set; }

        [DataMember]
        public int ApproBranchId { get; set; }


        [DataMember]
        public string BranchName { get; set; }

        [DataMember]
        public string Telephone { get; set; }

        [DataMember]
        public string Fax { get; set; }

        [DataMember]
        public string Extension { get; set; }

        [DataMember]
        [SensitiveData]
        public string Manager { get; set; }

        [DataMember]
        [SensitiveData]
        public string ManagerTitle { get; set; }

        [DataMember]
        [SensitiveData]
        public string Home { get; set; }

        [DataMember]
        [SensitiveData]
        public string Cell { get; set; }

        [DataMember]
        [SensitiveData]
        public string Rvp { get; set; }

        [DataMember]
        public bool Drivethrough { get; set; }

        [DataMember]
        public bool SafeDeposit { get; set; }
    }
}
