using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Wiggly.Identity;

#nullable disable

namespace Wiggly.Entities
{
    public class WigglyIdentityContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public WigglyIdentityContext(DbContextOptions<WigglyIdentityContext> options)
            : base(options)
        {

        }
    }
}
