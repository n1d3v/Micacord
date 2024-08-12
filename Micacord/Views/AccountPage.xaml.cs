using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Micacord.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Windows.Storage;

namespace Micacord.Views
{
    public sealed partial class AccountPage : Page
    {
        private readonly HttpClient _httpClient;

        public AccountViewModel ViewModel { get; }

        public AccountPage()
        {
            ViewModel = App.GetService<AccountViewModel>();
            InitializeComponent();
            _httpClient = new HttpClient();

            // Attempt auto-login
            CheckForExistingTokenAndLoginAsync();
        }

        public class LoginRequest
        {
            [JsonProperty("login")]
            public string Email { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("undelete")]
            public bool Undelete { get; set; } = false;

            [JsonProperty("login_source")]
            public object LoginSource { get; set; } = null;

            [JsonProperty("gift_code_sku_id")]
            public object GiftCodeSkuId { get; set; } = null;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = emailBox.Text;
            string password = passwordBox.Password;

            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var json = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://discord.com/api/v9/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    string token = responseObject?.token;

                    if (token != null)
                    {
                        var localSettings = ApplicationData.Current.LocalSettings;
                        localSettings.Values["token"] = token;
                        await ShowDialog("Logged in", "You are logged in!");
                    }
                    else
                    {
                        await ShowDialog("Error", "Token not found in response.");
                        Console.WriteLine("Token not found in response.");
                    }
                }
                else
                {
                    await ShowDialog("Error", $"Login failed: {responseContent}");
                    Console.WriteLine($"Error: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                await ShowDialog("Error", $"An error occurred: {ex.Message}");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task CheckForExistingTokenAndLoginAsync()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.TryGetValue("token", out var tokenObject))
            {
                string token = tokenObject as string;

                if (!string.IsNullOrEmpty(token))
                {
                    var isAuthenticated = await AuthenticateWithTokenAsync(token);

                    if (isAuthenticated)
                    {
                        await ShowDialog("Auto-Login", "You are already logged in!");
                    }
                    else
                    {
                        await ShowDialog("Error", "Token is invalid or expired. Please log in again.");
                    }
                }
            }
        }

        private async Task<bool> AuthenticateWithTokenAsync(string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/v9/users/@me");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token);

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during token authentication: {ex.Message}");
                return false;
            }
        }

        private async Task ShowDialog(string title, string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = ContentArea.XamlRoot,
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary
            };

            await dialog.ShowAsync();
        }
    }
}
