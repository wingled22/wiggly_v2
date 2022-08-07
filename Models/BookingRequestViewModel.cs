using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Models
{
    public class BookingRequestViewModel
    {
        public int Id { get; set; }
        public Guid? Item { get; set; }
        public string Message { get; set; }
        public string Address { get; set; }
        public string Long { get; set; }
        public string Lat { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public int BookingQuantity { get; set; }
        public int Kilos { get; set; }
        public decimal Amount{ get; set; }
        public decimal TotalAmount{ get; set; }
        public string Status { get; set; }

        public DateTime? DateCreated { get; set; }
    }


    public class BookingRequestViewModelRevised
    {
        public int Id { get; set; }
        public Guid? Item { get; set; }
        public string Message { get; set; }
        public string Address { get; set; }
        public string Long { get; set; }
        public string Lat { get; set; }
      
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public DateTime? DateCreated { get; set; }

        public List<BookingSubItems> SubItems{ get; set; }
    }

    public class BookingSubItems
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public int? QuantityBooked { get; set; }
        public decimal? Price { get; set; }
        public decimal? Kilos { get; set; }
        public decimal? Amount { get; set; }
    }


    public class FarmerPendingBookingSubItems
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Vendor { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
        public int? QuantityBooked { get; set; }
        public decimal? Price { get; set; }
        public decimal? Kilos { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? DeliveryDate{ get; set; }
    }
}
