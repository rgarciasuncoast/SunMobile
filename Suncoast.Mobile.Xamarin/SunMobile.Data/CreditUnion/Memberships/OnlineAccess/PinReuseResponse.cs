using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    [DataContract]
    public class PinReuseResponse
    {
        [DataMember]
        public bool HasPinBeenUsedWithinRestrictedReusePeriod { get; set; }

        [DataMember]
        public int PeriodInMonths { get; set; }
    }
}
