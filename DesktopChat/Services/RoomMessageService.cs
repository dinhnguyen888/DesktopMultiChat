using DesktopChat.Models;
using DesktopChat.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using DesktopChat.Interfaces;

namespace DesktopChat.Services
{
    public class RoomMessageService : BaseService
    {
        private const string Endpoint = "message";
        private readonly HubConnection _hubConnection;

        
        public RoomMessageService()
        {
            ISignalRService _signalRService = new SignalRService();
            string baseUrl = _signalRService.getSignalRUrl();
            string presenceUrl = $"{baseUrl}/presencehub";
            _signalRService.InitSignalR(presenceUrl);
            _hubConnection = _signalRService.HubConnection;

            // Register envent handlers for SignalR
            RegisterHandlers();
        }

        // Method to register event handlers for SignalR
        private void RegisterHandlers()
        {
            _hubConnection.On<RoomMessageGet>("ReceiveMessage", (message) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnMessageReceived?.Invoke(message);
                });
            });

            _hubConnection.On<int>("MessageDeleted", (messageId) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnMessageDeleted?.Invoke(messageId);
                });
            });
        }

        // Event for receiving a message
        public event Action<RoomMessageGet>? OnMessageReceived;

        // Event for deleting a message
        public event Action<int>? OnMessageDeleted;

        // Get all messages in a room
        public async Task<List<RoomMessageGet>> GetAllMessagesAsync(Guid roomId)
        {
            return await GetAsync<List<RoomMessageGet>>($"{Endpoint}/{roomId}");
        }

        // Create a new message
        public async Task<RoomMessageGet> CreateMessageAsync(RoomMessagePost dto)
        {
            return await PostAsync<RoomMessageGet>(Endpoint, dto);
        }

        // Delete a message
        public async Task<bool> DeleteMessageAsync(int id)
        {
            return await DeleteAsync($"{Endpoint}/{id}");
        }

        // Join a room
        public async Task JoinRoomAsync(Guid roomId)
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("JoinRoom", roomId.ToString());
            }
        }

        // Leave a room
        public async Task LeaveRoomAsync(Guid roomId)
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("LeaveRoom", roomId.ToString());
            }
        }
    }
}
