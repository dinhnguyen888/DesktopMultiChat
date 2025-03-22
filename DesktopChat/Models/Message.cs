using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopChat.Models
{
    public class MessageEntity // this class is used for ChatboxVM that can store both RoomMessage and DMs attribute
    {
        public Guid MessageEntityId { get; set; }
        public string MessageEntityName { get; set; }
    }
    public class RoomMessageGet
    {
        public int RoomMessageId { get; set; }
        public string SenderName { get; set; }
        public string MessageText { get; set; }
        public Guid RoomId { get; set; }
    }

    public class RoomMessagePost
    {
        public string SenderName { get; set; }
        public string MessageText { get; set; }
        public Guid RoomId { get; set; }
    }

    public class DirectMessageGet
    {
        public int DirectMessageId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string MessageText { get; set; }
    }

    public class DirectMessagePost
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string MessageText { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
    }
}
