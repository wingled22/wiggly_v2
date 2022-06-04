using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Models
{
    public class BookingRequestViewModel
    {
        public int Id { get; set; }
        public Guid? Item { get; set; }
        public string Message { get; set; }
        public string Address { get; set; }
        public string Long { get; set; }
        public string Lat { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public int Kilos { get; set; }
        public decimal Amount{ get; set; }
        public string Status { get; set; }

        public DateTime? DateCreated { get; set; }
    }
}
