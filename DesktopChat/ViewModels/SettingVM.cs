using DesktopChat.Commands;
using System;
using System.Windows.Input;

namespace DesktopChat.ViewModels
{
    public class SettingVM : BaseVM
    {
        private string _fullName;
        private string _email;
        private DateTime? _dateBirth;
        private string _phoneNumber;
        private string _role;

        public string FullName
        {
            get => _fullName;
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    OnPropertyChanged(nameof(FullName));
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

        public DateTime? DateBirth
        {
            get => _dateBirth;
            set
            {
                if (_dateBirth != value)
                {
                    _dateBirth = value;
                    OnPropertyChanged(nameof(DateBirth));
                }
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                }
            }
        }

        public string Role
        {
            get => _role;
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OnPropertyChanged(nameof(Role));
                }
            }
        }

        public ICommand LogoutCommand { get; }
        public ICommand EditProfileCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand SaveProfileCommand { get; }

        public SettingVM()
        {
            // Mock data - Thay bằng dữ liệu thực tế khi tích hợp backend
            FullName = "Nguyễn Đình";
            Email = "dinh@example.com";
            DateBirth = new DateTime(2000, 1, 1);
            PhoneNumber = "0123456789";
            Role = "Admin";

            LogoutCommand = new RelayCommand(Logout);
            EditProfileCommand = new RelayCommand(EditProfile);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
            SaveProfileCommand = new RelayCommand(SaveProfile);
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

        private void SaveProfile()
        {
            // Xử lý lưu thông tin
            System.Windows.MessageBox.Show("Thông tin đã được cập nhật thành công!");
        }
    }
}
