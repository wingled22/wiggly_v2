using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Areas.Vendor.Models
{
    public class CommentViewModel
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string UserFullname { get; set; }
        public string CommentBody { get; set; }
        public string DateCreated { get; set; }
    }
}
