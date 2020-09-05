using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Http;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
using Syncfusion.EJ2.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.BLL.Mapper
{
    public class MyAutoMapper: Profile
    {

        public MyAutoMapper()
        {
            CreateMap<ICollection<ChatUser>, List<UserViewModel>>();

            CreateMap<IQueryable<Chat>, List<ChatViewModel>>();

            CreateMap<Message, MessageViewModel>()
                .ForMember(x => x.Timestamp, opt => opt.MapFrom(src => src.Timestamp.ToString("hh:mm | d MMM")))
                .AfterMap<SetTypeToMessage>();
           
            CreateMap<ChatUser, UserViewModel>()               
               .ForMember(x => x.Id, opt => opt.MapFrom(src => src.UserId))
               .ForMember(x => x.UserName, opt => opt.MapFrom(src => src.User.UserName));
            CreateMap<ICollection<ChatUser>, List<UserViewModel>>();

            CreateMap<Chat, ChatViewModel>()
                .ForMember(x => x.LastMessage, opt => opt.MapFrom(src => src.Messages.Last().Text))
                .ForMember(x => x.LastMessageAuthor, opt => opt.MapFrom(src => src.Messages.Last().Name))
                .ForMember(x => x.LastMessageDate, opt => opt.MapFrom(src => src.Messages.Last().Timestamp.ToString("d MMM")))
                .ForMember(x => x.Messages, opt => opt.MapFrom(src => src.Messages))
                .ForMember(x => x.ChatParticipants, opt => opt.MapFrom(src => src.ChatParticipants.Select(x => x.User).ToList()))
                .AfterMap<SetUnreadMessageCountToCurrentUser>();

            CreateMap<IQueryable<User>, List<UserViewModel>>();

            CreateMap<User, UserViewModel>();
                        
        }
    }

    public class SetUnreadMessageCountToCurrentUser : IMappingAction<Chat, ChatViewModel>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SetUnreadMessageCountToCurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Process(Chat source, ChatViewModel destination, ResolutionContext context)
        {
            destination.UnreadMessages = source.ChatParticipants.Where(x => 
                x.UserId == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).FirstOrDefault().UnreadMessageCount;
        }
    }

    public class SetTypeToMessage : IMappingAction<Message, MessageViewModel>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SetTypeToMessage(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Process(Message source, MessageViewModel destination, ResolutionContext context)
        {
            var activeAccount = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            if (destination.Name == activeAccount) destination.Type = MessageType.Outgoing;
            else if (destination.Name == "Default") destination.Type = MessageType.Default;
            else destination.Type = MessageType.Incoming;
        }
    }

}
