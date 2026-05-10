using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRODHugs_frontend
{
    public partial class LoadingForm : Form
    {
        string username;
        string password;
        LoginForm loginForm;

        public LoadingForm(LoginForm form, string login, string password)
        {
            loginForm = form;
            username = login;
            this.password = password;
            InitializeComponent();
        }

        async Task<User?> login()
        {
            JsonObject t = new JsonObject();
            t["username"] = username;
            t["password"] = password;
            CookieContainer container = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = container };
            HttpClient client = new HttpClient(handler);
            try
            {
                StringContent content = new StringContent(t.ToJsonString(), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://xn--80ahmbjkfgik8g.xn--p1ai/api/v1/auth/login", content);
                if (!response.IsSuccessStatusCode) return null;
                string responseJ = await response.Content.ReadAsStringAsync();
                // cookie (why)
                if (container.Count > 0)
                {
                    var cookie = container.GetCookies(new Uri("https://xn--80ahmbjkfgik8g.xn--p1ai/api/v1/auth/login"));
                    var refreshToken = cookie["refresh_token"]?.Value;
                    Properties.Settings.Default["refresh_token"] = refreshToken;
                    Properties.Settings.Default.Save();
                }
                client.Dispose();
                return JsonNode.Parse(responseJ)?["user"].Deserialize<User>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async void LoadingForm_Load(object sender, EventArgs e)
        {
            User? user = await login();
            if (user == null)
            {
                Hide();
                loginForm.Show();
                MessageBox.Show("Не удалось войти.", loginForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            loginForm.Hide();
            DashboardForm dashboard = new(user);
            dashboard.Show();
            Close();
        }
    }
}
