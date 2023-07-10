using AutoMapper;
using ProjectRoomChat.Models;
using ProjectRoomChat.ViewModels;

namespace ProjectRoomChat.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(x => x.UserName)).ReverseMap();
        }
    }
}
