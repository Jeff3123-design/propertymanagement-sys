using System;
using System.Drawing;
using System.Windows.Forms;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Helpers;

namespace PropertyManagementSystem.Forms
{
    public partial class ManagerDashboard : Form
    {
        private DatabaseHelper db;
        private AuthHelper auth;

        public ManagerDashboard()
        {
            db = new DatabaseHelper();
            auth = new AuthHelper(db);
            InitializeComponent();
            LoadDashboardData();
        }

        private void InitializeComponent()
        {
            this.Text = "Property Management System - Manager Dashboard";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(243, 242, 241);

            // MenuStrip for Manager
            MenuStrip menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.FromArgb(0, 120, 212);
            menuStrip.ForeColor = Color.White;

            // File Menu
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            ToolStripMenuItem logoutItem = new ToolStripMenuItem("Logout");
            logoutItem.Click += (s, e) => {
                auth.Logout();
                LoginForm login = new LoginForm();
                login.Show();
                this.Close();
            };
            fileMenu.DropDownItems.Add(logoutItem);

            // Management Menu (Manager has all except user management)
            ToolStripMenuItem manageMenu = new ToolStripMenuItem("Management");
            ToolStripMenuItem propertiesItem = new ToolStripMenuItem("Properties");
            ToolStripMenuItem tenantsItem = new ToolStripMenuItem("Tenants");
            ToolStripMenuItem leasesItem = new ToolStripMenuItem("Leases");
            ToolStripMenuItem paymentsItem = new ToolStripMenuItem("Payments");
            ToolStripMenuItem maintenanceItem = new ToolStripMenuItem("Maintenance Requests");

            propertiesItem.Click += (s, e) => ShowProperties();
            tenantsItem.Click += (s, e) => ShowTenants();
            leasesItem.Click += (s, e) => ShowLeases();
            paymentsItem.Click += (s, e) => ShowPayments();
            maintenanceItem.Click += (s, e) => ShowMaintenance();

            manageMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                propertiesItem, tenantsItem, leasesItem, paymentsItem, maintenanceItem
            });

            // Reports Menu
            ToolStripMenuItem reportsMenu = new ToolStripMenuItem("Reports");
            ToolStripMenuItem occupancyReport = new ToolStripMenuItem("Occupancy Report");
            ToolStripMenuItem rentRoll = new ToolStripMenuItem("Rent Roll");
            ToolStripMenuItem maintenanceReport = new ToolStripMenuItem("Maintenance Report");

            occupancyReport.Click += (s, e) => ShowOccupancyReport();
            rentRoll.Click += (s, e) => ShowRentRoll();
            maintenanceReport.Click += (s, e) => ShowMaintenanceReport();

            reportsMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                occupancyReport, rentRoll, maintenanceReport
            });

            // Help Menu
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Help");
            ToolStripMenuItem aboutItem = new ToolStripMenuItem("About");
            aboutItem.Click += (s, e) => ShowAbout();
            helpMenu.DropDownItems.Add(aboutItem);

            menuStrip.Items.AddRange(new ToolStripMenuItem[] {
                fileMenu, manageMenu, reportsMenu, helpMenu
            });

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Main Tab Control
            TabControl tabControl = new TabControl { Dock = DockStyle.Fill, Padding = new Point(10, 5) };

            TabPage dashboardPage = new TabPage("Dashboard");
            CreateManagerDashboard(dashboardPage);
            tabControl.TabPages.Add(dashboardPage);

            TabPage propertiesPage = new TabPage("Properties");
            CreatePropertiesView(propertiesPage);
            tabControl.TabPages.Add(propertiesPage);

            TabPage tenantsPage = new TabPage("Tenants");
            CreateTenantsView(tenantsPage);
            tabControl.TabPages.Add(tenantsPage);

            TabPage leasesPage = new TabPage("Leases");
            CreateLeasesView(leasesPage);
            tabControl.TabPages.Add(leasesPage);

            TabPage paymentsPage = new TabPage("Payments");
            CreatePaymentsView(paymentsPage);
            tabControl.TabPages.Add(paymentsPage);

            TabPage maintenancePage = new TabPage("Maintenance");
            CreateMaintenanceView(maintenancePage);
            tabControl.TabPages.Add(maintenancePage);

            this.Controls.Add(tabControl);
        }

        private void CreateManagerDashboard(TabPage page)
        {
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                Padding = new Padding(15),
                BackColor = Color.FromArgb(243, 242, 241)
            };

            int totalProps = db.GetTotalProperties();
            int occupiedProps = db.GetOccupiedProperties();
            int totalTenants = db.GetTotalTenants();
            decimal monthlyIncome = db.GetMonthlyIncome();
            int pendingMaintenance = 3; // This would come from database

            AddManagerStatCard(layout, "Properties", totalProps.ToString(), "Total units", Color.FromArgb(0, 120, 212), 0, 0);
            int occupancyPercentage = totalProps > 0 ? (occupiedProps * 100 / totalProps) : 0;
            AddManagerStatCard(layout, "Occupancy", $"{occupancyPercentage}%", $"{occupiedProps} rented", Color.FromArgb(0, 120, 212), 1, 0);
            AddManagerStatCard(layout, "Tenants", totalTenants.ToString(), "Active tenants", Color.FromArgb(16, 124, 16), 2, 0);
            AddManagerStatCard(layout, "Revenue", $"${monthlyIncome:N2}", "Monthly income", Color.FromArgb(16, 124, 16), 0, 1);
            AddManagerStatCard(layout, "Maintenance", pendingMaintenance.ToString(), "Open requests", Color.FromArgb(255, 140, 0), 1, 1);
            AddManagerStatCard(layout, "Leases", "8", "Active leases", Color.FromArgb(0, 120, 212), 2, 1);

            // Recent Activity
            ListBox recentActivity = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                Height = 150
            };
            recentActivity.Items.Add($"Welcome Manager! {DateTime.Now:MM/dd/yyyy}");
            recentActivity.Items.Add($"3 maintenance requests pending");
            recentActivity.Items.Add($"2 leases expiring this month");
            recentActivity.Items.Add($"Monthly revenue target: ${monthlyIncome:N2}");

            GroupBox activityBox = new GroupBox { Text = " Recent Activity ", Dock = DockStyle.Fill, Margin = new Padding(5) };
            activityBox.Controls.Add(recentActivity);
            layout.Controls.Add(activityBox, 0, 2);
            layout.SetColumnSpan(activityBox, 3);

            page.Controls.Add(layout);
        }

        private void AddManagerStatCard(TableLayoutPanel layout, string title, string value, string subtitle, Color color, int col, int row)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(5),
                Height = 100
            };

            Label lblTitle = new Label
            {
                Text = title,
                Location = new Point(15, 10),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = color
            };

            Label lblValue = new Label
            {
                Text = value,
                Location = new Point(15, 40),
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 49, 48)
            };

            Label lblSubtitle = new Label
            {
                Text = subtitle,
                Location = new Point(15, 80),
                Size = new Size(150, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };

            card.Controls.AddRange(new Control[] { lblTitle, lblValue, lblSubtitle });
            layout.Controls.Add(card, col, row);
        }

        private void CreatePropertiesView(TabPage page)
        {
            DataGridView dgv = CreateStyledDataGrid();
            dgv.DataSource = db.GetAllProperties();

            Panel buttonPanel = CreateButtonPanel("Add Property", Color.FromArgb(0, 120, 212), () => {
                PropertyForm form = new PropertyForm(db, auth);
                if (form.ShowDialog() == DialogResult.OK)
                    dgv.DataSource = db.GetAllProperties();
            });

            page.Controls.Add(dgv);
            page.Controls.Add(buttonPanel);
        }

        private void CreateTenantsView(TabPage page)
        {
            DataGridView dgv = CreateStyledDataGrid();
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
            dgv.DataSource = db.GetAllPayments();

            Panel buttonPanel = CreateButtonPanel("Record Payment", Color.FromArgb(16, 124, 16), () => {
                PaymentForm form = new PaymentForm(db, auth);
                if (form.ShowDialog() == DialogResult.OK)
                    dgv.DataSource = db.GetAllPayments();
            });

            page.Controls.Add(dgv);
            page.Controls.Add(buttonPanel);
        }

        private void CreateMaintenanceView(TabPage page)
        {
            DataGridView dgv = CreateStyledDataGrid();
            // Load maintenance requests
            dgv.Columns.Add("RequestId", "ID");
            dgv.Columns.Add("Property", "Property");
            dgv.Columns.Add("Issue", "Issue");
            dgv.Columns.Add("Status", "Status");
            dgv.Columns.Add("Date", "Date");

            dgv.Rows.Add("1", "123 Main St", "Leaking faucet", "In Progress", "2024-03-20");
            dgv.Rows.Add("2", "456 Oak Ave", "AC not working", "Pending", "2024-03-22");
            dgv.Rows.Add("3", "789 Pine St", "Broken window", "Completed", "2024-03-18");

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
                MessageBox.Show("Maintenance request form coming soon!", "Maintenance",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            buttonPanel.Controls.Add(btnNewRequest);

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
                BorderStyle = BorderStyle.FixedSingle
            };

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 212);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.RowHeadersVisible = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            return dgv;
        }

        private Panel CreateButtonPanel(string buttonText, Color buttonColor, Action onClick)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(243, 242, 241)
            };

            Button btn = new Button
            {
                Text = buttonText,
                Location = new Point(10, 8),
                Size = new Size(130, 35),
                BackColor = buttonColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => onClick();

            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(150, 8),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(161, 159, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => {
                // Refresh logic
            };

            panel.Controls.Add(btn);
            panel.Controls.Add(btnRefresh);

            return panel;
        }

        private void LoadDashboardData() { }
        private void ShowProperties() { }
        private void ShowTenants() { }
        private void ShowLeases() { }
        private void ShowPayments() { }
        private void ShowMaintenance() { }
        private void ShowOccupancyReport() { MessageBox.Show("Occupancy Report - Coming soon!"); }
        private void ShowRentRoll() { MessageBox.Show("Rent Roll Report - Coming soon!"); }
        private void ShowMaintenanceReport() { MessageBox.Show("Maintenance Report - Coming soon!"); }
        private void ShowAbout() { MessageBox.Show("Property Management System v1.0\nManager Dashboard\n© 2024", "About"); }
    }
}