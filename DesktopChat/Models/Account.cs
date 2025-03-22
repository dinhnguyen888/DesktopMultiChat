using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Media;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopChat.Models
{
    public class Account
    {
        public string AccountId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? DateBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; }
    }

    public class AccountViewStatus
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsOnline { get; set; }



    }


}
