using DesktopChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopChat.Services
{
    class ProjectService : BaseService
    {
        public async Task<IEnumerable<ProjectGet>> GetAllProjectAsync()
        {
            var projects = await GetAsync<IEnumerable<ProjectGet>>("Project", "no_token");
            return projects ?? new List<ProjectGet>();
        }

        
    }
}
