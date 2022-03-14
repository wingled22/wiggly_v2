using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Areas.Vendor.Models
{
    public class TransactionInfoViewModel
    {
        public int TransactionID { get; set; }
        public int Farmer { get; set; }
        //public string Status { get; set; }
        public DateTime? BookDate { get; set; }
        //public DateTime? DateCreated { get; set; }

        /*
         Kilos
         */
        public int? PorkNum { get; set; }
        public double? Pork { get; set; }
        public int? BeefNum { get; set; }
        public double? Beef { get; set; }
        public int? ChickenNum { get; set; }
        public double? Chicken { get; set; }
        public int? GoatNum { get; set; }
        public double? Goat { get; set; }
        public int? CarabaoNum { get; set; }
        public double? Carabao { get; set; }

        /*
         Payment
        */
        public string PaymentType { get; set; }
        //public string PaymentStatus { get; set; }
        public decimal? Amount { get; set; }
    }
}
