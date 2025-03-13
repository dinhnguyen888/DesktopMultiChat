using DesktopChat.Commands;
using DesktopChat.Views;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace DesktopChat.ViewModels
{
    public class MainVM : BaseVM
    {
        private UserControl _currentView;
        private bool isLoading = false;
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
        public ICommand NavigateCommand { get; }

        public MainVM()
        {
         

            //CurrentView = new DashboardView();
            //NavigateCommand = new RelayCommand(Navigate);
        }

        //private void Navigate(object parameter)
        //{
        //    if (parameter is Type targetType)
        //    {
        //        var instance = Activator.CreateInstance(targetType);
        //        if (instance is UserControl view)
        //        {
        //            CurrentView = view;
        //        }
        //    }
        //}
    }
}
