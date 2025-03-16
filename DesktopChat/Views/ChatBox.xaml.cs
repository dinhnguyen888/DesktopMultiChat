using DesktopChat.Services;
using DesktopChat.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopChat.Views
{
    /// <summary>
    /// Interaction logic for ChatBox.xaml
    /// </summary>
    public partial class ChatBox : UserControl
    {
        public ChatBox()
        {
            InitializeComponent();
            var viewModel = new ChatVM(new RoomService(), new MessageService());
            DataContext = viewModel;
            viewModel.ScrollToBottom = () =>
            {
                MessagesScrollViewer.ScrollToBottom();
            };
        }


        private void SendMsg_OnClick(object sender, RoutedEventArgs e)
        {
            MessagesScrollViewer.ScrollToBottom();
        }
        private void Msg_Input_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SendMsg_OnClick(SendBtn, new RoutedEventArgs());
                SendBtn.Command.Execute(null);
                e.Handled = true;
            }
        }


    }
}
