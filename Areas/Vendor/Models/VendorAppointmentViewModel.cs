using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Areas.Vendor.Models
{
    public class VendorAppointmentViewModel
    {
        public int SchedId { get; set; }
        [Required]
        public int? Farmer { get; set; }
        [Required]
        public DateTime? BookingStartDate { get; set; }
        [Required]
        public DateTime? BookingEndDate { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Notes { get; set; }
    }
}
