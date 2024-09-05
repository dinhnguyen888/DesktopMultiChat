using System.Net.Http;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Newtonsoft.Json;

namespace DesktopChat
{
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();
        }

        private void RegisterLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Lấy tham chiếu đến MainWindow
            var mainWindow = Window.GetWindow(this) as MainWindow;

            // Tạo instance của RegisterControl
            var registerControl = new RegisterControl();

            // Thay thế nội dung hiện tại của MainWindow bằng RegisterControl
            mainWindow.Content = registerControl;
        }

        private async void LoginBtn_Click(Object sender, RoutedEventArgs e)
        {
            // Kiểm tra nếu bất kỳ trường nào trống
            if (string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrWhiteSpace(PassBox.Password))
            {
                MessageBox.Show("Số điện thoại và mật khẩu không được để trống");
                return;
            }

            // Tạo đối tượng để gửi đi
            var loginData = new
            {
                phoneNumber = PhoneBox.Text,
                password = PassBox.Password
            };

            try
            {
                using (var client = new HttpClient())
                {
                    var apiUrl = "https://localhost:7066/api/Login";
                    var json = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Giả sử API trả về thông tin người dùng
                        var responseBody = await response.Content.ReadAsStringAsync();
                        dynamic user = JsonConvert.DeserializeObject(responseBody);

                        // Cập nhật ApplicationState
                        ApplicationState.CurrentUserName = user.fullName;
                        ApplicationState.CurrentUserId = user.id;
                        ApplicationState.CurrentUserPhone = user.phoneNumber;
                        MessageBox.Show("Đăng nhập thành công!");

                        var mainWindow = Window.GetWindow(this) as MainWindow;
                        var chatControl = new ChatControl();

                        //mainWindow.MainGrid.Children.Clear();
                        mainWindow.Content = chatControl;

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
