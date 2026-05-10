using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PRODHugs_frontend
{
    public partial class DashboardForm : Form
    {
        User user;

        public DashboardForm(User user)
        {
            this.user = user;
            InitializeComponent();
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            helloLabel.Text = $"Привет, {user.DisplayName}";
        }
    }
}
