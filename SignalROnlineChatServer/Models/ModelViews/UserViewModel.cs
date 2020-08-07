using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models.ModelViews
{
    public class UserViewModel
    {
        public UserViewModel()
        {

        }
        //public UserViewModel(string Name, string Id)
        //{
        //    this.Id = Id;
        //    UserName = Name;
        //}

        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
