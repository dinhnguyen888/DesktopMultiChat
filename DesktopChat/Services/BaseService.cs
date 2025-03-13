using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DesktopChat.Services
{
    public class BaseService 
    {
        private readonly HttpClient _httpClient;
        protected readonly string BaseUrl = "https://localhost:7202/api";
        private const string NO_TOKEN = "no_token";

        public BaseService()
        {
            _httpClient = new HttpClient();
        }

        // Method to add authentication header token for request
        // Parameter accessToken is optional
        protected void AddAuthenticationHeader(string accessToken = null)
        {
            // If accessToken is NO_TOKEN, return
            if (accessToken == NO_TOKEN)
            {
                return;
            }

            // If accessToken is null or empty, use the global variable
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = Helpers.GlobalVariableHelper.AccessToken;
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        // Method to delete authentication header token after request
        protected void RemoveAuthenticationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        protected async Task<T> GetAsync<T>(string endpoint, string accessToken = null)
        {
            AddAuthenticationHeader(accessToken);

            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{endpoint}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(json) ??
                       throw new InvalidOperationException($"Failed to deserialize response from {endpoint}");
            }
            finally
            {
                RemoveAuthenticationHeader();
            }
        }

        protected async Task<T> PostAsync<T>(string endpoint, object data, string accessToken = null)
        {
            AddAuthenticationHeader(accessToken);

            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{BaseUrl}/{endpoint}", content);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(result) ??
                       throw new InvalidOperationException($"Failed to deserialize response from {endpoint}");
            }
            finally
            {
                RemoveAuthenticationHeader();
            }
        }

        protected async Task<T> PutAsync<T>(string endpoint, object data, string accessToken = null)
        {
            AddAuthenticationHeader(accessToken);

            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{BaseUrl}/{endpoint}", content);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(result) ??
                       throw new InvalidOperationException($"Failed to deserialize response from {endpoint}");
            }
            finally
            {
                RemoveAuthenticationHeader();
            }
        }

        protected async Task<bool> DeleteAsync(string endpoint, string accessToken = null)
        {
            AddAuthenticationHeader(accessToken);

            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{endpoint}");
                return response.IsSuccessStatusCode;
            }
            finally
            {
                RemoveAuthenticationHeader();
            }
        }
    }
}