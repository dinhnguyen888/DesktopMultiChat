using DesktopChat.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopChat.Services
{
    public class SignalRService : ISignalRService
    {
        private string baseSignalRUrl = "https://localhost:7202/hub";

        public HubConnection HubConnection { get; private set; }
        public string getSignalRUrl()
        {
            return this.baseSignalRUrl;
        }

        public void InitSignalR(string hubUrl)
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            HubConnection.Closed += async (error) =>
            {
                await Task.Delay(1000);
                try
                {
                    await HubConnection.StartAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to reconnect to SignalR:\n{ex.Message}",
                                    "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
        }



        public async Task DisconnectAsync()
        {
            if (HubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    await HubConnection.StopAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to disconnect from SignalR:\n{ex.Message}",
                                    "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
