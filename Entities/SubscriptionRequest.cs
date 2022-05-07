using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class SubscriptionRequest
    {
        public int Id { get; set; }
        public string ProofOfPayment { get; set; }
        public int? UserId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? StartSubs { get; set; }
        public DateTime? EndSubs { get; set; }
        public int? Months { get; set; }
        public string Status { get; set; }
    }
}
