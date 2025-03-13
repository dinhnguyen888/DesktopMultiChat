using DesktopChat.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopChat.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomGet>> GetAllRoomAsync();
        Task<IEnumerable<RoomGet>> GetRoomByUserIdAsync(Guid userId);
    }
}
