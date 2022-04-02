﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Areas.Vendor.Models
{
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public String PostBody { get; set; }
        public string Image { get; set; }
        public int UserId { get; set; }
        public string UserFullname { get; set; }
        public DateTime DateCreated{ get; set; }
        public virtual ICollection<Images> ImageList {get; set;}
    }

    public class Images { 
        public string ImagePath { get; set; }
    
    }
}
