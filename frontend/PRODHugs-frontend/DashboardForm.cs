using PRODHugs_frontend.Util;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace PRODHugs_frontend
{
    public partial class DashboardForm : Form
    {
        private readonly UserData _user;
        private readonly HttpClient _client;
        private bool _closeFlag = true;

        public DashboardForm(UserData user, HttpClient client)
        {
            _user = user;
            _client = client;
            InitializeComponent();
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            // name
            helloLabel.Text = $"Привет, {_user.DisplayName}";
            // coins
            coinsLabel.Text = _user.Coins.ToString();
            // stats
            AcceptedHugsLabel.Text = _user.AcceptedHugs.ToString();
            InitiatedHugsLabel.Text = _user.InitiatedHugs.ToString();
            TotalHugsLabel.Text = _user.TotalHugs.ToString();
            RankLabel.Text = _user.Rank;
            // history
            HugHistoryGrid.DataSource = _user.HugsHistory;
            HugHistoryGrid.Columns["GiverDisplayName"].HeaderText = "Кто обнял(-а)";
            HugHistoryGrid.Columns["ReceiverDisplayName"].HeaderText = "Кого обнял(-и)";
            // inbox
            HugsInboxGrid.DataSource = _user.HugsInbox;
            HugsInboxGrid.Columns["GiverDisplayName"].Visible = false;
            HugsInboxGrid.Columns["ReceiverDisplayName"].HeaderText = "Кто хочет обнять";
        }

        private void DashboardForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_closeFlag) Application.Exit();
        }

        private void UsersButton_Click(object sender, EventArgs e)
        {
            UsersForm users = new()
            {
                Location = Location
            };
            users.Show();
            _closeFlag = false;
            Close();
        }

        private async void DailyRewardButton_click(object sender, EventArgs e)
        {
            string? token = Properties.Settings.Default["token_itself"]?.ToString();
            var handler = HttpMessageCreator.CreatePost(token, new Uri("https://xn--80ahmbjkfgik8g.xn--p1ai/api/v1/daily-reward"));
            var response = await _client.SendAsync(handler);
            Debug.WriteLine((int)response.StatusCode);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode.Equals(429))
                    MessageBox.Show("Вы уже забрали награду!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Не удалось забрать награду.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DailyRewardButton.Enabled = false;
                return;
            }
            DailyRewardButton.Enabled = false;
            var responseJ = await response.Content.ReadAsStringAsync();
            JsonNode? json = JsonNode.Parse(responseJ);
            int dailyMoney = json!["amount"]!.GetValue<int>();
            bool alreadyClaimed = json!["already_claimed"]!.GetValue<bool>();
            if (alreadyClaimed) MessageBox.Show("Вы уже забрали награду!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            _user.Coins += dailyMoney;
            coinsLabel.Text = _user.Coins.ToString();
        }
    }
}
