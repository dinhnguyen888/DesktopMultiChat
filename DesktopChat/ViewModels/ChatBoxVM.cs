using DesktopChat.Models;
using DesktopChat.Services;
using DesktopChat.Helpers;
using DesktopChat.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using Wpf.Ui.Input;
using DesktopChat.Commands;

namespace DesktopChat.ViewModels
{
    public abstract class ChatBoxVM : BaseVM
    {
        protected readonly RoomMessageService _messageService;
        protected readonly FileService _fileService;

        public ObservableCollection<RoomMessageGet> Messages { get; } = new();

        public Action ScrollToBottom { get; set; }
        private MessageEntity _messageEntity { get; set; }
        public MessageEntity MessageEntity
        {
            get => _messageEntity;
            set
            {
                _messageEntity = value;
                OnPropertyChanged(nameof(MessageEntity));
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
                CommandManager.InvalidateRequerySuggested();

            }
        }

        public ICommand SendMessageCommand { get; }
        public ICommand DeleteMessageCommand { get; }
        public ICommand UploadFileCommand { get; }

        protected ChatBoxVM(RoomMessageService messageService, FileService fileService)
        {
            _messageService = messageService;
            _fileService = fileService;

            SendMessageCommand = new RelayCommand(
                async () => await SendMessageAsync(),
                () => CanSendMessage());

            DeleteMessageCommand = new RelayCommand<RoomMessageGet>(
                async (msg) => await DeleteMessageAsync(msg));

            UploadFileCommand = new RelayCommand(
                async () => await UploadFileAsync(),
                () => CanUploadFile());

            _messageService.OnMessageReceived += MessageReceived;
            _messageService.OnMessageDeleted += MessageDeleted;
            MessageEntity = new MessageEntity();

        }

        protected abstract bool CanSendMessage();
        protected abstract bool CanUploadFile();

        protected abstract Task<RoomMessagePost> CreateMessageAsync();

        private async Task SendMessageAsync()
        {
            if (!CanSendMessage() || string.IsNullOrWhiteSpace(NewMessage))
                return;

            var newMessage = await CreateMessageAsync();
            if (newMessage != null)
            {
                Messages.Add(newMessage.ToMessageGet());
                NewMessage = string.Empty;
                ScrollToBottom.Invoke();
            }
        }

        private void MessageReceived(RoomMessageGet message)
        {
            if (ShouldReceiveMessage(message))
            {
                Messages.Add(message);
               
            }
        }

        private void MessageDeleted(int messageId)
        {
            var messageToRemove = Messages.FirstOrDefault(m => m.RoomMessageId == messageId);
            if (messageToRemove != null)
            {
                Messages.Remove(messageToRemove);
            }
        }

        private async Task DeleteMessageAsync(RoomMessageGet message)
        {
            if (message == null) return;
            await _messageService.DeleteMessageAsync(message.RoomMessageId);
            MessageDeleted(message.RoomMessageId);
        }

        private async Task UploadFileAsync()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select file to upload",
                Filter = "All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                var fileName = Path.GetFileName(filePath);

                var parameters = GetUploadParameters();

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var newMessage = await _fileService.UploadChatRoomFileAsync(fileStream, fileName, parameters);
                    Messages.Add(newMessage);
                    ScrollToBottom.Invoke();
                }
            }
        }

        protected abstract bool ShouldReceiveMessage(RoomMessageGet message);

        protected abstract Dictionary<string, string> GetUploadParameters();
    }
}
