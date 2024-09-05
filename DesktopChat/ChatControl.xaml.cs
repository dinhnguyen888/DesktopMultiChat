using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR.Client;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace DesktopChat
{
    public partial class ChatControl : UserControl
    {
        public static string GlobalClickName { get; set; }
        public static string GlobalClickPhoneNumber { get; set; }
        public static string GlobalClickConversation { get; set; }
        private HubConnection _hubConnection;
        private HubConnection _messageHubConnection;

        public ChatControl()
        {
            InitializeComponent();
            ContactBtn.Click += ContactBtn_Click;
            SendMsg.Click += SendMsg_Click;
            RoomBtn.Click += RoomBtn_Click;  // Thêm sự kiện RoomBtn_Click
            CurrentUserName.Content = ApplicationState.CurrentUserName;
            InitializeSignalRConnection();
            InitializeMessageHubConnection();
        }

        private async void InitializeSignalRConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7066/statushub")
                .Build();

            // Lắng nghe sự kiện người dùng đăng nhập
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

            // Lắng nghe sự kiện người dùng đăng xuất
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
                // Lấy số điện thoại hiện tại từ ApplicationState
                string currentUserPhone = ApplicationState.CurrentUserPhone;

                if (string.IsNullOrEmpty(currentUserPhone))
                {
                    MessageBox.Show("Số điện thoại không được tìm thấy.");
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    // Gửi yêu cầu GET tới API
                    var response = await client.GetStringAsync($"https://localhost:7066/api/Contacts/get-all-room/{currentUserPhone}");

                    // Deserialize kết quả JSON thành danh sách các phòng
                    var rooms = JsonConvert.DeserializeObject<List<Contact>>(response);

                    // Xóa nội dung cũ của DisplayContact trước khi hiển thị kết quả mới
                    DisplayContact.ItemsSource = null;

                    // Cập nhật DisplayContact với danh sách phòng
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
                // Định dạng lại messageText để bao gồm cả tên người gửi
                string formattedMessage = $"{ApplicationState.CurrentUserName} >>> {messageText}";

                // Hiển thị ngay lập tức tin nhắn đã gửi lên giao diện
                Dispatcher.Invoke(() =>
                {
                    //var messageBlock = new TextBlock
                    //{
                    //    Text = $"{DateTime.Now.ToShortTimeString()}: {formattedMessage}",
                    //    FontSize = 14,
                    //    Margin = new Thickness(3)
                    //};
                    //ChatDisplayArea.Children.Add(messageBlock);

                    ChatScrollViewer.ScrollToBottom();
                });

                await _messageHubConnection.InvokeAsync("SendMessage", fromNumber, formattedMessage, conversationId);
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

        // Làm mới danh sách liên hệ
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
                var conversationId = GetConversationId(GlobalClickPhoneNumber);

                // Hiển thị thanh tiến trình
                GlobalProgressBar.Visibility = Visibility.Visible;
                GlobalProgressBar.Value = 0;

                try
                {
                    using (var client = new HttpClient())
                    using (var form = new MultipartFormDataContent())
                    {
                        // Chuẩn bị file để gửi
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            var fileContent = new StreamContent(fileStream);
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                            // Add file to form
                            form.Add(fileContent, "file", fileName);
                        }

                        // Thêm các tham số khác với Content-Type xác định rõ ràng
                        var fromNumberContent = new StringContent(fromNumber);
                        fromNumberContent.Headers.ContentType = null; // Xóa bỏ content type để tránh lỗi BadRequest
                        form.Add(fromNumberContent, "fromNumber");

                        var conversationIdContent = new StringContent(conversationId.ToString());
                        conversationIdContent.Headers.ContentType = null; // Xóa bỏ content type để tránh lỗi BadRequest
                        form.Add(conversationIdContent, "conversationId");

                        // Gửi file với HttpClient
                        var response = await client.PostAsync("https://localhost:7066/api/File/SendFile", form);

                        // Kiểm tra lỗi phản hồi từ server
                        if (!response.IsSuccessStatusCode)
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Lỗi khi gửi file: {response.ReasonPhrase}\n{errorContent}");
                            return;
                        }

                        // Update progress bar - giả lập quá trình gửi file
                        for (int i = 1; i <= 100; i++)
                        {
                            await Task.Delay(30);  // Giả lập quá trình gửi file
                            GlobalProgressBar.Value = i;
                        }

                        // Lấy URL của file đã gửi
                        var responseBody = await response.Content.ReadAsStringAsync();
                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                        string fileUrl = jsonResponse.fileUrl;

                        // Sau khi gửi file thành công, thêm thông báo
                        var messageBlock = new TextBlock
                        {
                            Text = $"File '{fileName}' sent successfully",
                            FontSize = 14,
                            Margin = new Thickness(3)
                        };
                        ChatDisplayArea.Children.Add(messageBlock);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi gửi file: {ex.Message}");
                }
                finally
                {
                    // Ẩn thanh tiến trình sau khi hoàn thành
                    GlobalProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }



        private void ViewFile_Click(object sender, RoutedEventArgs e)
        {
            var fileUrl = ""; // Thay đổi để lấy URL của file đã gửi

            try
            {
                string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string savePath = Path.Combine(userFolder, "FileInChat");

                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                var fileName = Path.GetFileName(fileUrl);
                var filePath = Path.Combine(savePath, fileName);

                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri(fileUrl), filePath);
                }

                // Mở file đã lưu
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở file: {ex.Message}");
            }
        }


        private async void DisplayContact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedContact = DisplayContact.SelectedItem as Contact;
            if (selectedContact != null && _hubConnection.State == HubConnectionState.Connected)
            {
                // Reset giao diện tin nhắn cũ
                ChatDisplayArea.Children.Clear();

                GlobalClickName = selectedContact.ContactName;
                GlobalClickPhoneNumber = selectedContact.PhoneNumber;
                GlobalClickConversation = selectedContact.ConversationId.ToString();
                CurrentContactName.Content = selectedContact.ContactName;

                int conversationId = GetConversationId(selectedContact.PhoneNumber);

                // Join vào cuộc trò chuyện mới
                await _messageHubConnection.InvokeAsync("JoinConversation", conversationId);

                MessageBox.Show($"Bạn đã chọn liên hệ: {GlobalClickName}\nSố điện thoại: {GlobalClickPhoneNumber} id:{GlobalClickConversation}");
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
