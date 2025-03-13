using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopChat.Models
{
    public class MessagePost
    {
        public string SenderName { get; set; }
        public string MessageText { get; set; }
        public Guid RoomId { get; set; }
    }
}
