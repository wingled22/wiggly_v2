using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class Message
    {
        public Guid Id { get; set; }
        public int? Room { get; set; }
        public int? UserId { get; set; }
        public string MessageText { get; set; }
        public DateTime? DatetimeCreate { get; set; }
    }
}
