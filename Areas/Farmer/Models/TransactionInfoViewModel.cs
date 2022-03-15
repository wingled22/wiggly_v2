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
        public string VendorFullname { get; set; }
        public DateTime? BookDate { get; set; }
        //public DateTime? DateCreated { get; set; }

        /*
         Kilos
         */
        [Display(Name ="No of Pigs")]
        public int? PorkNum { get; set; }
        [Display(Name ="Kilos of pork")]
        public double? Pork { get; set; }
        [Display(Name ="No of Cows")]
        public int? BeefNum { get; set; }
        [Display(Name ="Kilos of beef")]
        public double? Beef { get; set; }
        [Display(Name ="No of Chicken")]
        public int? ChickenNum { get; set; }
        [Display(Name ="Kilos of chicken")]
        public double? Chicken { get; set; }
        [Display(Name ="No of Goats")]
        public int? GoatNum { get; set; }
        [Display(Name ="Kilos of goat")]
        public double? Goat { get; set; }
        [Display(Name ="No of carabao")]
        public int? CarabaoNum { get; set; }
        [Display(Name ="Kilos of carabao")]
        public double? Carabao { get; set; }

        /*
         Payment
        */
        public string PaymentType { get; set; }
        //public string PaymentStatus { get; set; }
        public decimal? Amount { get; set; }

        public string Status { get; set; }

    }
}
