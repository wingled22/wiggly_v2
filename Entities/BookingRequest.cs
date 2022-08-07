using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class BookingRequest
    {
        public int Id { get; set; }
        public Guid? Item { get; set; }
        public int? Vendor { get; set; }
        public int? Farmer { get; set; }
        public int? Amount { get; set; }
        public int? Quantity { get; set; }
        public string Status { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
