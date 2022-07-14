using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class TransactionSubItem
    {
        public int Id { get; set; }
        public int? TransactionId { get; set; }
        public int? SubItemId { get; set; }
        public string Units { get; set; }
        public string Category { get; set; }
        public decimal? Kilos { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public decimal? SubTotal { get; set; }
    }
}
