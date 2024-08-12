using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Micacord.UserControls;
using Newtonsoft.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Micacord.Views
{
    public sealed partial class ServersPage : Page
    {
        private readonly HttpClient _httpClient;

        public ServersPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();

            // Attempt auto-login and fetch guilds
            CheckForExistingTokenAndLoginAsync();
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
                        await FetchGuildsAsync(token);
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
                var responseContent = await response.Content.ReadAsStringAsync();

                // Log the response for debugging
                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Content: {responseContent}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during token authentication: {ex.Message}");
                return false;
            }
        }

        private async Task FetchGuildsAsync(string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/v9/users/@me/guilds");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token);

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var guilds = JsonConvert.DeserializeObject<List<Guild>>(responseContent);
                    PopulateServerList(guilds);
                }
                else
                {
                    await ShowDialog("Error", "Failed to fetch guilds.");
                    Console.WriteLine($"Error fetching guilds: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                await ShowDialog("Error", $"An error occurred while fetching guilds: {ex.Message}");
                Console.WriteLine($"Error fetching guilds: {ex.Message}");
            }
        }

        private void PopulateServerList(List<Guild> guilds)
        {
            ServerListPanel.Children.Clear();

            foreach (var guild in guilds)
            {
                var serverItem = new ServerListItem
                {
                    ServerName = guild.name,
                    ServerPicture = new BitmapImage(new Uri(guild.iconUrl))
                };

                ServerListPanel.Children.Add(serverItem);
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

        public class Guild
        {
            public string id { get; set; }
            public string name { get; set; }
            public string icon { get; set; }

            // Assuming Discord uses a URL pattern for guild icons
            public string iconUrl => string.IsNullOrEmpty(icon) ? null : $"https://cdn.discordapp.com/icons/{id}/{icon}.png";
        }
    }
}
