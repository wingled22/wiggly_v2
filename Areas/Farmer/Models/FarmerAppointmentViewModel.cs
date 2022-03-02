using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Areas.Farmer.Models
{
    public class FarmerAppointmentViewModel
    {
        public int SchedId { get; set; }
        public int? Vendor { get; set; }
        public DateTime? BookingStartDate { get; set; }
        public DateTime? BookingEndDate { get; set; }
        public string Status { get; set; }
    }
}
