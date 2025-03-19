using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopChat.Models
{
    public class RoomGet
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public int MemberCount { get; set; }
        public int OnlineUserCount { get; set; }
    }

    class RoomPost
    {
    }
}
