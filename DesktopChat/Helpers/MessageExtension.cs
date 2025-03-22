using DesktopChat.Models;

namespace DesktopChat.Helpers
{
    public static class MessageExtensions
    {
        public static RoomMessageGet ToMessageGet(this RoomMessagePost post)
        {
            return new RoomMessageGet
            {
                RoomId = post.RoomId,
                MessageText = post.MessageText,
                SenderName = post.SenderName,

            };
        }
    }
}
