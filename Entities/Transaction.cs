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
        public DateTime? BookDate { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
