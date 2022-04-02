using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class UserLikedPost
    {
        public Guid Id { get; set; }
        public Guid? Post { get; set; }
        public int? User { get; set; }
    }
}
