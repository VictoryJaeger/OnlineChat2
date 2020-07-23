using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models.ModelViews
{
    public class CreateGroupModelView
    {
        public string Name { get; set; }
        public IEnumerable<string> ChatParticipantsId { get; set; }

    }
}
