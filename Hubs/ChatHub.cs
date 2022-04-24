using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Wiggly.Entities;
using Wiggly.Identity;

namespace Wiggly.Hubs
{
    public class ChatHub :  Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public ChatHub(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<ChatHub> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            try
            {
                //_usrMngr.
                //var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.u .Identity.Name).FirstOrDefault();
                //userViewModel.Device = GetDevice();
                //userViewModel.CurrentRoom = "";

                //if (!_Connections.Any(u => u.Username == IdentityName))
                //{
                //    _Connections.Add(userViewModel);
                //    _ConnectionsMap.Add(IdentityName, Context.ConnectionId);
                //}

                //Clients.Caller.SendAsync("getProfileInfo", user.FullName, user.Avatar);
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
            }
            return base.OnConnectedAsync();
        }
    }
}
