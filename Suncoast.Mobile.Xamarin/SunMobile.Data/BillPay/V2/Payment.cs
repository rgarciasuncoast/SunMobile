using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Extensions;

namespace SunBlock.DataTransferObjects.BillPay.V2
{
    [DataContract]
    public class Payment
    {
        #region Declarations...

        #endregion

        #region Properties...

        [DataMember]
        public long PaymentId { get; set; }

        [DataMember]
        public long RecurringId
        {
            get;
            set;
        }

        [DataMember]
        public long MemberPayeeId { get; set; }

        [DataMember]
        public string PayeeName { get; set; }

        [DataMember]
        public string PayeeAlias { get; set; }

        [DataMember]
        public string PayeeAccountNumber { get; set; }

        [DataMember]
        public string DeliveryMethod { get; set; }

        [DataMember]
        public string DeliveryDays { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public int StatusCode { get; set; }

        [DataMember]
        public string SourceAccount { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public DateTime SendOn { get; set; }

        [DataMember]
        public DateTime DeliverBy { get; set; }

        [DataMember]
        public int RemainingPayments { get; set; }

        [DataMember]
        public bool IsRecurring { get; set; }

        [DataMember]
        public int FrequencyId { get; set; }

        [DataMember]
        public string Frequency { get; set; }

        [DataMember]
        public bool FrequencyIndefinite { get; set; }

        [DataMember]
        public string Memo { get; set; }

        [DataMember]
        public int MemberId { get; set; }

        [DataMember]
        public string SourceApplication { get; set; }

        [DataMember]
        public DateTime HistoricalDate { get; set; }

        #endregion

        #region Methods...

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {

            try
            {
                return this.SerializeToJsonSafe();
            }
            catch
            {
                return "Unable to Parse Payment Object";
            }
        }

        #endregion
    }
}