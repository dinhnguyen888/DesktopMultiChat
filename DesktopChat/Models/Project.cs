using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopChat.Models
{
    public class ProjectGet
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid OwnerId { get; set; }

    }

    public class MemberProgressInProject
    {
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public double ProgressPercentage { get; set; }
    }
}
