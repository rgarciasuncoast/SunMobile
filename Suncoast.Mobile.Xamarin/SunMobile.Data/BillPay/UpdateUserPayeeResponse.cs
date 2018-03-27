using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class UpdateUserPayeeResponse : BillPayResponseBase
    {
        [DataMember]
        public Payee UserPayee { get; set; }
    }
}
