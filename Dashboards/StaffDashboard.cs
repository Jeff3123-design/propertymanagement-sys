using System;
using System.Drawing;
using System.Windows.Forms;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Helpers;

namespace PropertyManagementSystem.Forms
{
    public partial class StaffDashboard : Form
    {
        private DatabaseHelper db;
        private AuthHelper auth;

        public StaffDashboard()
        {
            db = new DatabaseHelper();
            auth = new AuthHelper(db);
            InitializeComponent();
            LoadDashboardData();
        }

        private void InitializeComponent()
        {
            this.Text = "Property Management System - Staff Dashboard";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(243, 242, 241);

            // MenuStrip (Limited options)
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

            // Management Menu (No User Management)
            ToolStripMenuItem manageMenu = new ToolStripMenuItem("Management");
            ToolStripMenuItem propertiesItem = new ToolStripMenuItem("Properties");
            ToolStripMenuItem tenantsItem = new ToolStripMenuItem("Tenants");
            ToolStripMenuItem leasesItem = new ToolStripMenuItem("Leases");
            ToolStripMenuItem paymentsItem = new ToolStripMenuItem("Payments");

            propertiesItem.Click += (s, e) => ShowProperties();
            tenantsItem.Click += (s, e) => ShowTenants();
            leasesItem.Click += (s, e) => ShowLeases();
            paymentsItem.Click += (s, e) => ShowPayments();

            manageMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                propertiesItem, tenantsItem, leasesItem, paymentsItem
            });

            // Reports Menu
            ToolStripMenuItem reportsMenu = new ToolStripMenuItem("Reports");
            ToolStripMenuItem rentRoll = new ToolStripMenuItem("Rent Roll");
            rentRoll.Click += (s, e) => ShowRentRoll();
            reportsMenu.DropDownItems.Add(rentRoll);

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

            // Dashboard Tab
            TabPage dashboardPage = new TabPage("Dashboard");
            CreateStaffDashboard(dashboardPage);
            tabControl.TabPages.Add(dashboardPage);

            // Properties Tab
            TabPage propertiesPage = new TabPage("Properties");
            CreatePropertiesView(propertiesPage);
            tabControl.TabPages.Add(propertiesPage);

            // Tenants Tab
            TabPage tenantsPage = new TabPage("Tenants");
            CreateTenantsView(tenantsPage);
            tabControl.TabPages.Add(tenantsPage);

            // Leases Tab
            TabPage leasesPage = new TabPage("Leases");
            CreateLeasesView(leasesPage);
            tabControl.TabPages.Add(leasesPage);

            // Payments Tab
            TabPage paymentsPage = new TabPage("Payments");
            CreatePaymentsView(paymentsPage);
            tabControl.TabPages.Add(paymentsPage);

            this.Controls.Add(tabControl);
        }

        private void CreateStaffDashboard(TabPage page)
        {
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(15),
                BackColor = Color.FromArgb(243, 242, 241)
            };

            int totalProps = db.GetTotalProperties();
            int occupiedProps = db.GetOccupiedProperties();
            int totalTenants = db.GetTotalTenants();
            decimal monthlyIncome = db.GetMonthlyIncome();

            AddStatCard(layout, "Total Properties", totalProps.ToString(), "Properties", Color.FromArgb(0, 120, 212), 0, 0);
            int occupancyRate = totalProps > 0 ? (occupiedProps * 100 / totalProps) : 0;
            AddStatCard(layout, "Occupancy Rate", $"{occupancyRate}%", $"{occupiedProps}/{totalProps} rented", Color.FromArgb(0, 120, 212), 1, 0);
            AddStatCard(layout, "Active Tenants", totalTenants.ToString(), "Current tenants", Color.FromArgb(16, 124, 16), 0, 1);
            AddStatCard(layout, "Monthly Revenue", $"${monthlyIncome:N2}", "Total income", Color.FromArgb(16, 124, 16), 1, 1);

            // Quick Actions Panel
            Panel quickActions = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(5)
            };

            Label lblQuick = new Label
            {
                Text = "Quick Actions",
                Location = new Point(15, 10),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 212)
            };

            Button btnAddProperty = CreateQuickButton("Add Property", new Point(15, 45), Color.FromArgb(0, 120, 212));
            Button btnAddTenant = CreateQuickButton("Add Tenant", new Point(15, 90), Color.FromArgb(16, 124, 16));
            Button btnRecordPayment = CreateQuickButton("Record Payment", new Point(15, 135), Color.FromArgb(0, 120, 212));

            btnAddProperty.Click += (s, e) => ShowProperties();
            btnAddTenant.Click += (s, e) => ShowTenants();
            btnRecordPayment.Click += (s, e) => ShowPayments();

            quickActions.Controls.AddRange(new Control[] { lblQuick, btnAddProperty, btnAddTenant, btnRecordPayment });
            layout.Controls.Add(quickActions, 0, 2);
            layout.SetColumnSpan(quickActions, 2);

            page.Controls.Add(layout);
        }

        private void AddStatCard(TableLayoutPanel layout, string title, string value, string subtitle, Color color, int col, int row)
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
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = color
            };

            Label lblValue = new Label
            {
                Text = value,
                Location = new Point(15, 40),
                Size = new Size(200, 35),
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 49, 48)
            };

            Label lblSubtitle = new Label
            {
                Text = subtitle,
                Location = new Point(15, 80),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };

            card.Controls.AddRange(new Control[] { lblTitle, lblValue, lblSubtitle });
            layout.Controls.Add(card, col, row);
        }

        private Button CreateQuickButton(string text, Point location, Color color)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(200, 35),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
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
        private void ShowRentRoll() { MessageBox.Show("Rent Roll Report - Coming soon!"); }
        private void ShowAbout() { MessageBox.Show("Property Management System v1.0\nStaff Dashboard\n© 2024", "About"); }
    }
}