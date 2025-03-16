using System.Linq;
using System.Collections.ObjectModel;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using DesktopChat.ViewModels;

public class DashboardVM : BaseVM
{
    public ObservableCollection<Account> Accounts { get; set; }
    public ObservableCollection<Room> Rooms { get; set; }

    public SeriesCollection OverallProgressSeries { get; set; }
    public SeriesCollection IndividualProgressSeries { get; set; }

    private string[] _employeeNames;
    public string[] EmployeeNames
    {
        get => _employeeNames;
        set
        {
            _employeeNames = value;
            OnPropertyChanged();
        }
    }

    public Func<double, string> YFormatter { get; set; }

    public DashboardVM()
    {
        Accounts = new ObservableCollection<Account>
        {
            new Account { Username = "John Doe", StatusColor = Brushes.Green },
            new Account { Username = "Jane Smith", StatusColor = Brushes.Red }
        };

        Rooms = new ObservableCollection<Room>
        {
            new Room { RoomName = "Room A" },
            new Room { RoomName = "Room B" }
        };

        // Cập nhật EmployeeNames khi Accounts thay đổi
        UpdateEmployeeNames();

        OverallProgressSeries = new SeriesCollection
        {
            new PieSeries { Title = "Hoàn thành", Values = new ChartValues<double> { 70 }, Fill = Brushes.Green },
            new PieSeries { Title = "Chưa hoàn thành", Values = new ChartValues<double> { 30 }, Fill = Brushes.Red }
        };

        IndividualProgressSeries = new SeriesCollection
    {
        new RowSeries
        {
            Title = "John Doe",
            Values = new ChartValues<double> { 80 },
            Fill = Brushes.Green,
        },
        new RowSeries
        {
            Title = "John Die",
            Values = new ChartValues<double> { 80 },
            Fill = Brushes.Yellow,
        },
        new RowSeries
        {
            Title = "John Fuck",
            Values = new ChartValues<double> { 80 },
            Fill = Brushes.Gray,
        },
        new RowSeries
        {
            Title = "Jane Smith",
            Values = new ChartValues<double> { 60 },
            Fill = Brushes.Blue,
        }
    };

        // Cập nhật EmployeeNames khi SeriesCollection thay đổi
        IndividualProgressSeries.CollectionChanged += (s, e) => UpdateEmployeeNames();
        UpdateEmployeeNames();

        YFormatter = value => $"{value}%";
    }

    private void UpdateEmployeeNames()
    {
        EmployeeNames = Accounts.Select(a => a.Username).ToArray();
    }
}

public class Account
{
    public string Username { get; set; }
    public Brush StatusColor { get; set; }
}

public class Room
{
    public string RoomName { get; set; }
}
