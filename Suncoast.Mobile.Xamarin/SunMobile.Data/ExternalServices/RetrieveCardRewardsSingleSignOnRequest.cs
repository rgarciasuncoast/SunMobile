using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.ExternalServices
{
    [DataContract]
    public class RetrieveCardRewardsSingleSignOnRequest
    {
        #region Declarations...


        #endregion

        #region Properties...
        [DataMember]
        public string PaymentCardAccountNumber { get; set; }
        [DataMember]
        public int MemberId { get; set; }

        #endregion

        #region Methods...

        #endregion
    }
}