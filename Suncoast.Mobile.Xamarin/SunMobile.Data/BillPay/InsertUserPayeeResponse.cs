using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class InsertUserPayeeResponse : BillPayResponseBase
    {
        [DataMember]
        public Payee UserPayee { get; set; }
    }
}