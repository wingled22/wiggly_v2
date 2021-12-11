using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class Schedules
    {
        public int SchedId { get; set; }
        public int? Vendor { get; set; }
        public int? Farmer { get; set; }
        public DateTime? BookingStartDate { get; set; }
        public DateTime? BookingEndDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
    }
}
