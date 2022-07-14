using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class MarketplaceItemLivestock
    {
        public int Id { get; set; }
        public Guid? MarketplaceItem { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public int? Quantity { get; set; }
        public decimal? Kilos { get; set; }
        public decimal? Price { get; set; }
    }
}
