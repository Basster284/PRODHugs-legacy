using System.ComponentModel;

namespace PRODHugs_frontend
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(LoginForm));
            languageButton = new Button();
            label1 = new Label();
            panel1 = new Panel();
            passwordInput = new TextBox();
            loginButton = new Button();
            label3 = new Label();
            usernameInput = new TextBox();
            label2 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // languageButton
            // 
            resources.ApplyResources(languageButton, "languageButton");
            languageButton.BackColor = Color.FromArgb(0, 64, 46);
            languageButton.Cursor = Cursors.Hand;
            languageButton.FlatAppearance.BorderSize = 0;
            languageButton.ForeColor = Color.White;
            languageButton.Name = "languageButton";
            languageButton.UseVisualStyleBackColor = false;
            languageButton.Click += changeLanguage;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.White;
            label1.Name = "label1";
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.BackColor = Color.FromArgb(0, 51, 36);
            panel1.Controls.Add(passwordInput);
            panel1.Controls.Add(loginButton);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(usernameInput);
            panel1.Controls.Add(label2);
            panel1.Name = "panel1";
            // 
            // passwordInput
            // 
            resources.ApplyResources(passwordInput, "passwordInput");
            passwordInput.BackColor = Color.FromArgb(0, 64, 46);
            passwordInput.BorderStyle = BorderStyle.None;
            passwordInput.ForeColor = Color.White;
            passwordInput.Name = "passwordInput";
            passwordInput.UseSystemPasswordChar = true;
            // 
            // loginButton
            // 
            resources.ApplyResources(loginButton, "loginButton");
            loginButton.BackColor = Color.FromArgb(255, 221, 45);
            loginButton.Cursor = Cursors.Hand;
            loginButton.Name = "loginButton";
            loginButton.UseVisualStyleBackColor = false;
            loginButton.Click += loginButton_Click;
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.White;
            label3.Name = "label3";
            // 
            // usernameInput
            // 
            resources.ApplyResources(usernameInput, "usernameInput");
            usernameInput.BackColor = Color.FromArgb(0, 64, 46);
            usernameInput.BorderStyle = BorderStyle.None;
            usernameInput.ForeColor = Color.White;
            usernameInput.Name = "usernameInput";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = Color.White;
            label2.Name = "label2";
            // 
            // LoginForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 64, 46);
            Controls.Add(panel1);
            Controls.Add(label1);
            Controls.Add(languageButton);
            MaximizeBox = false;
            Name = "LoginForm";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button languageButton;
        private Label label1;
        private Panel panel1;
        private Label label2;
        private TextBox usernameInput;
        private Label label3;
        private Button loginButton;
        private TextBox passwordInput;
    }
}
