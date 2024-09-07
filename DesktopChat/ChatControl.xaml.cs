using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR.Client;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using System.Net;
using ToastNotifications.Messages;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
/* * */

namespace DesktopChat
{
    public partial class ChatControl : UserControl
    {
        private string GlobalClickPhoneNumber;
        private string GlobalClickName;
        public string GlobalClickConversation;
        private HubConnection _hubConnection;
        private HubConnection _messageHubConnection;

        public ChatControl()
        {
            InitializeComponent();
            ContactBtn.Click += ContactBtn_Click;
            SendMsg.Click += SendMsg_Click;
            RoomBtn.Click += RoomBtn_Click;
            CurrentUserName.Content = ApplicationState.CurrentUserName;
            InitializeSignalRConnection();
            InitializeMessageHubConnection();
            GlobalClickName = ApplicationState.GlobalClickName;
            GlobalClickPhoneNumber = ApplicationState.GlobalClickPhoneNumber;
            GlobalClickConversation = ApplicationState.GlobalClickConversation;

        }
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        private async void InitializeSignalRConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7066/statushub")
                .Build();

            _hubConnection.On<string>("UserConnected", (phoneNumber) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var contact = ((List<Contact>)DisplayContact.ItemsSource)?.FirstOrDefault(c => c.PhoneNumber == phoneNumber);
                    if (contact != null)
                    {
                        contact.IsOnline = true;
                        DisplayContact.Items.Refresh();
                    }
                });
            });

            _hubConnection.On<string>("UserDisconnected", (phoneNumber) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var contact = ((List<Contact>)DisplayContact.ItemsSource)?.FirstOrDefault(c => c.PhoneNumber == phoneNumber);
                    if (contact != null)
                    {
                        contact.IsOnline = false;
                        DisplayContact.Items.Refresh();
                    }
                });
            });
            _hubConnection.On<string>("ReceiveNotification", (notification) =>
            {
                Dispatcher.Invoke(() =>
                {
                    notifier.ShowInformation(notification);
                });
            });

            await _hubConnection.StartAsync();
            string currentUserPhone = ApplicationState.CurrentUserPhone;
            if (!string.IsNullOrEmpty(currentUserPhone))
            {
                await _hubConnection.InvokeAsync("RegisterContact", currentUserPhone);
            }
        }

        private async void InitializeMessageHubConnection()
        {
            _messageHubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7066/messagehub")
                .Build();

            _messageHubConnection.On<string, string, DateTime>("ReceiveMessage", (fromNumber, messageText, sentDateTime) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var messageBlock = new TextBlock
                    {
                        Text = $"{sentDateTime.ToShortTimeString()}: {messageText}",
                        FontSize = 14,
                        Margin = new Thickness(2)
                    };
                    ChatDisplayArea.Children.Add(messageBlock);
                    ChatScrollViewer.ScrollToBottom();
                });
            });

            await _messageHubConnection.StartAsync();
        }

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                SendMsg_Click(this, new RoutedEventArgs());
            }
        }

        private async void RoomBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string currentUserPhone = ApplicationState.CurrentUserPhone;
                if (string.IsNullOrEmpty(currentUserPhone))
                {
                    MessageBox.Show("Số điện thoại không được tìm thấy.");
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync($"https://localhost:7066/api/Contacts/get-all-room/{currentUserPhone}");
                    var rooms = JsonConvert.DeserializeObject<List<Contact>>(response);
                    DisplayContact.ItemsSource = null;
                    DisplayContact.ItemsSource = rooms;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy danh sách phòng: {ex.Message}");
            }
        }

        private async void SendMsg_Click(object sender, RoutedEventArgs e)
        {
            var messageText = MessageInput.Text;
            if (string.IsNullOrWhiteSpace(messageText) || string.IsNullOrEmpty(GlobalClickPhoneNumber))
            {
                MessageBox.Show("Please select a contact and enter a message.");
                return;
            }

            string fromNumber = ApplicationState.CurrentUserPhone;
            int conversationId = GetConversationId(GlobalClickPhoneNumber);

            try
            {
                string formattedMessage = $"{ApplicationState.CurrentUserName} >>> {messageText}";
                Dispatcher.Invoke(() => ChatScrollViewer.ScrollToBottom());
                await _messageHubConnection.InvokeAsync("SendMessage", fromNumber, formattedMessage, conversationId);
                await _hubConnection.InvokeAsync("SendNotificationToOnlineUsersInConversation", fromNumber, formattedMessage, conversationId);
               
                MessageInput.Clear();
              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi tin nhắn: " + ex.Message);
            }
        }

        private int GetConversationId(string phoneNumber)
        {
            return int.Parse(GlobalClickConversation);
        }

        private async void ContactBtn_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string currentUserPhone = ApplicationState.CurrentUserPhone;
                var response = await client.GetStringAsync($"https://localhost:7066/api/Contacts/get-all-contacts/{currentUserPhone}");
                var contacts = JsonConvert.DeserializeObject<List<Contact>>(response);
                var onlineContacts = await _hubConnection.InvokeAsync<string[]>("GetOnlineContacts");

                foreach (var contact in contacts)
                {
                    contact.IsOnline = onlineContacts.Contains(contact.PhoneNumber);
                }

                DisplayContact.ItemsSource = contacts;
                DisplayContact.Items.Refresh();
            }
        }

        private async void SendFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                var filePath = openFileDialog.FileName;
                var fileName = openFileDialog.SafeFileName;
                var fromNumber = ApplicationState.CurrentUserPhone;
                var conversationId = GlobalClickConversation;
                var senderName = ApplicationState.CurrentUserName;
                var url = "https://localhost:7066/api/File/SendFile";

                if (string.IsNullOrWhiteSpace(fromNumber) || string.IsNullOrWhiteSpace(conversationId))
                {
                    MessageBox.Show("Required fields are missing or invalid. Please check your input.");
                    return;
                }

                GlobalProgressBar.Visibility = Visibility.Visible;
                GlobalProgressBar.Value = 0;

                string formattedMessage = $"{ApplicationState.CurrentUserName} >>> {fileName}";

                try
                {
                    using (var httpClient = new HttpClient())
                    using (var multipartContent = new MultipartFormDataContent())
                    {
                        var fileContent = new StreamContent(File.OpenRead(filePath));
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");

                        
                        multipartContent.Add(fileContent, "file", fileName);
                        multipartContent.Add(new StringContent(conversationId), "conversationId");
                        multipartContent.Add(new StringContent(fromNumber), "fromNumber");
                        multipartContent.Add(new StringContent(senderName), "senderName");

                        var response = await httpClient.PostAsync(url, multipartContent);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"File sent successfully. Response from server: {responseContent}");
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Error: {response.StatusCode}\n{errorContent}");
                        }
                    }

                    await _hubConnection.InvokeAsync("SendNotificationToOnlineUsersInConversation", fromNumber, formattedMessage, Convert.ToInt32(conversationId));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
                finally
                {
                    GlobalProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ViewFile_Click(object sender, RoutedEventArgs e)
        {
            int conversationId = Convert.ToInt32(GlobalClickConversation);
            if (conversationId != 0)
            {
                var listFileWindow = new ListFile(GlobalClickConversation);
                listFileWindow.Show();
            }
            else { MessageBox.Show("vui lòng chọn một cuộc trò chuyện"); }
        }


        private async void DisplayContact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedContact = DisplayContact.SelectedItem as Contact;
            if (selectedContact != null && _hubConnection.State == HubConnectionState.Connected)
            {
                ChatDisplayArea.Children.Clear();
                GlobalClickName = selectedContact.ContactName;
                GlobalClickPhoneNumber = selectedContact.PhoneNumber;
                GlobalClickConversation = selectedContact.ConversationId.ToString();
                CurrentContactName.Content = selectedContact.ContactName;
                int conversationId = GetConversationId(selectedContact.PhoneNumber);
                await _messageHubConnection.InvokeAsync("JoinConversation", conversationId);
                //MessageBox.Show($"Bạn đã chọn liên hệ: {GlobalClickName}\nSố điện thoại: {GlobalClickPhoneNumber} id:{GlobalClickConversation}");
            }
        }
    }
    public class Contact
    {
        public int ConversationId { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsOnline { get; set; }
    }
}
