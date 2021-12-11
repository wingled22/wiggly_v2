using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Wiggly.Entities;
using Wiggly.Identity;

namespace Wiggly.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AspNetUsers, AppUser>();
        }
    }
}
