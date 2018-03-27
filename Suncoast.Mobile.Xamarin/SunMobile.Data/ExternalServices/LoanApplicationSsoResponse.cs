using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.ExternalServices
{
    [DataContract]
    public class LoanApplicationSsoResponse : StatusResponse
    {

        #region Declarations...

        #endregion

        #region Properties...

        [DataMember]
        public string SsoToken { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string HomePhone { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string Zip { get; set; }
        [DataMember]
        public string DateOfBirth { get; set; }
        [DataMember]
        public string TaxId { get; set; }
		[DataMember]
		public string LoanAppUrl { get; set; }

        #endregion

        #region Methods...

        #endregion
    }

}
