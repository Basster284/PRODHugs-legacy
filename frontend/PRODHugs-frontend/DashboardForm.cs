using System.Diagnostics;

namespace PRODHugs_frontend
{
    public partial class DashboardForm : Form
    {
        private readonly UserData _user;
        private bool _closeFlag = true;

        public DashboardForm(UserData user)
        {
            _user = user;
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
            HugHistoryGrid.Columns["GiverDisplayName"].HeaderText = "Кто обнял";
            HugHistoryGrid.Columns["ReceiverDisplayName"].HeaderText = "Кого обняли";
            // inbox
            HugsInboxGrid.DataSource = _user.HugsInbox;
            HugsInboxGrid.Columns["GiverDisplayName"].Visible = false;
            HugsInboxGrid.Columns["ReceiverDisplayName"].HeaderText = "Кто хочет обнять";
        }

        private void coinsLabel_TextChanged(object sender, EventArgs e)
        {
            coinsLabel.Left = ClientSize.Width - coinsLabel.Width - 10;
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
    }
}
