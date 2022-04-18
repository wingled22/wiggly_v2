using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Models
{
    public class MarketPlaceViewModel
    {
        public Guid ItemID { get; set; }
        public string Caption { get; set; }
        public int BuyOrSell { get; set; }
        public string Category { get; set; }
        public int UserId{ get; set; }
        public int UserFullname{ get; set; }
        public string Image { get; set; }
        public bool IsEditable { get; set; }


    }
    public class MarketPlaceImage
    {
        public Guid ImageId { get; set; }
        public string ImagePath { get; set; }
    }
}
