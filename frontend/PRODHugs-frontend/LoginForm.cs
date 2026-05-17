namespace PRODHugs_frontend
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        async void LoginButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(usernameInput.Text) ||
                string.IsNullOrEmpty(passwordInput.Text))
            {
                MessageBox.Show("Логин или пароль пусты.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LoadingForm loading = new(this, usernameInput.Text, passwordInput.Text);
            loading.Show();
        }
    }
}
