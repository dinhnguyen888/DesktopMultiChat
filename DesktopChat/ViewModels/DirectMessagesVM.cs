using DesktopChat.Models;
using DesktopChat.Services;
using DesktopChat.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;

namespace DesktopChat.ViewModels
{
    public class DirectMessagesVM : ChatBoxVM
    {
        private readonly AccountService _accountService;

        public ObservableCollection<AccountViewStatus> DirectMessages { get; } = new();

        private AccountViewStatus _selectedDirectMessage;
        public AccountViewStatus SelectedDirectMessage
        {
            get => _selectedDirectMessage;
            set
            {
                if (_selectedDirectMessage != value)
                {
                    _selectedDirectMessage = value;
                    OnPropertyChanged(nameof(SelectedDirectMessage));

                   
                    
                        // ✅ Đặt thông tin người dùng vào MessageEntity
                        MessageEntity.MessageEntityName = value.FullName;
                        MessageEntity.MessageEntityId = value.AccountId;
                        OnPropertyChanged(nameof(MessageEntity));
                        // ✅ Load tin nhắn trực tiếp
                        LoadDirectChat(value.AccountId);
                    
                }
            }
        }

        public DirectMessagesVM(AccountService accountService, RoomMessageService messageService, FileService fileService)
            : base(messageService, fileService)
        {
            _accountService = accountService;
            LoadDirectMessagesAsync();
        }

        // 🔥 Load danh sách tin nhắn trực tiếp
        private async void LoadDirectMessagesAsync()
        {
            try
            {
                var onlineAccounts = await _accountService.ViewOnlineAccountAsync();
                if (onlineAccounts != null)
                {
                    DirectMessages.Clear();
                    foreach (var account in onlineAccounts)
                    {
                        DirectMessages.Add(account);
                    }

                    // ✅ Chọn người đầu tiên nếu chưa chọn
                    if (SelectedDirectMessage == null && DirectMessages.Any())
                    {
                        SelectedDirectMessage = DirectMessages.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading direct messages: {ex.Message}");
            }
        }

        // 🔥 Load tin nhắn trực tiếp khi chọn người dùng
        private async void LoadDirectChat(Guid userId)
        {
            if (userId == Guid.Empty) return;

            Messages.Clear();

            var messages = await _messageService.GetAllMessagesAsync(userId);
            foreach (var message in messages)
            {
                Messages.Add(message);
            }

            // ✅ Tham gia vào room tin nhắn trực tiếp
            await _messageService.JoinRoomAsync(userId);
            ScrollToBottom?.Invoke();
        }

        // ✅ Kiểm tra điều kiện gửi tin nhắn
        protected override bool CanSendMessage() =>
            !string.IsNullOrWhiteSpace(NewMessage) && SelectedDirectMessage != null;

        // ✅ Kiểm tra điều kiện tải file
        protected override bool CanUploadFile() =>
            SelectedDirectMessage != null;

        // ✅ Tạo tin nhắn mới cho tin nhắn trực tiếp
        protected override async Task<RoomMessagePost> CreateMessageAsync()
        {
            //var newMessage = new RoomMessagePost
            //{
            //    RoomId = SelectedDirectMessage.AccountId,
            //    MessageText = NewMessage,
            //    SenderName = GlobalVariableHelper.GetUserInfoFromToken("fullName")
            //};

            //await _messageService.CreateMessageAsync(newMessage);
            //return newMessage;
            var newMessage = new RoomMessagePost
            {
                RoomId = SelectedDirectMessage.AccountId,
                MessageText = NewMessage,
                SenderName = GlobalVariableHelper.GetUserInfoFromToken("fullName")
            };
            MessageBox.Show($"RoomId: {SelectedDirectMessage.AccountId}\nMessageText: {NewMessage}\nSenderName: {GlobalVariableHelper.GetUserInfoFromToken("fullName")}");
            return newMessage;

        }

        // ✅ Kiểm tra tin nhắn nhận được có phải từ người đang chọn không
        protected override bool ShouldReceiveMessage(RoomMessageGet message)
        {
            return message.RoomId == SelectedDirectMessage?.AccountId;
        }

        // ✅ Truyền tham số khi tải file
        protected override Dictionary<string, string> GetUploadParameters()
        {
            return new Dictionary<string, string>
            {
                { "roomId", SelectedDirectMessage.AccountId.ToString() },
                { "senderName", GlobalVariableHelper.GetUserInfoFromToken("fullName") }
            };
        }
    }
}
