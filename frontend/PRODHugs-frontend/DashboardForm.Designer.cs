namespace PRODHugs_frontend
{
    partial class DashboardForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            coinsLabel = new Label();
            helloLabel = new Label();
            SuspendLayout();
            // 
            // coinsLabel
            // 
            coinsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            coinsLabel.AutoSize = true;
            coinsLabel.Font = new Font("Segoe UI", 14F);
            coinsLabel.ForeColor = Color.Gold;
            coinsLabel.Location = new Point(766, 9);
            coinsLabel.Name = "coinsLabel";
            coinsLabel.Size = new Size(22, 25);
            coinsLabel.TabIndex = 0;
            coinsLabel.Text = "0";
            // 
            // helloLabel
            // 
            helloLabel.AutoSize = true;
            helloLabel.Font = new Font("Segoe UI", 18F);
            helloLabel.ForeColor = Color.White;
            helloLabel.Location = new Point(12, 9);
            helloLabel.Name = "helloLabel";
            helloLabel.Size = new Size(137, 32);
            helloLabel.TabIndex = 1;
            helloLabel.Text = "Привет, %s";
            // 
            // DashboardForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 64, 46);
            ClientSize = new Size(800, 450);
            Controls.Add(helloLabel);
            Controls.Add(coinsLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "DashboardForm";
            Text = "Dashboard";
            Load += DashboardForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label coinsLabel;
        private Label helloLabel;
    }
}