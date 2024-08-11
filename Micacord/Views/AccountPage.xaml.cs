using System.Text;
using Micacord.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;

namespace Micacord.Views
{
    public sealed partial class AccountPage : Page
    {
        private readonly HttpClient _httpClient;

        public AccountViewModel ViewModel
        {
            get;
        }

        public AccountPage()
        {
            ViewModel = App.GetService<AccountViewModel>();
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        public class LoginRequest
        {
            [JsonProperty("login")]
            public string Email
            {
                get; set;
            }

            [JsonProperty("password")]
            public string Password
            {
                get; set;
            }

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
                    await ShowDialog("Logged in", "You are logged in!");
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
