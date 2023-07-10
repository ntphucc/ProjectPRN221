using AutoMapper;
using ProjectRoomChat.Models;
using ProjectRoomChat.ViewModels;

namespace ProjectRoomChat.Mappings
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomViewModel>()
                .ForMember(x => x.Admin, opt => opt.MapFrom(x => x.Admin.UserName))
                .ReverseMap();
        }
    }
}
