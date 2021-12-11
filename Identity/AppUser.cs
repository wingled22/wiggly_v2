using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

#nullable disable

namespace Wiggly.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public string Firstname { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set; }
        public string Password{ get; set; }
        public string UserType { get; set; }
    }
}
