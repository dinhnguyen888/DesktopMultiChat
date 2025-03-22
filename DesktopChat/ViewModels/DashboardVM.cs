using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using DesktopChat.Models;
using DesktopChat.Services;
using DesktopChat.Commands;
using System.Diagnostics;
using Wpf.Ui.Input;
using System.Windows;

namespace DesktopChat.ViewModels
{
    public class DashboardVM : BaseVM
    {
        // 🏷️ Services
        private readonly AccountService _accountService;
        private readonly RoomService _roomService;

        // 🏷️ Collections
        public ObservableCollection<Account> Accounts { get; set; }
        public ObservableCollection<RoomGet> Rooms { get; set; }
        public ObservableCollection<AccountViewStatus> OnlineAccountViews { get; set; }

        // 🏷️ Đếm số lượng người dùng online
        private int _onlineAccountCount;
        public int OnlineAccountCount
        {
            get => _onlineAccountCount;
            set
            {
                _onlineAccountCount = value;
                OnPropertyChanged(); // Thông báo cho View cập nhật
            }
        }

        // 🏷️ Lệnh để làm mới dữ liệu
        public ICommand RefreshCommand { get; }
        public ICommand MessageAccountCommand { get; }

        // 🏷️ Constructor
        public DashboardVM()
        {
            _accountService = new AccountService();
            _roomService = new RoomService();

            Accounts = new ObservableCollection<Account>();
            Rooms = new ObservableCollection<RoomGet>();
            OnlineAccountViews = new ObservableCollection<AccountViewStatus>();

            RefreshCommand = new RelayCommand(async _ => await LoadData());
            MessageAccountCommand = new RelayCommand<Guid>(SendMessage);

            LoadData();
           
        }

        // 📌 Tải toàn bộ dữ liệu từ service
        private async Task LoadData()
        {
            await Task.WhenAll(LoadOnlineUsers(), LoadRooms());
        }

        // 📌 Lấy danh sách người dùng online
        private async Task LoadOnlineUsers()
        {
            try
            {
                var onlineAccounts = await _accountService.ViewOnlineAccountAsync();
                Console.WriteLine(onlineAccounts);
                if (onlineAccounts != null)
                {
                    OnlineAccountViews.Clear();

                    foreach (var account in onlineAccounts)
                    {
                        if (account.IsOnline == true) 
                        {
                            OnlineAccountViews.Add(account);
                        }
                    }

                   
                    OnlineAccountCount = OnlineAccountViews.Count;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading online users: {ex.Message}");
            }
        }

        // 📌 Lấy danh sách phòng chat
        private async Task LoadRooms()
        {
            try
            {
                var rooms = await _roomService.GetAllRoomAsync();

                if (rooms != null)
                {
                    Rooms.Clear();

                    foreach (var room in rooms)
                    {
                        Rooms.Add(room);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading rooms: {ex.Message}");
            }
        }

        

        // 📌 Xử lý gửi tin nhắn
        private void SendMessage(Guid accountId)
        {
            MessageBox.Show($"Send message to account: {accountId}");


        }
    }
}
