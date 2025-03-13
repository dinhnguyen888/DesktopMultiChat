using DesktopChat.Models;
using DesktopChat.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DesktopChat.Commands;
using DesktopChat.Helpers;
using DesktopChat.Interfaces;
using Wpf.Ui.Input;

namespace DesktopChat.ViewModels
{
    public class ChatVM : BaseVM
    {
        private readonly MessageService _messageService;
        private readonly IRoomService _roomService;

        public ObservableCollection<RoomGet> Rooms { get; } = new();
        public ObservableCollection<MessageGet> Messages { get; } = new();

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

                    LoadRoomChat(value?.RoomId);
                }
            }
        }

        private string _newMessage;
        public string NewMessage
        {
            get => _newMessage;
            set
            {
                _newMessage = value;
                OnPropertyChanged(nameof(NewMessage));
            }
        }

        public ICommand SendMessageCommand { get; }
        public ICommand DeleteMessageCommand { get; }

        public ScrollViewer MessagesScrollViewer { get; set; }

        public ChatVM(IRoomService roomService, MessageService messageService)
        {
            _roomService = roomService;
            _messageService = messageService;

            LoadRoomsAsync();

            _messageService.OnMessageReceived += MessageReceived;
            _messageService.OnMessageDeleted += MessageDeleted;

            SendMessageCommand = new RelayCommand(async () => await SendMessageAsync(),
                () => !string.IsNullOrWhiteSpace(NewMessage) && SelectedRoom != null);

            DeleteMessageCommand = new RelayCommand<MessageGet>(async (msg) => await DeleteMessageAsync(msg));
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

            ScrollToBottom();
        }


        private async Task SendMessageAsync()
        {
            if (SelectedRoom == null || string.IsNullOrWhiteSpace(NewMessage)) return;

            var newMessage = await _messageService.CreateMessageAsync(new MessagePost
            {
                RoomId = SelectedRoom.RoomId,
                MessageText = NewMessage,
                SenderName = GlobalVariableHelper.GetUserInfoFromToken("fullName")
            });

            Messages.Add(newMessage);
            NewMessage = string.Empty;
            ScrollToBottom();

        }

        private void MessageReceived(MessageGet message)
        {
            if (message.RoomId == SelectedRoom?.RoomId)
            {
                Messages.Add(message);
             
            }
        }

        private void MessageDeleted(int messageId)
        {
            var messageToRemove = Messages.FirstOrDefault(m => m.MessageId == messageId);
            if (messageToRemove != null) Messages.Remove(messageToRemove);
        }

        private async Task DeleteMessageAsync(MessageGet message)
        {
            if (message == null) return;
            await _messageService.DeleteMessageAsync(message.MessageId);
            MessageDeleted(message.MessageId);
        }

        private void ScrollToBottom()
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessagesScrollViewer?.ScrollToEnd();
            }, System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

    }
}