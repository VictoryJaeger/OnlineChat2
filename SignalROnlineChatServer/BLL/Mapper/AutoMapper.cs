using AutoMapper;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.BLL.Mapper
{
    public class AutoMapper: Profile
    {
        public AutoMapper()
        {
            CreateMap<Chat, ChatViewModel>().ForMember(x => x.LastMessage, opt => opt.MapFrom(src => src.Messages.Last().Text))
                .ForMember(x => x.LastMessageDate, opt => opt.MapFrom(src => src.Messages.Last().Timestamp));
        }
    }
}
