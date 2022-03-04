using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wiggly.Entities
{
    public partial class Schedules
    {
        public int SchedId { get; set; }
        [Required]
        public int? Vendor { get; set; }
        public int? Farmer { get; set; }
        [Required]
        public DateTime? BookingStartDate { get; set; }
        [Required]
        public DateTime? BookingEndDate { get; set; }
        public DateTime? DateCreated { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Notes { get; set; }
    }
}
