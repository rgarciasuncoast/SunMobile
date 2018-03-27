using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.ExternalServices
{
    [DataContract]
    public class RetrieveCardRewardsSingleSignOnResponse : StatusResponse
    {
        #region Declarations...

        #endregion

        #region Properties...
        [DataMember]
        public string RedirectUrl { get; set; }

        #endregion

        #region Methods...

        #endregion
    }
}