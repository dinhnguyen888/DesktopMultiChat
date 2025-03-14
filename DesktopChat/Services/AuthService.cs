using DesktopChat.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using DesktopChat.Interfaces;
using DesktopChat.Helpers;
using System.Windows;

namespace DesktopChat.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly HubConnection _hubConnection;

        public AuthService()
        {
            ISignalRService _signalRService = new SignalRService();
            string baseUrl = _signalRService.getSignalRUrl();
            string presenceUrl = $"{baseUrl}/presencehub";
            _signalRService.InitSignalR(presenceUrl);
            _hubConnection = _signalRService.HubConnection;
        }


        // Login method
        public async Task<Token> LoginAsync(string email, string password)
        {
            var request = new Login
            {
                Email = email,
                Password = password
            };

            var token = await PostAsync<Token>("Auth/login", request, "no_token");

            if (token != null)
            {
                // Save token to global variable
                GlobalVariableHelper.AccessToken = token.AccessToken;
                GlobalVariableHelper.RefreshToken = token.RefreshToken;

                await ConnectToSignalRAsync();
            }

            return token;
        }

        // Connect to SignalR
        private async Task ConnectToSignalRAsync()
        {
            string userId = GlobalVariableHelper.GetUserInfoFromToken("id");
            string userName = GlobalVariableHelper.GetUserInfoFromToken("fullName");

            if (_hubConnection.State != HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.StartAsync();

                    await _hubConnection.InvokeAsync("OnConnectAsync", userId, userName);

                    MessageBox.Show($"Connected to SignalR\nUserId: {userId}\nUserName: {userName}",
                                    "Connection Status", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to connect to SignalR:\n{ex.Message}",
                                    "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}

