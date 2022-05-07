using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Wiggly.Entities;
using Wiggly.Identity;
using Wiggly.ViewModels;

namespace Wiggly.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AspNetUsers, AppUser>();

            CreateMap<AspNetUsers, UsersWithFullname>().ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => string.Format("{0} {1}", src.Firstname, src.LastName)));

           

        }
    }
}
