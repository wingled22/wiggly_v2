using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class Post
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public int? User { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
