using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class SubmitElectronicFormRequest
    {
        /// <summary>
        /// Eform Field Keys and Values to be Filled
        /// </summary>
        [DataMember]
        public Collection<OnBaseKey> EFormKeyValues { get; set; }

        /// <summary>
        /// Path to eForm Template
        /// *Required - Usually points to Enterprise Document Repository in Sharepoint
        /// </summary>
        [DataMember]
        public byte[] EFormData { get; set; }

        /// <summary>
        /// EForm that will be Stored in OnBase
        /// </summary>
        [DataMember]
        public WorkFlowDocument ElectronicForm { get; set; }

        /// <summary>
        /// True if the EForm has been successfully stored in OnBase.
        /// </summary>
        [DataMember]
        public bool IsElectronicFormStored { get; set; }

    }
}
