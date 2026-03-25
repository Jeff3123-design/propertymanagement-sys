using System;
using System.Drawing;
using System.Windows.Forms;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Forms;
using PropertyManagementSystem.Helpers;
using PropertyManagementSystem.Models;

namespace PropertyManagementSystem.Dashboards
{
    public partial class AdminDashboard : Form
    {
        private DatabaseHelper db;
        private AuthHelper auth;
        private System.Windows.Forms.Timer? refreshTimer;  // Made nullable
        private TabControl? mainTabControl;
        private bool isDarkMode = false;

        public AdminDashboard()
        {
            db = new DatabaseHelper();
            auth = new AuthHelper(db);
            InitializeComponent();
            LoadDashboardData();
            StartAutoRefresh();
        }

        private void InitializeComponent()
        {
            this.Text = "Property Management System - Admin Dashboard";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(243, 242, 241);
            this.MinimumSize = new Size(800, 500);

            // Create MenuStrip
            MenuStrip menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.FromArgb(0, 120, 212);
            menuStrip.ForeColor = Color.White;
            menuStrip.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // File Menu
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            fileMenu.BackColor = Color.FromArgb(0, 120, 212);
            fileMenu.ForeColor = Color.White;

            ToolStripMenuItem dashboardItem = new ToolStripMenuItem("Dashboard");
            dashboardItem.BackColor = Color.White;
            dashboardItem.ForeColor = Color.FromArgb(50, 49, 48);
            dashboardItem.Click += (s, e) => SwitchToTab(0);

            ToolStripMenuItem logoutItem = new ToolStripMenuItem("Logout");
            logoutItem.BackColor = Color.White;
            logoutItem.ForeColor = Color.FromArgb(50, 49, 48);
            logoutItem.Click += (s, e) => {
                auth.Logout();
                LoginForm login = new LoginForm();
                login.Show();
                this.Close();
            };

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.BackColor = Color.White;
            exitItem.ForeColor = Color.FromArgb(50, 49, 48);
            exitItem.Click += (s, e) => Application.Exit();

            fileMenu.DropDownItems.Add(dashboardItem);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(logoutItem);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(exitItem);

            // Management Menu
            ToolStripMenuItem manageMenu = new ToolStripMenuItem("Management");
            manageMenu.BackColor = Color.FromArgb(0, 120, 212);
            manageMenu.ForeColor = Color.White;

            ToolStripMenuItem propertiesItem = new ToolStripMenuItem("Properties");
            ToolStripMenuItem tenantsItem = new ToolStripMenuItem("Tenants");
            ToolStripMenuItem leasesItem = new ToolStripMenuItem("Leases");
            ToolStripMenuItem paymentsItem = new ToolStripMenuItem("Payments");
            ToolStripMenuItem usersItem = new ToolStripMenuItem("Users");
            ToolStripMenuItem maintenanceItem = new ToolStripMenuItem("Maintenance");

            propertiesItem.BackColor = Color.White;
            propertiesItem.ForeColor = Color.FromArgb(50, 49, 48);
            tenantsItem.BackColor = Color.White;
            tenantsItem.ForeColor = Color.FromArgb(50, 49, 48);
            leasesItem.BackColor = Color.White;
            leasesItem.ForeColor = Color.FromArgb(50, 49, 48);
            paymentsItem.BackColor = Color.White;
            paymentsItem.ForeColor = Color.FromArgb(50, 49, 48);
            usersItem.BackColor = Color.White;
            usersItem.ForeColor = Color.FromArgb(50, 49, 48);
            maintenanceItem.BackColor = Color.White;
            maintenanceItem.ForeColor = Color.FromArgb(50, 49, 48);

            propertiesItem.Click += (s, e) => SwitchToTab(1);
            tenantsItem.Click += (s, e) => SwitchToTab(2);
            leasesItem.Click += (s, e) => SwitchToTab(3);
            paymentsItem.Click += (s, e) => SwitchToTab(4);
            usersItem.Click += (s, e) => SwitchToTab(5);
            maintenanceItem.Click += (s, e) => SwitchToTab(6);

            // Create separator as ToolStripSeparator, not ToolStripMenuItem
            ToolStripSeparator separator = new ToolStripSeparator();

            manageMenu.DropDownItems.AddRange(new ToolStripItem[] {
                propertiesItem, tenantsItem, leasesItem, paymentsItem,
                separator,  // This is now correct type
                usersItem, maintenanceItem
            });

            // Reports Menu
            ToolStripMenuItem reportsMenu = new ToolStripMenuItem("Reports");
            reportsMenu.BackColor = Color.FromArgb(0, 120, 212);
            reportsMenu.ForeColor = Color.White;

            ToolStripMenuItem financialReport = new ToolStripMenuItem("Financial Report");
            ToolStripMenuItem occupancyReport = new ToolStripMenuItem("Occupancy Report");
            ToolStripMenuItem rentRoll = new ToolStripMenuItem("Rent Roll");
            ToolStripMenuItem maintenanceReport = new ToolStripMenuItem("Maintenance Report");

            financialReport.BackColor = Color.White;
            financialReport.ForeColor = Color.FromArgb(50, 49, 48);
            occupancyReport.BackColor = Color.White;
            occupancyReport.ForeColor = Color.FromArgb(50, 49, 48);
            rentRoll.BackColor = Color.White;
            rentRoll.ForeColor = Color.FromArgb(50, 49, 48);
            maintenanceReport.BackColor = Color.White;
            maintenanceReport.ForeColor = Color.FromArgb(50, 49, 48);

            financialReport.Click += (s, e) => ShowFinancialReport();
            occupancyReport.Click += (s, e) => ShowOccupancyReport();
            rentRoll.Click += (s, e) => ShowRentRoll();
            maintenanceReport.Click += (s, e) => ShowMaintenanceReport();

            reportsMenu.DropDownItems.AddRange(new ToolStripItem[] {
                financialReport, occupancyReport, rentRoll, maintenanceReport
            });

            // Settings Menu
            ToolStripMenuItem settingsMenu = new ToolStripMenuItem("Settings");
            settingsMenu.BackColor = Color.FromArgb(0, 120, 212);
            settingsMenu.ForeColor = Color.White;

            ToolStripMenuItem darkModeItem = new ToolStripMenuItem("Dark Mode");
            darkModeItem.BackColor = Color.White;
            darkModeItem.ForeColor = Color.FromArgb(50, 49, 48);
            darkModeItem.Click += (s, e) => ToggleDarkMode();

            ToolStripMenuItem backupItem = new ToolStripMenuItem("Backup Database");
            backupItem.BackColor = Color.White;
            backupItem.ForeColor = Color.FromArgb(50, 49, 48);
            backupItem.Click += (s, e) => BackupDatabase();

            ToolStripMenuItem refreshItem = new ToolStripMenuItem("Refresh Data");
            refreshItem.BackColor = Color.White;
            refreshItem.ForeColor = Color.FromArgb(50, 49, 48);
            refreshItem.Click += (s, e) => LoadDashboardData();

            settingsMenu.DropDownItems.AddRange(new ToolStripItem[] {
                darkModeItem, backupItem, refreshItem
            });

            // Help Menu
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Help");
            helpMenu.BackColor = Color.FromArgb(0, 120, 212);
            helpMenu.ForeColor = Color.White;

            ToolStripMenuItem aboutItem = new ToolStripMenuItem("About");
            aboutItem.BackColor = Color.White;
            aboutItem.ForeColor = Color.FromArgb(50, 49, 48);
            aboutItem.Click += (s, e) => ShowAbout();

            ToolStripMenuItem helpItem = new ToolStripMenuItem("Help Guide");
            helpItem.BackColor = Color.White;
            helpItem.ForeColor = Color.FromArgb(50, 49, 48);
            helpItem.Click += (s, e) => ShowHelp();

            helpMenu.DropDownItems.AddRange(new ToolStripItem[] { aboutItem, helpItem });

            menuStrip.Items.AddRange(new ToolStripItem[] {
                fileMenu, manageMenu, reportsMenu, settingsMenu, helpMenu
            });

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Main Tab Control
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Padding = new Point(10, 5),
                Font = new Font("Segoe UI", 9)
            };

            // Style the TabControl
            mainTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            mainTabControl.DrawItem += (sender, e) => {
                TabPage page = mainTabControl.TabPages[e.Index];
                Rectangle rect = e.Bounds;

                // Fill background
                if (e.Index == mainTabControl.SelectedIndex)
                {
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }
                    // Draw bottom border for active tab
                    using (Pen pen = new Pen(Color.FromArgb(0, 120, 212), 2))
                    {
                        e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 2, e.Bounds.Right, e.Bounds.Bottom - 2);
                    }
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }
                }

                // Draw text
                TextRenderer.DrawText(e.Graphics, page.Text, new Font("Segoe UI", 9, FontStyle.Bold), rect,
                    e.Index == mainTabControl.SelectedIndex ? Color.FromArgb(0, 120, 212) : Color.Gray,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            // Dashboard Tab
            TabPage dashboardPage = new TabPage(" DASHBOARD ");
            dashboardPage.BackColor = Color.FromArgb(243, 242, 241);
            CreateAdminDashboard(dashboardPage);
            mainTabControl.TabPages.Add(dashboardPage);

            // Properties Tab
            TabPage propertiesPage = new TabPage(" PROPERTIES ");
            propertiesPage.BackColor = Color.FromArgb(243, 242, 241);
            CreatePropertiesView(propertiesPage);
            mainTabControl.TabPages.Add(propertiesPage);

            // Tenants Tab
            TabPage tenantsPage = new TabPage(" TENANTS ");
            tenantsPage.BackColor = Color.FromArgb(243, 242, 241);
            CreateTenantsView(tenantsPage);
            mainTabControl.TabPages.Add(tenantsPage);

            // Leases Tab
            TabPage leasesPage = new TabPage(" LEASES ");
            leasesPage.BackColor = Color.FromArgb(243, 242, 241);
            CreateLeasesView(leasesPage);
            mainTabControl.TabPages.Add(leasesPage);

            // Payments Tab
            TabPage paymentsPage = new TabPage(" PAYMENTS ");
            paymentsPage.BackColor = Color.FromArgb(243, 242, 241);
            CreatePaymentsView(paymentsPage);
            mainTabControl.TabPages.Add(paymentsPage);

            // Users Tab
            TabPage usersPage = new TabPage(" USERS ");
            usersPage.BackColor = Color.FromArgb(243, 242, 241);
            CreateUsersView(usersPage);
            mainTabControl.TabPages.Add(usersPage);

            // Maintenance Tab
            TabPage maintenancePage = new TabPage(" MAINTENANCE ");
            maintenancePage.BackColor = Color.FromArgb(243, 242, 241);
            CreateMaintenanceView(maintenancePage);
            mainTabControl.TabPages.Add(maintenancePage);

            this.Controls.Add(mainTabControl);

            // Status Bar
            StatusStrip statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.FromArgb(0, 120, 212);
            statusStrip.ForeColor = Color.White;

            ToolStripStatusLabel statusLabel = new ToolStripStatusLabel($"Logged in as: {auth.CurrentUser?.FullName} ({auth.CurrentUser?.Role})");
            statusLabel.Spring = true;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;

            ToolStripStatusLabel dateLabel = new ToolStripStatusLabel(DateTime.Now.ToString("dddd, MMM dd, yyyy HH:mm"));
            dateLabel.TextAlign = ContentAlignment.MiddleRight;

            statusStrip.Items.Add(statusLabel);
            statusStrip.Items.Add(dateLabel);

            // Update time every second
            System.Windows.Forms.Timer timeTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            timeTimer.Tick += (s, e) => {
                dateLabel.Text = DateTime.Now.ToString("dddd, MMM dd, yyyy HH:mm");
            };
            timeTimer.Start();

            this.Controls.Add(statusStrip);
        }

        private void CreateAdminDashboard(TabPage page)
        {
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3,
                Padding = new Padding(15),
                BackColor = Color.FromArgb(243, 242, 241)
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 130));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 130));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Get statistics
            int totalProps = db.GetTotalProperties();
            int occupiedProps = db.GetOccupiedProperties();
            int totalTenants = db.GetTotalTenants();
            decimal monthlyIncome = db.GetMonthlyIncome();
            int activeLeases = 0;

            try
            {
                var activeLeasesTable = db.GetActiveLeases();
                if (activeLeasesTable != null)
                    activeLeases = activeLeasesTable.Rows?.Count ?? 0;
            }
            catch { activeLeases = 0; }

            // Calculate percentages
            int occupancyPercentage = totalProps > 0 ? (occupiedProps * 100 / totalProps) : 0;

            // Statistics Cards
            AddStatCard(layout, "Total Properties", totalProps.ToString(), "Properties", Color.FromArgb(0, 120, 212), "🏢", 0, 0);
            AddStatCard(layout, "Occupancy Rate", $"{occupancyPercentage}%", $"{occupiedProps}/{totalProps} rented", Color.FromArgb(0, 120, 212), "📊", 1, 0);
            AddStatCard(layout, "Active Tenants", totalTenants.ToString(), "Current tenants", Color.FromArgb(16, 124, 16), "👥", 2, 0);
            AddStatCard(layout, "Monthly Revenue", $"${monthlyIncome:N2}", "Total income", Color.FromArgb(16, 124, 16), "💰", 0, 1);
            AddStatCard(layout, "Active Leases", activeLeases.ToString(), "Current leases", Color.FromArgb(0, 120, 212), "📄", 1, 1);
            AddStatCard(layout, "System Users", "5", "Admin + Staff", Color.FromArgb(0, 120, 212), "👤", 2, 1);

            // Recent Activity Panel
            Panel activityPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.None
            };

            Label lblActivityTitle = new Label
            {
                Text = "📋 RECENT ACTIVITY",
                Location = new Point(15, 10),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 212)
            };

            ListBox recentActivity = new ListBox
            {
                Location = new Point(15, 45),
                Size = new Size(activityPanel.Width - 30, activityPanel.Height - 60),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                IntegralHeight = false
            };

            recentActivity.Items.Add($"✅ Welcome Admin! Last login: {DateTime.Now:MM/dd/yyyy HH:mm}");
            recentActivity.Items.Add($"💰 Total revenue this month: ${monthlyIncome:N2}");
            recentActivity.Items.Add($"🏠 Active properties: {occupiedProps}/{totalProps} ({occupancyPercentage}% occupancy)");
            recentActivity.Items.Add($"👥 Total tenants: {totalTenants}");
            recentActivity.Items.Add($"📄 Active leases: {activeLeases}");
            recentActivity.Items.Add($"🔧 Pending maintenance: 3 requests");
            recentActivity.Items.Add($"📅 Next rent collection: April 1, 2024");
            recentActivity.Items.Add($"⭐ System status: Online and operational");

            activityPanel.Controls.Add(lblActivityTitle);
            activityPanel.Controls.Add(recentActivity);

            activityPanel.Resize += (s, e) => {
                recentActivity.Size = new Size(activityPanel.Width - 30, activityPanel.Height - 60);
            };

            layout.Controls.Add(activityPanel, 0, 2);
            layout.SetColumnSpan(activityPanel, 3);

            page.Controls.Add(layout);
        }

        private void AddStatCard(TableLayoutPanel layout, string title, string value, string subtitle, Color color, string icon, int col, int row)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.None
            };

            // Add shadow effect
            card.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle,
                    Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid);
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Location = new Point(15, 10),
                Size = new Size(40, 40),
                Font = new Font("Segoe UI", 24),
                ForeColor = color,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblTitle = new Label
            {
                Text = title,
                Location = new Point(65, 15),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = color
            };

            Label lblValue = new Label
            {
                Text = value,
                Location = new Point(15, 55),
                Size = new Size(250, 40),
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 49, 48)
            };

            Label lblSubtitle = new Label
            {
                Text = subtitle,
                Location = new Point(15, 100),
                Size = new Size(250, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };

            card.Controls.AddRange(new Control[] { lblIcon, lblTitle, lblValue, lblSubtitle });
            layout.Controls.Add(card, col, row);
        }

        private void CreatePropertiesView(TabPage page)
        {
            DataGridView dgv = CreateStyledDataGrid();
            dgv.Name = "dgvProperties";
            dgv.DataSource = db.GetAllProperties();

            // Add search box
            Panel searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(243, 242, 241),
                Padding = new Padding(10)
            };

            Label lblSearch = new Label
            {
                Text = "Search:",
                Location = new Point(10, 12),
                Size = new Size(50, 25),
                Font = new Font("Segoe UI", 10)
            };

            TextBox txtSearch = new TextBox
            {
                Location = new Point(65, 10),
                Width = 250,
                Height = 30,
                Font = new Font("Segoe UI", 10)
            };

            Button btnSearch = new Button
            {
                Text = "🔍",
                Location = new Point(325, 10),
                Size = new Size(40, 30),
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;

            searchPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnSearch });

            Panel buttonPanel = CreateButtonPanel("Add Property", Color.FromArgb(0, 120, 212), () => {
                PropertyForm form = new PropertyForm(db, auth);
                if (form.ShowDialog() == DialogResult.OK)
                    dgv.DataSource = db.GetAllProperties();
            });

            page.Controls.Add(dgv);
            page.Controls.Add(buttonPanel);
            page.Controls.Add(searchPanel);
        }

        private void CreateTenantsView(TabPage page)
        {
            DataGridView dgv = CreateStyledDataGrid();
            dgv.Name = "dgvTenants";
            dgv.DataSource = db.GetAllTenants();

            Panel buttonPanel = CreateButtonPanel("Add Tenant", Color.FromArgb(16, 124, 16), () => {
                TenantForm form = new TenantForm(db, auth);
                if (form.ShowDialog() == DialogResult.OK)
                    dgv.DataSource = db.GetAllTenants();
            });

            page.Controls.Add(dgv);
            page.Controls.Add(buttonPanel);
        }

        private void CreateLeasesView(TabPage page)
        {
            DataGridView dgv = CreateStyledDataGrid();
            dgv.Name = "dgvLeases";
            dgv.DataSource = db.GetAllLeases();

            Panel buttonPanel = CreateButtonPanel("Create Lease", Color.FromArgb(0, 120, 212), () => {
                LeaseForm form = new LeaseForm(db, auth);
                if (form.ShowDialog() == DialogResult.OK)
                    dgv.DataSource = db.GetAllLeases();
            });

            page.Controls.Add(dgv);
            page.Controls.Add(buttonPanel);
        }

        private void CreatePaymentsView(TabPage page)
        {
            DataGridView dgv = CreateStyledDataGrid();
            dgv.Name = "dgvPayments";
            dgv.DataSource = db.GetAllPayments();

            Panel buttonPanel = CreateButtonPanel("Record Payment", Color.FromArgb(16, 124, 16), () => {
                PaymentForm form = new PaymentForm(db, auth);
                if (form.ShowDialog() == DialogResult.OK)
                    dgv.DataSource = db.GetAllPayments();
            });

            page.Controls.Add(dgv);
            page.Controls.Add(buttonPanel);
        }

        private void CreateUsersView(TabPage page)
        {
            DataGridView dgv = CreateStyledDataGrid();
            dgv.Name = "dgvUsers";
            try
            {
                dgv.DataSource = db.ExecuteQuery("SELECT UserId, Username, Email, FullName, Role, IsActive, CreatedDate FROM Users");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading users: {ex.Message}");
            }

            Panel buttonPanel = CreateButtonPanel("Add User", Color.FromArgb(0, 120, 212), () => {
                MessageBox.Show("User management feature coming soon!\n\nYou'll be able to add, edit, and manage user accounts.",
                    "User Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });

            page.Controls.Add(dgv);
            page.Controls.Add(buttonPanel);
        }

        private void CreateMaintenanceView(TabPage page)
        {
            DataGridView dgv = CreateStyledDataGrid();
            dgv.Name = "dgvMaintenance";

            // Create columns for maintenance requests
            dgv.Columns.Clear();
            dgv.Columns.Add("RequestId", "ID");
            dgv.Columns.Add("Property", "Property");
            dgv.Columns.Add("Tenant", "Tenant");
            dgv.Columns.Add("Issue", "Issue");
            dgv.Columns.Add("Priority", "Priority");
            dgv.Columns.Add("Status", "Status");
            dgv.Columns.Add("Date", "Date Reported");

            // Sample data
            dgv.Rows.Add("1", "123 Main St", "John Doe", "Leaking faucet", "Medium", "In Progress", "2024-03-20");
            dgv.Rows.Add("2", "456 Oak Ave", "Sarah Smith", "AC not working", "High", "Pending", "2024-03-22");
            dgv.Rows.Add("3", "789 Pine St", "Mike Johnson", "Broken window", "Low", "Completed", "2024-03-18");
            dgv.Rows.Add("4", "321 Elm St", "Emily Brown", "Electrical issue", "High", "Pending", "2024-03-23");

            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(243, 242, 241)
            };

            Button btnNewRequest = new Button
            {
                Text = "New Request",
                Location = new Point(10, 8),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnNewRequest.FlatAppearance.BorderSize = 0;
            btnNewRequest.Click += (s, e) => {
                MessageBox.Show("Maintenance request form coming soon!\n\nYou'll be able to create and manage maintenance requests.",
                    "Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(140, 8),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(161, 159, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;

            buttonPanel.Controls.AddRange(new Control[] { btnNewRequest, btnRefresh });

            page.Controls.Add(dgv);
            page.Controls.Add(buttonPanel);
        }

        private DataGridView CreateStyledDataGrid()
        {
            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = Color.FromArgb(225, 223, 221),
                Font = new Font("Segoe UI", 9),
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 212);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 35;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dgv.RowTemplate.Height = 30;

            return dgv;
        }

        private Panel CreateButtonPanel(string buttonText, Color buttonColor, Action onClick)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(243, 242, 241)
            };

            Button btn = new Button
            {
                Text = buttonText,
                Location = new Point(10, 10),
                Size = new Size(130, 38),
                BackColor = buttonColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = buttonColor == Color.FromArgb(0, 120, 212) ?
                Color.FromArgb(0, 100, 180) : Color.FromArgb(13, 100, 13);
            btn.Click += (s, e) => onClick?.Invoke();

            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(150, 10),
                Size = new Size(100, 38),
                BackColor = Color.FromArgb(161, 159, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => {
                // Refresh current view - find the DataGridView in the parent
                Control parent = panel.Parent;
                if (parent != null)
                {
                    foreach (Control ctrl in parent.Controls)
                    {
                        if (ctrl is DataGridView dgv && dgv.Name.StartsWith("dgv"))
                        {
                            if (dgv.Name == "dgvProperties")
                                dgv.DataSource = db.GetAllProperties();
                            else if (dgv.Name == "dgvTenants")
                                dgv.DataSource = db.GetAllTenants();
                            else if (dgv.Name == "dgvLeases")
                                dgv.DataSource = db.GetAllLeases();
                            else if (dgv.Name == "dgvPayments")
                                dgv.DataSource = db.GetAllPayments();
                        }
                    }
                }
            };

            panel.Controls.Add(btn);
            panel.Controls.Add(btnRefresh);

            return panel;
        }

        private void SwitchToTab(int tabIndex)
        {
            if (mainTabControl != null && tabIndex < mainTabControl.TabPages.Count)
            {
                mainTabControl.SelectedIndex = tabIndex;
            }
        }

        private void LoadDashboardData()
        {
            // Refresh all dashboard data
            if (mainTabControl != null && mainTabControl.TabPages.Count > 0)
            {
                // Refresh dashboard tab
                var dashboardPage = mainTabControl.TabPages[0];
                dashboardPage.Controls.Clear();
                CreateAdminDashboard(dashboardPage);
            }
        }

        private void StartAutoRefresh()
        {
            refreshTimer = new System.Windows.Forms.Timer { Interval = 30000 }; // 30 seconds
            refreshTimer.Tick += (s, e) => LoadDashboardData();
            refreshTimer.Start();
        }

        private void ShowFinancialReport()
        {
            MessageBox.Show("Financial Report - Coming soon!\n\nThis report will show:\n• Monthly income breakdown\n• Expense tracking\n• Profit/Loss statements\n• Year-over-year comparisons",
                "Financial Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowOccupancyReport()
        {
            MessageBox.Show("Occupancy Report - Coming soon!\n\nThis report will show:\n• Current occupancy rates\n• Historical occupancy trends\n• Vacancy analysis\n• Projected occupancy",
                "Occupancy Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowRentRoll()
        {
            MessageBox.Show("Rent Roll Report - Coming soon!\n\nThis report will show:\n• All active leases\n• Monthly rent amounts\n• Payment status\n• Upcoming renewals",
                "Rent Roll", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowMaintenanceReport()
        {
            MessageBox.Show("Maintenance Report - Coming soon!\n\nThis report will show:\n• Open maintenance requests\n• Completion times\n• Cost analysis\n• Recurring issues",
                "Maintenance Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ToggleDarkMode()
        {
            isDarkMode = !isDarkMode;
            Color bg = isDarkMode ? Color.FromArgb(30, 30, 30) : Color.FromArgb(243, 242, 241);
            Color fg = isDarkMode ? Color.White : Color.Black;

            this.BackColor = bg;
            foreach (Control control in this.Controls)
            {
                ApplyTheme(control, bg, fg);
            }
        }

        private void ApplyTheme(Control control, Color bg, Color fg)
        {
            if (control is Panel || control is TabPage)
            {
                control.BackColor = bg;
            }
            if (control is Label && control != null)
            {
                control.ForeColor = fg;
            }
            foreach (Control child in control.Controls)
            {
                ApplyTheme(child, bg, fg);
            }
        }

        private void BackupDatabase()
        {
            try
            {
                string backupPath = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                System.IO.File.Copy("PropertyManagement.db", backupPath, true);
                MessageBox.Show($"Database backup created successfully!\n\nLocation: {backupPath}",
                    "Backup Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup failed: {ex.Message}", "Backup Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "Property Management System\n\n" +
                "Version 1.0.0\n\n" +
                "© 2024 Property Management System\n\n" +
                "A comprehensive solution for managing properties,\n" +
                "tenants, leases, and payments efficiently.\n\n" +
                $"Logged in as: {auth.CurrentUser?.FullName} ({auth.CurrentUser?.Role})\n\n" +
                "Built with C# and .NET 8.0",
                "About Property Management System",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ShowHelp()
        {
            MessageBox.Show(
                "Help Guide - Quick Tips\n\n" +
                "• Use the tabs to navigate between different sections\n" +
                "• Click 'Add' buttons to create new records\n" +
                "• Use the search box to filter data\n" +
                "• Double-click any record to view/edit details\n" +
                "• Generate reports from the Reports menu\n" +
                "• Use Dark Mode for comfortable night viewing\n\n" +
                "For more help, contact your system administrator.",
                "Help Guide",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}