using AutoMapper;
using ProjectRoomChat.Helpers;
using ProjectRoomChat.Models;
using ProjectRoomChat.ViewModels;

namespace ProjectRoomChat.Mappings
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageViewModel>()
                .ForMember(x => x.FromUserName, opt => opt.MapFrom(x => x.FromUser.UserName))
                .ForMember(x => x.FromFullName, opt => opt.MapFrom(x => x.FromUser.FullName))
                .ForMember(x => x.Room, opt => opt.MapFrom(x => x.ToRoom.Name))
                .ForMember(x => x.Avatar, opt => opt.MapFrom(x => x.FromUser.Avatar))
                .ForMember(dst => dst.Content, opt => opt.MapFrom(x => BasicEmojis.ParseEmojis(x.Content)))
                .ReverseMap();
        }
    }
}
