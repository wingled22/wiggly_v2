using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Areas.Vendor.Models
{
    public class TransactionInfoViewModel
    {
        public int TransactionID { get; set; }
        
        [Required]
        public int Farmer { get; set; }
        public string FarmerFullname { get; set; }
        public string Status { get; set; }
        [Required]
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
        public decimal? Total { get; set; }
        public int Quantity { get; set; }

        [Required]
        public string PaymentType { get; set; }
        //public string PaymentStatus { get; set; }
        public string ProofOfpayment { get; set; }


    }
}
