using DesktopChat.Commands;
using System.Windows.Input;

namespace DesktopChat.ViewModels
{
    public class SettingVM : BaseVM
    {
        private string _userName;
        private string _email;

        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public ICommand LogoutCommand { get; }
        public ICommand EditProfileCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        public SettingVM()
        {
            UserName = "Nguyễn Đình";
            Email = "dinh@example.com";

            LogoutCommand = new RelayCommand(Logout);
            EditProfileCommand = new RelayCommand(EditProfile);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
        }

        private void Logout()
        {
            // Xử lý đăng xuất
            System.Windows.MessageBox.Show("Đăng xuất thành công!");
        }

        private void EditProfile()
        {
            // Xử lý thay đổi thông tin
            System.Windows.MessageBox.Show("Chuyển đến trang chỉnh sửa thông tin.");
        }

        private void ChangePassword()
        {
            // Xử lý đổi mật khẩu
            System.Windows.MessageBox.Show("Chuyển đến trang đổi mật khẩu.");
        }
    }
}
