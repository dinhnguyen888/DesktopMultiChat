using Microsoft.AspNetCore.SignalR.Client;

namespace DesktopChat.Interfaces
{
    public interface ISignalRService
    {
        string getSignalRUrl();
        void InitSignalR(string hubUrl);
        Task DisconnectAsync();
        HubConnection HubConnection { get; }

    }
}