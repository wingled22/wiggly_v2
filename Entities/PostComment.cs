using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class PostComment
    {
        public Guid Id { get; set; }
        public int? User { get; set; }
        public string Comment { get; set; }
        public Guid? PostId { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
