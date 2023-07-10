using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using ProjectRoomChat.Data;
using ProjectRoomChat.Models;
using ProjectRoomChat.ViewModels;
using System.Text.RegularExpressions;

namespace ProjectRoomChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly static List<UserViewModel> _connections = new List<UserViewModel>();
        private readonly static Dictionary<string, string> _connectionsMap = new Dictionary<string, string>();

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ChatHub(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private string IdentityName
        {
            get { return Context.User.Identity.Name; }
        }

        public async Task Leave(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task SendPrivate(string receiveName, string message)
        {
            if (_connectionsMap.TryGetValue(receiveName, out string userId))
            {
                var sender = _connections.Where(u => u.UserName == IdentityName).First();

                if (!string.IsNullOrEmpty(message.Trim()))
                {
                    
                    var messageViewModel = new MessageViewModel()
                    {
                        Content = Regex.Replace(message, @"<.*?>", string.Empty),
                        FromUserName = sender.UserName,
                        FromFullName = sender.FullName,
                        Avatar = sender.Avatar,
                        Room = "",
                        Timestamp = DateTime.Now
                    };

                    
                    await Clients.Client(userId).SendAsync("newMessage", messageViewModel);
                    await Clients.Caller.SendAsync("newMessage", messageViewModel);
                }
            }
        }

        public async Task Join(string roomName)
        {
            try
            {
                var user = _connections.Where(u => u.UserName == IdentityName).FirstOrDefault();
                if (user != null && user.CurrentRoom != roomName)
                {
                    if (!string.IsNullOrEmpty(user.CurrentRoom))
                        await Clients.OthersInGroup(user.CurrentRoom).SendAsync("removeUser", user);
                    await Leave(user.CurrentRoom);
                    await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                    user.CurrentRoom = roomName;
                    await Clients.OthersInGroup(roomName).SendAsync("addUser", user);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "You failed to join the chat room!" + ex.Message);
            }
        }

        public IEnumerable<UserViewModel> GetUsers(string roomName)
        {
            return _connections.Where(x => x.CurrentRoom == roomName).ToList();
        }

        private string GetDevice()
        {
            var device = Context.GetHttpContext().Request.Headers["Device"].ToString();
            if (!string.IsNullOrEmpty(device) && (device.Equals("Desktop") || device.Equals("Mobile"))){
                return device;
            }
            return "Web";
        }

        public override Task OnConnectedAsync()
        {
            {
                try
                {
                    var user = _context.Users.Where(u => u.UserName == IdentityName).FirstOrDefault();
                    var userViewModel = _mapper.Map<ApplicationUser, UserViewModel>(user);
                    userViewModel.Device = GetDevice();
                    userViewModel.CurrentRoom = "";

                    if (!_connections.Any(u => u.UserName == IdentityName))
                    {
                        _connections.Add(userViewModel);
                        _connectionsMap.Add(IdentityName, Context.ConnectionId);
                    }

                    Clients.Caller.SendAsync("getProfileInfo", userViewModel);
                }
                catch (Exception ex)
                {
                    Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
                }
                return base.OnConnectedAsync();
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var user = _connections.Where(u => u.UserName == IdentityName).First();
                _connections.Remove(user);

                Clients.OthersInGroup(user.CurrentRoom).SendAsync("removeUser", user);

                _connectionsMap.Remove(user.UserName);
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
