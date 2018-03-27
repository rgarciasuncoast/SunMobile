using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay.V2
{
    [DataContract]
    public class Frequency
    {
        #region Declarations...

        #endregion

        #region Properties...

        [DataMember]
        public int FrequencyId { get; set; }

        [DataMember]
        public string FrequencyDescription { get; set; }

        #endregion

        #region Methods...

        #endregion
    }
}