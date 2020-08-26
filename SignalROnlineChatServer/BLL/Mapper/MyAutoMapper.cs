using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Http;
using SignalROnlineChatServer.Models;
using SignalROnlineChatServer.Models.ModelViews;
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

            CreateMap<Message, MessageViewModel>()
                .ForMember(x => x.Timestamp, opt => opt.MapFrom(src => src.Timestamp.ToString("hh:mm | d MMM")));
           
            CreateMap<ChatUser, UserViewModel>()               
               .ForMember(x => x.Id, opt => opt.MapFrom(src => src.UserId))
               .ForMember(x => x.UserName, opt => opt.MapFrom(src => src.User.UserName));
            CreateMap<ICollection<ChatUser>, List<UserViewModel>>();

            CreateMap<Chat, ChatViewModel>()
                .ForMember(x => x.LastMessage, opt => opt.MapFrom(src => src.Messages.Last().Text))
                .ForMember(x => x.LastMessageAuthor, opt => opt.MapFrom(src => src.Messages.Last().Name))
                .ForMember(x => x.LastMessageDate, opt => opt.MapFrom(src => src.Messages.Last().Timestamp.ToString("d MMM")))
                .ForMember(x => x.Messages, opt => opt.MapFrom(src => src.Messages))
                .ForMember(x => x.ChatParticipants, opt => opt.MapFrom(src => src.ChatParticipants.Select(x => x.User).ToList()));

            CreateMap<IQueryable<User>, List<UserViewModel>>();

            CreateMap<User, UserViewModel>();
                        
        }
    }

    public class UserIdResolver : IValueResolver<Message, MessageViewModel, string>
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserIdResolver(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string Resolve(Message source, MessageViewModel destination, string destMember, ResolutionContext context)
        {
            return _contextAccessor.HttpContext.User.Claims
                .Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
        }

        //public string Resolve(Message source, MessageViewModel destination, UserClaim destinationMember, ResolutionContext context)
        //{
        //    return _contextAccessor.HttpContext.User.Claims
        //        .Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
        //        //.Select(c => c.).SingleOrDefault();
        //}

        //public User IValueResolver<Message, MessageViewModel, User>.Resolve(Message source, MessageViewModel destination, User destMember, ResolutionContext context)
        //{
        //    throw new NotImplementedException();
        //}
    }


}
