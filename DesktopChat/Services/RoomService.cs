using DesktopChat.Interfaces;
using DesktopChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopChat.Services
{
    public class RoomService : BaseService, IRoomService
    {
        public async Task<IEnumerable<RoomGet>> GetAllRoomAsync()
        {

            var result = await GetAsync<IEnumerable<RoomGet>>("Room");
            return result ?? new List<RoomGet>();
        }

        public async Task<IEnumerable<RoomGet>> GetRoomByUserIdAsync(Guid userId)
        {
            var result = await GetAsync<IEnumerable<RoomGet>>($"Room/user/{userId}");
            return result ?? new List<RoomGet>();

        }
    }
}
