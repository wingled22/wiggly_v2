using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class PostPhoto
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string Filename { get; set; }
        public int? User { get; set; }
        public Guid? Post { get; set; }
    }
}
