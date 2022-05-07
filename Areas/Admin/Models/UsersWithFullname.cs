using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Areas.Admin.Models
{
    public class UsersWithFullname
    {
        public Guid Id { get; set; }

        public string Firstname { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string FullName { get; set; }
    }
}
