using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class ProfilePic
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
    }
}
