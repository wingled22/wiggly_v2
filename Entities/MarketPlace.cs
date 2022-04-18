using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class MarketPlace
    {
        public Guid Id { get; set; }
        public string Caption { get; set; }
        public string BuyOrSell { get; set; }
        public string Category { get; set; }
        public int? User { get; set; }
    }
}
