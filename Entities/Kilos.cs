using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class Kilos
    {
        public int Id { get; set; }
        public int? Transaction { get; set; }
        public double? Pork { get; set; }
        public double? Beef { get; set; }
        public double? Chicken { get; set; }
        public double? Goat { get; set; }
        public double? Lamb { get; set; }
    }
}
