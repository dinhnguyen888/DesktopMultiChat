using DesktopChat.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopChat.Services
{
    public class FileService : BaseService
    {
        public async Task<RoomMessageGet> UploadChatRoomFileAsync(Stream fileStream, string fileName, Dictionary<string, string> parameters)
        {
            return await UploadFileAsync<RoomMessageGet>("File/send-file-to-room", fileStream, fileName, parameters);
        }

       
    }
}
