using System;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace DesktopChat
{
    public static class ApplicationState
    {
        public static string CurrentUserName { get; set; }
        public static int? CurrentUserId { get; set; }
        public static string? CurrentUserPhone { get; set; }
        public static string GlobalClickName { get; set; }
        public static string GlobalClickPhoneNumber { get; set; }
        public static string GlobalClickConversation { get; set; }

     
    }

}
