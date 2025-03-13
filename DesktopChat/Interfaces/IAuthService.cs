using DesktopChat.Models;

namespace DesktopChat.Interfaces
{
    public interface IAuthService
    {
        Task<Token> LoginAsync(string email, string password);
    }
}