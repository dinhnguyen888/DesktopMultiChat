using DesktopChat.Views;
using DesktopChat.Services;
using System.Windows;
using DesktopChat.Views.Windows;

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

            ShowLoginDialog();
        }

        private void ShowLoginDialog()
        {
            var loginVM = new LoginVM(_authService, OnLoginSuccess);
            var loginWindow = new LoginWindow
            {
                DataContext = loginVM
            };

            // Hide MainWindow before showing LoginWindow
            Application.Current.MainWindow.Hide();

            // Show LoginWindow as dialog
            bool? result = loginWindow.ShowDialog();

            if (result == true)
            {
                
                Application.Current.MainWindow.Show();
                CurrentView = new MainView(); // Switch to MainView after successful login
            }
            else
            {
                // Close application
                Application.Current.Shutdown();
            }
        }
          private void OnLoginSuccess()
        {
            // Switch to MainView after successful login
            CurrentView = new MainView();
        }
    }
}
