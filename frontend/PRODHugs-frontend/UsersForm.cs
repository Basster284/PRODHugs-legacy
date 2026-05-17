namespace PRODHugs_frontend
{
    public partial class UsersForm : Form
    {
        private bool _closeFlag = true;

        public UsersForm()
        {
            InitializeComponent();
        }

        private void UsersForm_Load(object sender, EventArgs e)
        {

        }

        private void UsersForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_closeFlag) Application.Exit();
        }
    }
}
