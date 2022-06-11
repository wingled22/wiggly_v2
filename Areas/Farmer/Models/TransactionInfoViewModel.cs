using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Areas.Farmer.Models
{
    public class TransactionInfoViewModel
    {
        public int TransactionID { get; set; }
        
        public int Farmer { get; set; }
        public int Vendor { get; set; }
        public string Status { get; set; }
        public string VendorFullname { get; set; }
        public DateTime? BookDate { get; set; }
        //public DateTime? DateCreated { get; set; }


        /**
         * livestocks 
         **/
        public string LiveStockType { get; set; }
        [Required]
        public decimal? Kilos { get; set; }
        [Required]
        public decimal? Amount { get; set; }
        public int Quantity { get; set; }

        [Required]
        public string PaymentType { get; set; }
        //public string PaymentStatus { get; set; }
        public string ProofOfpayment { get; set; }

    }
}
