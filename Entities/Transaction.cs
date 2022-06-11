using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public int? Vendor { get; set; }
        public int? Farmer { get; set; }
        public string Status { get; set; }
        public string TypeOfLivestock { get; set; }
        public int? Quantity { get; set; }
        public decimal? Kilos { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentType { get; set; }
        public string ProofOfPayment { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? BookDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid? BookingId { get; set; }
    }
}
