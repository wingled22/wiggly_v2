using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class MarketPlace
    {
        public Guid Id { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Total { get; set; }
        public int? Kilos { get; set; }
        public string Address { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string BuyOrSell { get; set; }
        public string Category { get; set; }
        public int? User { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
    }
}
