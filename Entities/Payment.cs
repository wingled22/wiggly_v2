using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class Payment
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal? Total { get; set; }
        public string Status { get; set; }
        public int? Transaction { get; set; }
        public int? Vendor { get; set; }
        public int? Farmer { get; set; }
        public string ProofPaymentVendor { get; set; }
        public string ProofPaymentFarmer { get; set; }
        public decimal? Amount { get; set; }
    }
}
