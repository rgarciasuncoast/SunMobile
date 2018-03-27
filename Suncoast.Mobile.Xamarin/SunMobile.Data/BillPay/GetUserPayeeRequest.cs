using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.BillPay
{
    [DataContract]
    public class GetUserPayeeRequest
    {
        [DataMember]
        public int MemberId { get; set; }
    }
}