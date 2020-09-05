using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.Models
{
    public class Connection
    {
        public int Id { get; set; }
        public string ConnectionID { get; set; }
        public bool Connected { get; set; }
    }
}
