using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class GetUserPayeeResponse : BillPayResponseBase
    {
        [DataMember]
        public List<Payee> Payees { get; set; }
    }
}