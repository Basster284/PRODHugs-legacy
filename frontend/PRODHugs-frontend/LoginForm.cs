using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;

namespace PRODHugs_frontend
{
    public partial class LoginForm : Form
    {
        bool isEng = false;
        ComponentResourceManager resources = new ComponentResourceManager(typeof(LoginForm));
        public LoginForm()
        {
            InitializeComponent();
        }

        public void changeLanguage(object sender, EventArgs e)
        {
            if (!isEng)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                isEng = true;
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
                isEng = false;
            }
            Controls.Clear();
            InitializeComponent();
        }

        async void loginButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(usernameInput.Text) ||
                String.IsNullOrEmpty(passwordInput.Text))
            {
                MessageBox.Show(resources.GetString("EmptyLoginOrPassword"), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LoadingForm loading = new LoadingForm();
            loading.Show();
            JsonObject data = await login();
            if (data == null)
            {
                loading.Close();
                MessageBox.Show(resources.GetString("RequestFailed"), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!data.ContainsKey("token"))
            {
                loading.Close();
                MessageBox.Show(resources.GetString("WrongLoginOrPassword"), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            loading.Close();
        }

        async Task<JsonObject?> login()
        {
            JsonObject t = new JsonObject();
            t.Add("username", usernameInput.Text);
            t.Add("password", passwordInput.Text);
            HttpClient client = new HttpClient();
            try
            {
                StringContent content = new StringContent(t.ToJsonString(), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://xn--80ahmbjkfgik8g.xn--p1ai/api/v1/auth/login", content);
                string responseJ = await response.Content.ReadAsStringAsync();
                return JsonNode.Parse(responseJ)?.AsObject();
            } catch (Exception ex)
            {
                Debug.WriteLine($"FUCK: {ex.Message}");
                return null;
            }
        }
    }
}
