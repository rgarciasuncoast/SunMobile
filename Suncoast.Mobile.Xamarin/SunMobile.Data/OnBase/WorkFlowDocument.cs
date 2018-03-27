using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class WorkFlowDocument : Document
    {

        /// <summary>
        /// Gets/sets the workflow document type.
        /// </summary>
        [DataMember]
        public string WorkFlowDocumentType { get; set; }

        /// <summary>
        /// Gets/sets the Html string for this workflow.
        /// </summary>
        [DataMember]
        public string HtmlString { get; set; }
    }
}
