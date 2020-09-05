using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models.ModelViews
{
    public class CreateGroupModelView
    {
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select at least one user")]
        public IEnumerable<string> ChatParticipantsId { get; set; }

    }
}
