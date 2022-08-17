using System;
using System.Collections.Generic;

namespace Wiggly.Entities
{
    public partial class Notif
    {
        public Guid Id { get; set; }
        public int? Recipient { get; set; }
        public string Message { get; set; }
        public DateTime? DateCreated { get; set; }
        public string DateCreatedString { get; set; }
        public string NotifType { get; set; }
        public string NotifIsRead { get; set; }
        public int? BookingRequest { get; set; }
        public string PushNotifIsRead { get; set; }
        public DateTime? PushNotifReminder { get; set; }
        public string PushNotifReminderIsRead { get; set; }
    }
}
