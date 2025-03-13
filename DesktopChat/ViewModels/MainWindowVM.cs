using DesktopChat.Views;
using System.Windows.Input;
using DesktopChat.Commands;
using DesktopChat.Models;
using DesktopChat.Services;

namespace DesktopChat.ViewModels
{
    public class MainWindowVM : BaseVM
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private readonly AuthService _authService;

        public MainWindowVM()
        {
            _authService = new AuthService();

            // Mặc định hiển thị LoginView
            var loginVM = new LoginVM(_authService, OnLoginSuccess);
            CurrentView = new LoginView { DataContext = loginVM };

            
        }

        private void OnLoginSuccess()
        {
            // Chuyển từ LoginView sang MainView sau khi đăng nhập thành công
            CurrentView = new MainView();
        }

       
    }
}
