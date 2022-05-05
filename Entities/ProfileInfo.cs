using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class ProfileInfo
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProfilePic { get; set; }
    }
}
