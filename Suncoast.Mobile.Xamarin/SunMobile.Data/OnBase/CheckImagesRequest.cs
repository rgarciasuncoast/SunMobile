using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public class CheckImagesRequest
    {

        #region Declarations...

        #endregion

        #region Properties...

        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string Suffix { get; set; }
        [DataMember]
        public int CheckNumber { get; set; }
        [DataMember]
        public string DocumentId { get; set; }
        [DataMember]
        public bool ExcludeCheckImages { get; set; }
        [DataMember]
        public bool ReturnAllChecks { get; set; }
        [DataMember]
        public string RemoteDepositTraceNumber { get; set; }
        [DataMember]
        public DateTime QueryBeginTimeUtc { get; set; }
        [DataMember]
        public DateTime QueryEndTimeUtc { get; set; }

        #endregion

        #region Methods...

        #endregion
    }
}