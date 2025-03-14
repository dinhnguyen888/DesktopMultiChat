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
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
            }
        }

      
        public ICommand NavigateCommand { get; }

        public MainVM()
        {
          

            
    
        }

       

    }
}
