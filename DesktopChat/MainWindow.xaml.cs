using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public static class ApplicationState
    {
        public static string CurrentUserName { get; set; }
        public static int? CurrentUserId { get; set; }
        public static string? CurrentUserPhone { get; set; }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoginControl loginControl = new LoginControl();
            MainGrid.Children.Add(loginControl);
        }
    }
    
}