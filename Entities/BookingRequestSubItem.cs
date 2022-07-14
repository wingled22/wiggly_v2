using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class BookingRequestSubItem
    {
        public int Id { get; set; }
        public int? BookingReqId { get; set; }
        public Guid? MarketPlaceItem { get; set; }
        public int? SubItemId { get; set; }
        public int? Quantity { get; set; }
    }
}
