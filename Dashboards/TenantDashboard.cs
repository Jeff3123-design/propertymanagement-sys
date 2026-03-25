using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Helpers;

namespace PropertyManagementSystem.Forms
{
    public partial class TenantDashboard : Form
    {
        private DatabaseHelper db;
        private AuthHelper auth;

        public TenantDashboard()
        {
            db = new DatabaseHelper();
            auth = new AuthHelper(db);
            InitializeComponent();
            LoadTenantData();
        }

        private void InitializeComponent()
        {
            this.Text = "Property Management System - Tenant Portal";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(243, 242, 241);

            // Simple Menu for Tenants
            MenuStrip menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.FromArgb(0, 120, 212);
            menuStrip.ForeColor = Color.White;

            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            ToolStripMenuItem logoutItem = new ToolStripMenuItem("Logout");
            logoutItem.Click += (s, e) => {
                auth.Logout();
                LoginForm login = new LoginForm();
                login.Show();
                this.Close();
            };
            fileMenu.DropDownItems.Add(logoutItem);

            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Help");
            ToolStripMenuItem aboutItem = new ToolStripMenuItem("About");
            aboutItem.Click += (s, e) => ShowAbout();
            helpMenu.DropDownItems.Add(aboutItem);

            menuStrip.Items.AddRange(new ToolStripMenuItem[] { fileMenu, helpMenu });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Main Content
            TabControl tabControl = new TabControl { Dock = DockStyle.Fill, Padding = new Point(10, 5) };

            TabPage dashboardPage = new TabPage("My Dashboard");
            CreateTenantDashboard(dashboardPage);
            tabControl.TabPages.Add(dashboardPage);

            TabPage paymentsPage = new TabPage("My Payments");
            CreateTenantPaymentsView(paymentsPage);
            tabControl.TabPages.Add(paymentsPage);

            TabPage profilePage = new TabPage("My Profile");
            CreateProfileView(profilePage);
            tabControl.TabPages.Add(profilePage);

            this.Controls.Add(tabControl);
        }

        private void CreateTenantDashboard(TabPage page)
        {
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(243, 242, 241)
            };

            // Get tenant information
            int tenantId = GetCurrentTenantId();
            var leaseInfo = GetTenantLeaseInfo(tenantId);

            // Property Card
            Panel propertyCard = CreateInfoCard("My Property", leaseInfo.PropertyAddress ?? "No active lease",
                Color.FromArgb(0, 120, 212), 0, 0);

            // Rent Card
            Panel rentCard = CreateInfoCard("Monthly Rent", $"${leaseInfo.MonthlyRent:N2}",
                Color.FromArgb(16, 124, 16), 1, 0);

            // Lease Status Card
            Panel leaseCard = CreateInfoCard("Lease Status", leaseInfo.LeaseStatus ?? "No active lease",
                Color.FromArgb(0, 120, 212), 0, 1);

            // Next Payment Card
            Panel paymentCard = CreateInfoCard("Next Payment Due", leaseInfo.NextPaymentDue ?? "N/A",
                Color.FromArgb(16, 124, 16), 1, 1);

            layout.Controls.Add(propertyCard, 0, 0);
            layout.Controls.Add(rentCard, 1, 0);
            layout.Controls.Add(leaseCard, 0, 1);
            layout.Controls.Add(paymentCard, 1, 1);

            // Payment History
            DataGridView paymentHistory = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                BackgroundColor = Color.White,
                Margin = new Padding(5)
            };
            paymentHistory.DataSource = GetPaymentHistory(tenantId);

            GroupBox historyBox = new GroupBox { Text = " Payment History ", Dock = DockStyle.Fill, Margin = new Padding(5) };
            historyBox.Controls.Add(paymentHistory);
            layout.Controls.Add(historyBox, 0, 2);
            layout.SetColumnSpan(historyBox, 2);

            page.Controls.Add(layout);
        }

        private Panel CreateInfoCard(string title, string value, Color color, int col, int row)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(5),
                Height = 120
            };

            Label lblTitle = new Label
            {
                Text = title,
                Location = new Point(15, 15),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = color
            };

            Label lblValue = new Label
            {
                Text = value,
                Location = new Point(15, 50),
                Size = new Size(250, 40),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 49, 48)
            };

            card.Controls.AddRange(new Control[] { lblTitle, lblValue });
            return card;
        }

        private void CreateTenantPaymentsView(TabPage page)
        {
            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                BackgroundColor = Color.White
            };

            int tenantId = GetCurrentTenantId();
            dgv.DataSource = GetPaymentHistory(tenantId);

            // Pay Now Button
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(243, 242, 241),
                Padding = new Padding(10)
            };

            Button btnPayNow = new Button
            {
                Text = "Make a Payment",
                Location = new Point(10, 10),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(16, 124, 16),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPayNow.FlatAppearance.BorderSize = 0;
            btnPayNow.Click += (s, e) => {
                MessageBox.Show("Payment gateway integration coming soon!", "Payment",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            buttonPanel.Controls.Add(btnPayNow);

            page.Controls.Add(dgv);
            page.Controls.Add(buttonPanel);
        }

        private void CreateProfileView(TabPage page)
        {
            Panel profilePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                AutoScroll = true
            };

            int tenantId = GetCurrentTenantId();
            var tenant = GetTenantDetails(tenantId);

            // Profile Picture Placeholder
            Panel picPanel = new Panel
            {
                Size = new Size(120, 120),
                Location = new Point(50, 20),
                BackColor = Color.FromArgb(0, 120, 212),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label picLabel = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 48),
                ForeColor = Color.White,
                Location = new Point(30, 25),
                Size = new Size(60, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            picPanel.Controls.Add(picLabel);

            // Profile Info
            int yPos = 20;
            int xPos = 200;
            int labelWidth = 120;
            int controlWidth = 300;

            AddProfileField(profilePanel, "Full Name:", tenant.FullName, xPos, ref yPos, labelWidth, controlWidth);
            AddProfileField(profilePanel, "Email:", tenant.Email, xPos, ref yPos, labelWidth, controlWidth);
            AddProfileField(profilePanel, "Phone:", tenant.Phone, xPos, ref yPos, labelWidth, controlWidth);
            AddProfileField(profilePanel, "Property:", tenant.PropertyAddress, xPos, ref yPos, labelWidth, controlWidth);
            AddProfileField(profilePanel, "Lease Start:", tenant.LeaseStart, xPos, ref yPos, labelWidth, controlWidth);
            AddProfileField(profilePanel, "Lease End:", tenant.LeaseEnd, xPos, ref yPos, labelWidth, controlWidth);
            AddProfileField(profilePanel, "Monthly Rent:", $"${tenant.MonthlyRent:N2}", xPos, ref yPos, labelWidth, controlWidth);

            Button btnEditProfile = new Button
            {
                Text = "Edit Profile",
                Location = new Point(xPos, yPos + 20),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnEditProfile.FlatAppearance.BorderSize = 0;
            btnEditProfile.Click += (s, e) => {
                MessageBox.Show("Profile edit feature coming soon!", "Profile",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            profilePanel.Controls.Add(picPanel);
            profilePanel.Controls.Add(btnEditProfile);

            page.Controls.Add(profilePanel);
        }

        private void AddProfileField(Panel panel, string label, string value, int x, ref int y, int labelWidth, int controlWidth)
        {
            Label lbl = new Label
            {
                Text = label,
                Location = new Point(x, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 49, 48)
            };

            Label lblValue = new Label
            {
                Text = value,
                Location = new Point(x + labelWidth + 10, y),
                Size = new Size(controlWidth, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(50, 49, 48)
            };

            panel.Controls.Add(lbl);
            panel.Controls.Add(lblValue);
            y += 35;
        }

        private int GetCurrentTenantId()
        {
            // Get tenant ID from logged in user
            // For demo, return 1
            return 1;
        }

        private (string PropertyAddress, decimal MonthlyRent, string LeaseStatus, string NextPaymentDue) GetTenantLeaseInfo(int tenantId)
        {
            // Query database for tenant lease info
            return ("123 Main St, Springfield", 1200, "Active - Ends Dec 2025", "April 1, 2024");
        }

        private DataTable GetPaymentHistory(int tenantId)
        {
            return db.ExecuteQuery("SELECT PaymentDate, Amount, PaymentMethod, Status FROM Payments WHERE LeaseId IN (SELECT LeaseId FROM Leases WHERE TenantId = @id)");
        }

        private (string FullName, string Email, string Phone, string PropertyAddress, string LeaseStart, string LeaseEnd, decimal MonthlyRent) GetTenantDetails(int tenantId)
        {
            return ("John Doe", "john@example.com", "555-1234", "123 Main St", "Jan 1, 2024", "Dec 31, 2024", 1200);
        }

        private void LoadTenantData() { }
        private void ShowAbout() { MessageBox.Show("Property Management System v1.0\nTenant Portal\n© 2024", "About"); }
    }
}