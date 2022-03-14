using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class Kilos
    {
        public int Id { get; set; }
        public int? Transaction { get; set; }
        public int? Vendor { get; set; }
        public int? Farmer { get; set; }
        public int? PorkNum { get; set; }
        public double? Pork { get; set; }
        public int? BeefNum { get; set; }
        public double? Beef { get; set; }
        public int? ChickenNum { get; set; }
        public double? Chicken { get; set; }
        public int? GoatNum { get; set; }
        public double? Goat { get; set; }
        public int? CarabaoNum { get; set; }
        public double? Carabao { get; set; }
    }
}
