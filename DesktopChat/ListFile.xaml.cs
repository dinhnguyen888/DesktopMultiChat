using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopChat
{
    public partial class ListFile : Window
    {
        private string _globalClickConversation;

        public ListFile(string GlobalClickConversation)
        {
            _globalClickConversation = GlobalClickConversation;
            InitializeComponent();
            LoadFiles();
        }

        private async void LoadFiles()
        {
            try
            {
                if (!int.TryParse(_globalClickConversation, out int conversationId))
                {
                    MessageBox.Show("ID cuộc hội thoại không hợp lệ");
                    return;
                }

                // Đường dẫn API sử dụng conversationId
                string apiUrl = $"https://localhost:7066/api/File/view-file/{conversationId}";

                using (HttpClient client = new HttpClient())
                {
                    // Gửi yêu cầu GET đến API
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    // Kiểm tra mã trạng thái của phản hồi
                    if (response.IsSuccessStatusCode)
                    {
                        // Đọc và log phản hồi JSON
                        var jsonResponse = await response.Content.ReadAsStringAsync();

                        // Deserialize JSON thành danh sách FileModel
                        var fileData = JsonConvert.DeserializeObject<List<FileModel>>(jsonResponse);

                        // Kiểm tra và hiển thị dữ liệu file
                        if (fileData != null && fileData.Count > 0)
                        {
                            FilesListBox.ItemsSource = fileData;

                            // Cuộn xuống cuối danh sách
                            FilesListBox.ScrollIntoView(fileData[^1]);
                        }
                        else
                        {
                            MessageBox.Show("Không có dữ liệu file để hiển thị.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        // Xử lý trường hợp lỗi từ server
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Không thể lấy dữ liệu từ server. Mã lỗi: {response.StatusCode}, Nội dung: {errorContent}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi nếu yêu cầu thất bại
                MessageBox.Show($"Lỗi khi gửi yêu cầu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xử lý sự kiện double click vào file trong danh sách
        private void FilesListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (FilesListBox.SelectedItem is FileModel selectedFile)
            {
                // YesNo question
                MessageBoxResult result = MessageBox.Show(
                    $"Mở file: {selectedFile.FileName} ?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    // Open URL
                    Process.Start(new ProcessStartInfo(selectedFile.FileUrl) { UseShellExecute = true });
                }
                else
                {
                    return;
                }
            }
        }

        // Sự kiện Loaded của ListBox
        private void FilesListBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Cuộn xuống cuối danh sách khi ListBox được tải
            if (FilesListBox.Items.Count > 0)
            {
                FilesListBox.ScrollIntoView(FilesListBox.Items[FilesListBox.Items.Count - 1]);
            }
        }
    }

    // Model đại diện cho dữ liệu file
    public class FileModel
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }
}
