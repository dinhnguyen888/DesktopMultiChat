using DesktopChat.Helpers;
using DesktopChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace DesktopChat.Services
{
    public class AccountService : BaseService
    {
        public async Task<IEnumerable<Account>> GetAllAccountAsync()
        {
            var accounts = await GetAsync<IEnumerable<Account>>("Account", "no_token");
            return accounts ?? new List<Account>();
        }
        
        public async Task<List<AccountViewStatus>> ViewOnlineAccountAsync()
        {
            try
            {

                var accounts = await GetAsync<List<AccountViewStatus>>("Account/online");
                return accounts;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ViewOnlineAccountAsync: {ex.Message}");
                return null;
            }
        }
    }
}
