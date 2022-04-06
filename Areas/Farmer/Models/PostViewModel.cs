using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Areas.Farmer.Models
{
    public class PostViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public String PostBody { get; set; }
        public string Image { get; set; }
        public int UserId { get; set; }
        public string UserFullname { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Liked { get; set; }
        public int LikeCounts { get; set; }
        public List<Images> ImageList { get; set; }
        public bool IsEditable { get; set; }

    }

    public class Images
    {
        public string ImagePath { get; set; }

    }
}
