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
        public string Address { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string Description { get; set; }
        public string BuyOrSell { get; set; }
        public string Category { get; set; }
        public int UserId{ get; set; }
        public string UserFullname{ get; set; }
        public string DateCreated{ get; set; }
        public string Image { get; set; }
        public bool IsEditable { get; set; }
        public List<MarketPlaceImage> ImageList { get; set; }


    }
    public class MarketPlaceImage
    {
        public Guid ImageId { get; set; }
        public string ImagePath { get; set; }
    }
}
