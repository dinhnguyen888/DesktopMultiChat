using DesktopChat.Models;
using DesktopChat.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DesktopChat.Commands;
using DesktopChat.Helpers;
using DesktopChat.Interfaces;
using Wpf.Ui.Input;
using Microsoft.Win32;
using System.IO;
using System.IO.Pipes;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace DesktopChat.ViewModels
{
    public class RoomMessagesVM : ChatBoxVM
    {
        private readonly IRoomService _roomService;

        public ObservableCollection<RoomGet> Rooms { get; } = new();

        private RoomGet _selectedRoom;
        public RoomGet SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                if (_selectedRoom != value)
                {
                    _selectedRoom = value;
                    OnPropertyChanged(nameof(SelectedRoom));
                    MessageEntity.MessageEntityName = value.RoomName ??"Can not get message";
                    MessageEntity.MessageEntityId = value.RoomId;
                    OnPropertyChanged(nameof(MessageEntity));
                    LoadRoomChat(value?.RoomId);
                   
                }
            }
        }

        public RoomMessagesVM(
            IRoomService roomService,
            RoomMessageService messageService,
            FileService fileService)
            : base(messageService, fileService)
        {
            _roomService = roomService;


            LoadRoomsAsync();
          
        }

        private async void LoadRoomsAsync()
        {
            var userId = Guid.Parse(GlobalVariableHelper.GetUserInfoFromToken("id"));
            var rooms = await _roomService.GetRoomByUserIdAsync(userId);
            Rooms.Clear();
            foreach (var room in rooms) Rooms.Add(room);

            if (SelectedRoom == null && Rooms.Any())
            {
                SelectedRoom = Rooms.First();
            }
        }

        private async void LoadRoomChat(Guid? roomId)
        {
            if (roomId == null) return;

            Messages.Clear();
            var messages = await _messageService.GetAllMessagesAsync(roomId.Value);
            foreach (var message in messages) Messages.Add(message);

            await _messageService.JoinRoomAsync(roomId.Value);
            ScrollToBottom?.Invoke();
        }

        protected override bool CanSendMessage() =>
    !string.IsNullOrWhiteSpace(NewMessage) && SelectedRoom != null;
        protected override bool CanUploadFile() => SelectedRoom != null;

        protected override async Task<RoomMessagePost> CreateMessageAsync()
        {
            var newMessage = new RoomMessagePost
            {
                RoomId = SelectedRoom.RoomId,
                MessageText = NewMessage,
                SenderName = GlobalVariableHelper.GetUserInfoFromToken("fullName")
            };

            await _messageService.CreateMessageAsync(newMessage);
            return newMessage;
        }

        protected override bool ShouldReceiveMessage(RoomMessageGet message)
            => message.RoomId == SelectedRoom?.RoomId;

        protected override Dictionary<string, string> GetUploadParameters()
        {
            return new Dictionary<string, string>
        {
            { "roomId", SelectedRoom.RoomId.ToString() },
            { "senderName", GlobalVariableHelper.GetUserInfoFromToken("fullName") }
        };
        }
    }

}
