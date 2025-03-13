using DesktopChat.Models;
using System.Net.Http;
using System.Windows.Input;
using DesktopChat.Services;
using DesktopChat.Commands;
using System.Windows;
using DesktopChat.Helpers;
using System.Windows.Xps;
using System.Net;
using DesktopChat.Interfaces;
namespace DesktopChat.ViewModels
{
    public class LoginVM : BaseVM
    {
        private Login _user;
        private string isHidden = "Hidden";
        private readonly IAuthService _authService;

        private readonly Action _onLoginSuccess;

        public ICommand LoginCommand { get; }

        // ➡️ Constructor mặc định để dùng trong XAML
        public LoginVM() : this(new AuthService(), () => { }) { }

        public LoginVM(IAuthService authService, Action onLoginSuccess)
        {
            _user = new Login();
            _authService = authService;
            _onLoginSuccess = onLoginSuccess;

            LoginCommand = new RelayCommand(async _ => await LoggedIn());
        }

        public string Email
        {
            get => _user.Email;
            set
            {
                _user.Email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Password
        {
            get => _user.Password;
            set
            {
                _user.Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string IsHidden
        {
            get => isHidden;
            set
            {
                isHidden = value;
                OnPropertyChanged(nameof(IsHidden));
            }
        }

        private async Task LoggedIn()
        {
            IsHidden = "Visible";
            try
            {
                var token = await _authService.LoginAsync(Email, Password);

               
                GlobalVariableHelper.AccessToken = token.AccessToken;
                GlobalVariableHelper.RefreshToken = token.RefreshToken;

                string userEmail = GlobalVariableHelper.GetUserInfoFromToken("email");

                MessageBox.Show($"Login success!\nEmail: {userEmail}");
                _onLoginSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                // Trường hợp _authService sử dụng một client HTTP khác
                if (ex.Message.Contains("401"))
                {
                    MessageBox.Show("Login failed: Invalid Email or Password");
                }
                else
                {
                    MessageBox.Show($"Login failed: {ex.Message}");
                }
            }
            finally
            {
                IsHidden = "Hidden";
            }
        }
    }
}
