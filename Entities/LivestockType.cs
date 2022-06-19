using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class LivestockType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
