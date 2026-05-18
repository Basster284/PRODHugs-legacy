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
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            coinsLabel = new Label();
            helloLabel = new Label();
            HugHistoryGrid = new DataGridView();
            label1 = new Label();
            panel1 = new Panel();
            pictureBox4 = new PictureBox();
            pictureBox3 = new PictureBox();
            UsersButton = new PictureBox();
            pictureBox1 = new PictureBox();
            HugsInboxGrid = new DataGridView();
            label2 = new Label();
            label3 = new Label();
            panel2 = new Panel();
            DailyRewardButton = new Button();
            label4 = new Label();
            panel3 = new Panel();
            RankLabel = new Label();
            label11 = new Label();
            AcceptedHugsLabel = new Label();
            label9 = new Label();
            InitiatedHugsLabel = new Label();
            label7 = new Label();
            TotalHugsLabel = new Label();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)HugHistoryGrid).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)UsersButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)HugsInboxGrid).BeginInit();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
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
            coinsLabel.RightToLeft = RightToLeft.No;
            coinsLabel.Size = new Size(22, 25);
            coinsLabel.TabIndex = 0;
            coinsLabel.Text = "0";
            coinsLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // helloLabel
            // 
            helloLabel.AutoSize = true;
            helloLabel.Font = new Font("Segoe UI", 18F);
            helloLabel.ForeColor = Color.White;
            helloLabel.Location = new Point(56, 9);
            helloLabel.Name = "helloLabel";
            helloLabel.Size = new Size(137, 32);
            helloLabel.TabIndex = 1;
            helloLabel.Text = "Привет, %s";
            // 
            // HugHistoryGrid
            // 
            HugHistoryGrid.AllowUserToAddRows = false;
            HugHistoryGrid.AllowUserToDeleteRows = false;
            HugHistoryGrid.AllowUserToResizeColumns = false;
            HugHistoryGrid.AllowUserToResizeRows = false;
            HugHistoryGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            HugHistoryGrid.BackgroundColor = Color.FromArgb(0, 51, 36);
            HugHistoryGrid.BorderStyle = BorderStyle.None;
            HugHistoryGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.FromArgb(0, 51, 36);
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = Color.White;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            HugHistoryGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            HugHistoryGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.FromArgb(0, 64, 46);
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = Color.White;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            HugHistoryGrid.DefaultCellStyle = dataGridViewCellStyle6;
            HugHistoryGrid.EnableHeadersVisualStyles = false;
            HugHistoryGrid.GridColor = Color.FromArgb(0, 64, 46);
            HugHistoryGrid.Location = new Point(548, 88);
            HugHistoryGrid.Name = "HugHistoryGrid";
            HugHistoryGrid.ReadOnly = true;
            HugHistoryGrid.RowHeadersVisible = false;
            HugHistoryGrid.Size = new Size(240, 350);
            HugHistoryGrid.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14F);
            label1.ForeColor = Color.White;
            label1.Location = new Point(549, 60);
            label1.Name = "label1";
            label1.RightToLeft = RightToLeft.No;
            label1.Size = new Size(239, 25);
            label1.TabIndex = 3;
            label1.Text = "Твоя история обнимашек:";
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(0, 51, 36);
            panel1.Controls.Add(pictureBox4);
            panel1.Controls.Add(pictureBox3);
            panel1.Controls.Add(UsersButton);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(50, 450);
            panel1.TabIndex = 8;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = Properties.Resources.LeaderboardIcon;
            pictureBox4.Location = new Point(12, 105);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(25, 25);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 3;
            pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.FeedIcon;
            pictureBox3.Location = new Point(12, 74);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(25, 25);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 2;
            pictureBox3.TabStop = false;
            // 
            // UsersButton
            // 
            UsersButton.Image = Properties.Resources.UsersIcon;
            UsersButton.Location = new Point(12, 43);
            UsersButton.Name = "UsersButton";
            UsersButton.Size = new Size(25, 25);
            UsersButton.SizeMode = PictureBoxSizeMode.StretchImage;
            UsersButton.TabIndex = 1;
            UsersButton.TabStop = false;
            UsersButton.Click += UsersButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.DashboardIconOpened;
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(25, 25);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // HugsInboxGrid
            // 
            HugsInboxGrid.AllowUserToAddRows = false;
            HugsInboxGrid.AllowUserToDeleteRows = false;
            HugsInboxGrid.AllowUserToResizeColumns = false;
            HugsInboxGrid.AllowUserToResizeRows = false;
            HugsInboxGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            HugsInboxGrid.BackgroundColor = Color.FromArgb(0, 51, 36);
            HugsInboxGrid.BorderStyle = BorderStyle.None;
            HugsInboxGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = Color.FromArgb(0, 51, 36);
            dataGridViewCellStyle7.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle7.ForeColor = Color.White;
            dataGridViewCellStyle7.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = DataGridViewTriState.True;
            HugsInboxGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            HugsInboxGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = Color.FromArgb(0, 64, 46);
            dataGridViewCellStyle8.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle8.ForeColor = Color.White;
            dataGridViewCellStyle8.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = DataGridViewTriState.False;
            HugsInboxGrid.DefaultCellStyle = dataGridViewCellStyle8;
            HugsInboxGrid.EnableHeadersVisualStyles = false;
            HugsInboxGrid.GridColor = Color.FromArgb(0, 64, 46);
            HugsInboxGrid.Location = new Point(56, 88);
            HugsInboxGrid.Name = "HugsInboxGrid";
            HugsInboxGrid.ReadOnly = true;
            HugsInboxGrid.RowHeadersVisible = false;
            HugsInboxGrid.Size = new Size(211, 350);
            HugsInboxGrid.TabIndex = 9;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14F);
            label2.ForeColor = Color.White;
            label2.Location = new Point(56, 60);
            label2.Name = "label2";
            label2.RightToLeft = RightToLeft.No;
            label2.Size = new Size(211, 25);
            label2.TabIndex = 10;
            label2.Text = "Входящие обнимашки:";
            // 
            // label3
            // 
            label3.Font = new Font("Segoe UI", 14F);
            label3.ForeColor = Color.White;
            label3.Location = new Point(3, 0);
            label3.Name = "label3";
            label3.RightToLeft = RightToLeft.No;
            label3.Size = new Size(263, 49);
            label3.TabIndex = 11;
            label3.Text = "Ежедневная награда";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(0, 51, 36);
            panel2.Controls.Add(DailyRewardButton);
            panel2.Controls.Add(label3);
            panel2.Location = new Point(273, 338);
            panel2.Name = "panel2";
            panel2.Size = new Size(269, 100);
            panel2.TabIndex = 12;
            // 
            // DailyRewardButton
            // 
            DailyRewardButton.BackColor = Color.Yellow;
            DailyRewardButton.BackgroundImageLayout = ImageLayout.None;
            DailyRewardButton.FlatStyle = FlatStyle.Flat;
            DailyRewardButton.Font = new Font("Segoe UI", 9F);
            DailyRewardButton.ForeColor = Color.Black;
            DailyRewardButton.Location = new Point(3, 52);
            DailyRewardButton.Name = "DailyRewardButton";
            DailyRewardButton.Size = new Size(263, 45);
            DailyRewardButton.TabIndex = 0;
            DailyRewardButton.Text = "Забрать";
            DailyRewardButton.UseVisualStyleBackColor = false;
            DailyRewardButton.Click += DailyRewardButton_click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 14F);
            label4.ForeColor = Color.White;
            label4.Location = new Point(273, 60);
            label4.Name = "label4";
            label4.RightToLeft = RightToLeft.No;
            label4.Size = new Size(112, 25);
            label4.TabIndex = 13;
            label4.Text = "Статистика:";
            // 
            // panel3
            // 
            panel3.BackColor = Color.FromArgb(0, 51, 36);
            panel3.Controls.Add(RankLabel);
            panel3.Controls.Add(label11);
            panel3.Controls.Add(AcceptedHugsLabel);
            panel3.Controls.Add(label9);
            panel3.Controls.Add(InitiatedHugsLabel);
            panel3.Controls.Add(label7);
            panel3.Controls.Add(TotalHugsLabel);
            panel3.Controls.Add(label5);
            panel3.Location = new Point(276, 88);
            panel3.Name = "panel3";
            panel3.Size = new Size(266, 244);
            panel3.TabIndex = 14;
            // 
            // RankLabel
            // 
            RankLabel.AutoSize = true;
            RankLabel.Font = new Font("Segoe UI", 14F);
            RankLabel.ForeColor = Color.Gold;
            RankLabel.Location = new Point(66, 210);
            RankLabel.Name = "RankLabel";
            RankLabel.Size = new Size(136, 25);
            RankLabel.TabIndex = 7;
            RankLabel.Text = "Нетактильный";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 14F);
            label11.ForeColor = Color.White;
            label11.Location = new Point(15, 210);
            label11.Name = "label11";
            label11.Size = new Size(55, 25);
            label11.TabIndex = 6;
            label11.Text = "Ранг:";
            // 
            // AcceptedHugsLabel
            // 
            AcceptedHugsLabel.AutoSize = true;
            AcceptedHugsLabel.Font = new Font("Segoe UI", 18F);
            AcceptedHugsLabel.ForeColor = Color.Gold;
            AcceptedHugsLabel.Location = new Point(15, 156);
            AcceptedHugsLabel.Name = "AcceptedHugsLabel";
            AcceptedHugsLabel.Size = new Size(27, 32);
            AcceptedHugsLabel.TabIndex = 5;
            AcceptedHugsLabel.Text = "0";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 14F);
            label9.ForeColor = Color.White;
            label9.Location = new Point(15, 131);
            label9.Name = "label9";
            label9.Size = new Size(92, 25);
            label9.TabIndex = 4;
            label9.Text = "Принято:";
            // 
            // InitiatedHugsLabel
            // 
            InitiatedHugsLabel.AutoSize = true;
            InitiatedHugsLabel.Font = new Font("Segoe UI", 18F);
            InitiatedHugsLabel.ForeColor = Color.Gold;
            InitiatedHugsLabel.Location = new Point(15, 99);
            InitiatedHugsLabel.Name = "InitiatedHugsLabel";
            InitiatedHugsLabel.Size = new Size(27, 32);
            InitiatedHugsLabel.TabIndex = 3;
            InitiatedHugsLabel.Text = "0";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 14F);
            label7.ForeColor = Color.White;
            label7.Location = new Point(15, 74);
            label7.Name = "label7";
            label7.Size = new Size(149, 25);
            label7.TabIndex = 2;
            label7.Text = "Инициировано:";
            // 
            // TotalHugsLabel
            // 
            TotalHugsLabel.AutoSize = true;
            TotalHugsLabel.Font = new Font("Segoe UI", 18F);
            TotalHugsLabel.ForeColor = Color.Gold;
            TotalHugsLabel.Location = new Point(15, 42);
            TotalHugsLabel.Name = "TotalHugsLabel";
            TotalHugsLabel.Size = new Size(27, 32);
            TotalHugsLabel.TabIndex = 1;
            TotalHugsLabel.Text = "0";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 14F);
            label5.ForeColor = Color.White;
            label5.Location = new Point(15, 17);
            label5.Name = "label5";
            label5.Size = new Size(170, 25);
            label5.TabIndex = 0;
            label5.Text = "Всего обнимашек:";
            // 
            // DashboardForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(0, 64, 46);
            ClientSize = new Size(800, 450);
            Controls.Add(panel3);
            Controls.Add(label4);
            Controls.Add(panel2);
            Controls.Add(label2);
            Controls.Add(HugsInboxGrid);
            Controls.Add(panel1);
            Controls.Add(label1);
            Controls.Add(HugHistoryGrid);
            Controls.Add(helloLabel);
            Controls.Add(coinsLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "DashboardForm";
            Text = "Dashboard";
            FormClosed += DashboardForm_FormClosed;
            Load += DashboardForm_Load;
            ((System.ComponentModel.ISupportInitialize)HugHistoryGrid).EndInit();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)UsersButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)HugsInboxGrid).EndInit();
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label coinsLabel;
        private Label helloLabel;
        private DataGridView HugHistoryGrid;
        private Label label1;
        private Panel panel1;
        private PictureBox pictureBox4;
        private PictureBox pictureBox3;
        private PictureBox UsersButton;
        private PictureBox pictureBox1;
        private DataGridView HugsInboxGrid;
        private Label label2;
        private Label label3;
        private Panel panel2;
        private Button DailyRewardButton;
        private Label label4;
        private Panel panel3;
        private Label label5;
        private Label TotalHugsLabel;
        private Label label7;
        private Label InitiatedHugsLabel;
        private Label AcceptedHugsLabel;
        private Label label9;
        private Label label11;
        private Label RankLabel;
    }
}