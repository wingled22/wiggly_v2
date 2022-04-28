using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class RoomMember
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? RoomId { get; set; }
        public Guid? ItemId { get; set; }
    }
}
