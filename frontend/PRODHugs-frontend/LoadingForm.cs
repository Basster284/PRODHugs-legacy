using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PRODHugs_frontend
{
    public partial class LoadingForm : Form
    {
        private readonly string _username;
        private readonly string _password;
        private readonly LoginForm _loginForm;
        private readonly HttpClient _client;
        private readonly CookieContainer _container;

        public LoadingForm(LoginForm form, string login, string password)
        {
            _loginForm = form;
            _username = login;
            _password = password;
            _container = new();
            HttpClientHandler handler = new() { CookieContainer = _container };
            _client = new HttpClient(handler);
            InitializeComponent();
        }

        private async Task<HttpRequestMessage> CreateHeadersGet(string token, string uri)
        {
            HttpRequestMessage message = new(HttpMethod.Get, new Uri(uri));
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return message;
        }

        private async Task<UserData?> Login()
        {
            JsonObject t = new()
            {
                ["username"] = _username,
                ["password"] = _password
            };
            try
            {
                StringContent content = new(t.ToJsonString(), System.Text.Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("https://xn--80ahmbjkfgik8g.xn--p1ai/api/v1/auth/login", content);
                if (!response.IsSuccessStatusCode) return null;
                string responseJ = await response.Content.ReadAsStringAsync();
                // cookie (why)
                if (_container.Count > 0)
                {
                    var cookie = _container.GetCookies(new Uri("https://xn--80ahmbjkfgik8g.xn--p1ai/api/v1/auth/login"));
                    var refreshToken = cookie["refresh_token"]?.Value;
                    Properties.Settings.Default["refresh_token"] = refreshToken;
                    Properties.Settings.Default.Save();
                }
                string? token = JsonNode.Parse(responseJ)?["token"]?.GetValue<string>();
                JsonNode? userJ = JsonNode.Parse(responseJ)?["user"];
                // the most of user
                UserData user = new()
                {
                    DisplayName = userJ["display_name"]?.GetValue<string>(),
                    Username = userJ["username"]?.GetValue<string>(),
                    Gender = userJ["gender"]?.GetValue<string>(),
                    Tag = userJ["tag"]?.GetValue<string>(),
                    TelegramId = userJ["telegram_id"]?.GetValue<int>() ?? 0,
                    Id = userJ["id"]?.GetValue<string>()
                };
                // coins
                var coinsResponse = await _client.SendAsync(await CreateHeadersGet(token, "https://xn--80ahmbjkfgik8g.xn--p1ai/api/v1/balance"));
                if (!coinsResponse.IsSuccessStatusCode) return null;
                string coinsResponseJ = await coinsResponse.Content.ReadAsStringAsync();
                user.Coins = JsonNode.Parse(coinsResponseJ)!["amount"]!.GetValue<int>();
                // hugs history
                var hugsHistoryResponse = await _client.SendAsync(await CreateHeadersGet(token, "https://xn--80ahmbjkfgik8g.xn--p1ai/api/v1/hugs/history"));
                if (!hugsHistoryResponse.IsSuccessStatusCode) return null;
                string hugsResponseJ = await hugsHistoryResponse.Content.ReadAsStringAsync();
                foreach (var hug in JsonNode.Parse(hugsResponseJ)!.AsArray())
                {
                    user.HugsHistory.Add(new HugElement(hug["giver_display_name"]?.GetValue<string>() ?? hug["giver_username"]?.GetValue<string>(), 
                        hug["receiver_display_name"]?.GetValue<string>() ?? hug["receiver_username"]?.GetValue<string>()));
                }
                // hugs inbox
                var hugsInboxResponse = await _client.SendAsync(await CreateHeadersGet(token, "https://xn--80ahmbjkfgik8g.xn--p1ai/api/v1/hugs/inbox"));
                if (!hugsInboxResponse.IsSuccessStatusCode) return null;
                string hugsInboxResponseJ = await hugsInboxResponse.Content.ReadAsStringAsync();
                foreach (var hug in JsonNode.Parse(hugsInboxResponseJ)!.AsArray())
                {
                    user.HugsInbox.Add(new HugElement(hug["giver_display_name"]?.GetValue<string>() ?? hug["giver_username"]?.GetValue<string>(),
                        hug["receiver_display_name"]?.GetValue<string>() ?? hug["receiver_username"]?.GetValue<string>()));
                }
                // oh my god
                var totalHugsResponse = await _client.SendAsync(await CreateHeadersGet(token, $"https://xn--80ahmbjkfgik8g.xn--p1ai/api/v1/users/{user.Id}/profile"));
                if (!totalHugsResponse.IsSuccessStatusCode) return null;
                string totalHugsResponseJ = await totalHugsResponse.Content.ReadAsStringAsync();
                JsonNode? totalHugsJ = JsonNode.Parse(totalHugsResponseJ);
                user.TotalHugs = totalHugsJ?["total_hugs"]?.GetValue<int?>() ?? 0;
                user.AcceptedHugs = totalHugsJ?["hugs_received"]?.GetValue<int?>() ?? 0;
                user.InitiatedHugs = totalHugsJ?["hugs_given"]?.GetValue<int?>() ?? 0;
                user.Rank = totalHugsJ?["rank"]?.GetValue<string>();
                return user;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async void LoadingForm_Load(object sender, EventArgs e)
        {
            UserData? user = await Login();
            if (user == null)
            {
                Hide();
                MessageBox.Show("Не удалось войти.", _loginForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            _loginForm.Hide();
            _client.Dispose();
            DashboardForm dashboard = new(user);
            dashboard.Show();
            Close();
        }
    }
}
