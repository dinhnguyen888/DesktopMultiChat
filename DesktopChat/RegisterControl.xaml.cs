using System.IO;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net.Http;
using System.Text;

namespace DesktopChat
{
    public partial class RegisterControl : UserControl
    {
        public RegisterControl()
        {
            InitializeComponent();
           
        }

      

        private void LoginLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            var loginControl = new LoginControl();
            mainWindow.Content = loginControl;
        }

        private async void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra nếu bất kỳ trường nào trống
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrWhiteSpace(PassBox.Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassBox.Password))
            {
                MessageBox.Show("Các trường không được để trống");
                return;
            }

            // Kiểm tra nếu mật khẩu không khớp
            if (PassBox.Password != ConfirmPassBox.Password)
            {
                MessageBox.Show("Xác nhận mật khẩu không chính xác");
                return;
            }

            // Tạo đối tượng để gửi đi
            var user = new
            {
                fullName = NameBox.Text,
                phoneNumber = PhoneBox.Text,
                password = PassBox.Password
            };

            try
            {
                using (var client = new HttpClient())
                {
                    // Cấu hình API URL từ cấu hình (nếu có)
                    var apiUrl = "https://localhost:7066/api/Register";

                    // Chuyển đối tượng thành JSON
                    var json = JsonConvert.SerializeObject(user);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Gửi yêu cầu POST
                    var response = await client.PostAsync(apiUrl, content);

                    // Xử lý phản hồi từ server
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Đăng ký thành công!");
                        var loginControl = new LoginControl();
                        var mainWindow = Window.GetWindow(this) as MainWindow;
                        mainWindow.Content = loginControl;
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Lỗi: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }

            
        }

    }
}
