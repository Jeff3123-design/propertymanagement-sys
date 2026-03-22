using System;
using System.Windows.Forms;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Helpers;

namespace PropertyManagementSystem.Forms
{
    public partial class MainForm : Form
    {
        private DatabaseHelper db;
        private AuthHelper auth;
        private TabControl mainTabControl;

        public MainForm(DatabaseHelper database, AuthHelper authHelper)
        {
            db = database;
            auth = authHelper;
            InitializeComponent();
            UpdateUIForUser();
        }

        private void UpdateUIForUser()
        {
            this.Text = $"Property Management System - Welcome {auth.CurrentUser.FullName} ({auth.CurrentUser.Role})";
        }

        private void InitializeComponent()
        {
            this.Text = "Property Management System";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create MenuStrip
            MenuStrip menuStrip = new MenuStrip();

            // File menu
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            ToolStripMenuItem logoutItem = new ToolStripMenuItem("Logout");
            logoutItem.Click += (s, e) => {
                auth.Logout();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            };

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => Application.Exit();

            fileMenu.DropDownItems.Add(logoutItem);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(exitItem);

            // Management menu
            ToolStripMenuItem manageMenu = new ToolStripMenuItem("Management");
            ToolStripMenuItem propertiesItem = new ToolStripMenuItem("Properties");
            ToolStripMenuItem tenantsItem = new ToolStripMenuItem("Tenants");
            ToolStripMenuItem leasesItem = new ToolStripMenuItem("Leases");
            ToolStripMenuItem paymentsItem = new ToolStripMenuItem("Payments");

            propertiesItem.Click += (s, e) => ShowPropertiesTab();
            tenantsItem.Click += (s, e) => ShowTenantsTab();
            leasesItem.Click += (s, e) => ShowLeasesTab();
            paymentsItem.Click += (s, e) => ShowPaymentsTab();

            manageMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                propertiesItem, tenantsItem, leasesItem, paymentsItem
            });

            // Reports menu
            ToolStripMenuItem reportsMenu = new ToolStripMenuItem("Reports");
            ToolStripMenuItem rentRollItem = new ToolStripMenuItem("Rent Roll");
            ToolStripMenuItem vacancyReportItem = new ToolStripMenuItem("Vacancy Report");
            rentRollItem.Click += (s, e) => ShowRentRollReport();
            vacancyReportItem.Click += (s, e) => ShowVacancyReport();
            reportsMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                rentRollItem, vacancyReportItem
            });

            // Admin menu (visible only for admin)
            if (auth.IsAdmin)
            {
                ToolStripMenuItem adminMenu = new ToolStripMenuItem("Admin");
                ToolStripMenuItem usersItem = new ToolStripMenuItem("Manage Users");
                usersItem.Click += (s, e) => ShowUserManagement();
                adminMenu.DropDownItems.Add(usersItem);
                menuStrip.Items.Add(adminMenu);
            }

            // Help menu
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Help");
            ToolStripMenuItem aboutItem = new ToolStripMenuItem("About");
            aboutItem.Click += (s, e) => ShowAbout();
            helpMenu.DropDownItems.Add(aboutItem);

            menuStrip.Items.AddRange(new ToolStripMenuItem[] {
                fileMenu, manageMenu, reportsMenu, helpMenu
            });

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Create TabControl for main content
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Padding = new System.Drawing.Point(10, 5),
                Name = "mainTabControl"
            };

            // Dashboard Tab
            TabPage dashboardPage = new TabPage("Dashboard");
            CreateDashboard(dashboardPage);
            mainTabControl.TabPages.Add(dashboardPage);

            // Properties Tab
            TabPage propertiesPage = new TabPage("Properties");
            CreatePropertiesView(propertiesPage);
            mainTabControl.TabPages.Add(propertiesPage);

            // Tenants Tab
            TabPage tenantsPage = new TabPage("Tenants");
            CreateTenantsView(tenantsPage);
            mainTabControl.TabPages.Add(tenantsPage);

            // Leases Tab
            TabPage leasesPage = new TabPage("Leases");
            CreateLeasesView(leasesPage);
            mainTabControl.TabPages.Add(leasesPage);

            // Payments Tab
            TabPage paymentsPage = new TabPage("Payments");
            CreatePaymentsView(paymentsPage);
            mainTabControl.TabPages.Add(paymentsPage);

            this.Controls.Add(mainTabControl);

            // Set dashboard as active tab
            mainTabControl.SelectedIndex = 0;
        }

        private void CreateDashboard(TabPage page)
        {
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(10)
            };

            // Summary Cards
            GroupBox statsBox = new GroupBox
            {
                Text = "Quick Statistics",
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };

            FlowLayoutPanel statsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };

            // Get real statistics from database
            int totalProps = db.GetTotalProperties();
            int occupiedProps = db.GetOccupiedProperties();
            int totalTenants = db.GetTotalTenants();
            decimal monthlyIncome = db.GetMonthlyIncome();

            int occupancyPercentage = totalProps > 0 ? (occupiedProps * 100 / totalProps) : 0;

            Label totalProperties = new Label
            {
                Text = $"Total Properties: {totalProps}",
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(5)
            };

            Label occupiedProperties = new Label
            {
                Text = $"Occupied Properties: {occupiedProps} ({occupancyPercentage}%)",
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(5)
            };

            Label totalTenantsLabel = new Label
            {
                Text = $"Total Tenants: {totalTenants}",
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(5)
            };

            Label monthlyIncomeLabel = new Label
            {
                Text = $"Monthly Income: ${monthlyIncome:N2}",
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(5)
            };

            statsPanel.Controls.AddRange(new Control[] { totalProperties, occupiedProperties, totalTenantsLabel, monthlyIncomeLabel });
            statsBox.Controls.Add(statsPanel);

            // Recent Activities
            ListBox recentActivities = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 10)
            };

            recentActivities.Items.Add($"Welcome {auth.CurrentUser.FullName}!");
            recentActivities.Items.Add($"Last login: {DateTime.Now:MM/dd/yyyy HH:mm}");
            recentActivities.Items.Add($"---");
            recentActivities.Items.Add($"Total Properties: {totalProps}");
            recentActivities.Items.Add($"Active Leases: {occupiedProps}");
            recentActivities.Items.Add($"Monthly Revenue: ${monthlyIncome:N2}");
            recentActivities.Items.Add($"Occupancy Rate: {occupancyPercentage}%");

            GroupBox activitiesBox = new GroupBox
            {
                Text = "Welcome & Summary",
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            activitiesBox.Controls.Add(recentActivities);

            layout.Controls.Add(statsBox, 0, 0);
            layout.Controls.Add(activitiesBox, 1, 0);

            // Upcoming payments
            DataGridView upcomingPayments = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            try
            {
                upcomingPayments.DataSource = db.GetUpcomingPayments();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading payments: {ex.Message}");
            }

            GroupBox paymentsBox = new GroupBox
            {
                Text = "Upcoming Rent Due",
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            paymentsBox.Controls.Add(upcomingPayments);

            layout.Controls.Add(paymentsBox, 0, 1);
            layout.SetColumnSpan(paymentsBox, 2);

            page.Controls.Add(layout);
        }

        private void CreatePropertiesView(TabPage page)
        {
            DataGridView dgvProperties = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Name = "dgvProperties"
            };

            // Load properties data
            try
            {
                dgvProperties.DataSource = db.GetAllProperties();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading properties: {ex.Message}");
            }

            // Add buttons panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(5)
            };

            Button btnAdd = new Button
            {
                Text = "Add Property",
                Location = new System.Drawing.Point(5, 5),
                Size = new System.Drawing.Size(100, 30)
            };
            btnAdd.Click += (s, e) => {
                PropertyForm propertyForm = new PropertyForm(db, auth);
                if (propertyForm.ShowDialog() == DialogResult.OK)
                {
                    dgvProperties.DataSource = db.GetAllProperties();
                }
            };

            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new System.Drawing.Point(110, 5),
                Size = new System.Drawing.Size(100, 30)
            };
            btnRefresh.Click += (s, e) => {
                dgvProperties.DataSource = db.GetAllProperties();
            };

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnRefresh });

            page.Controls.Add(dgvProperties);
            page.Controls.Add(buttonPanel);
        }

        private void CreateTenantsView(TabPage page)
        {
            DataGridView dgvTenants = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Name = "dgvTenants"
            };

            try
            {
                dgvTenants.DataSource = db.GetAllTenants();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading tenants: {ex.Message}");
            }

            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(5)
            };

            Button btnAdd = new Button
            {
                Text = "Add Tenant",
                Location = new System.Drawing.Point(5, 5),
                Size = new System.Drawing.Size(100, 30)
            };
            btnAdd.Click += (s, e) => {
                TenantForm tenantForm = new TenantForm(db, auth);
                if (tenantForm.ShowDialog() == DialogResult.OK)
                {
                    dgvTenants.DataSource = db.GetAllTenants();
                }
            };

            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new System.Drawing.Point(110, 5),
                Size = new System.Drawing.Size(100, 30)
            };
            btnRefresh.Click += (s, e) => {
                dgvTenants.DataSource = db.GetAllTenants();
            };

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnRefresh });

            page.Controls.Add(dgvTenants);
            page.Controls.Add(buttonPanel);
        }

        private void CreateLeasesView(TabPage page)
        {
            DataGridView dgvLeases = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Name = "dgvLeases"
            };

            try
            {
                dgvLeases.DataSource = db.GetAllLeases();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading leases: {ex.Message}");
            }

            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(5)
            };

            Button btnAdd = new Button
            {
                Text = "Create Lease",
                Location = new System.Drawing.Point(5, 5),
                Size = new System.Drawing.Size(100, 30)
            };
            btnAdd.Click += (s, e) => {
                LeaseForm leaseForm = new LeaseForm(db, auth);
                if (leaseForm.ShowDialog() == DialogResult.OK)
                {
                    dgvLeases.DataSource = db.GetAllLeases();
                }
            };

            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new System.Drawing.Point(110, 5),
                Size = new System.Drawing.Size(100, 30)
            };
            btnRefresh.Click += (s, e) => {
                dgvLeases.DataSource = db.GetAllLeases();
            };

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnRefresh });

            page.Controls.Add(dgvLeases);
            page.Controls.Add(buttonPanel);
        }

        private void CreatePaymentsView(TabPage page)
        {
            DataGridView dgvPayments = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Name = "dgvPayments"
            };

            try
            {
                dgvPayments.DataSource = db.GetAllPayments();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading payments: {ex.Message}");
            }

            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(5)
            };

            Button btnAdd = new Button
            {
                Text = "Record Payment",
                Location = new System.Drawing.Point(5, 5),
                Size = new System.Drawing.Size(120, 30)
            };
            btnAdd.Click += (s, e) => {
                PaymentForm paymentForm = new PaymentForm(db, auth);
                if (paymentForm.ShowDialog() == DialogResult.OK)
                {
                    dgvPayments.DataSource = db.GetAllPayments();
                }
            };

            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new System.Drawing.Point(130, 5),
                Size = new System.Drawing.Size(100, 30)
            };
            btnRefresh.Click += (s, e) => {
                dgvPayments.DataSource = db.GetAllPayments();
            };

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnRefresh });

            page.Controls.Add(dgvPayments);
            page.Controls.Add(buttonPanel);
        }

        private void ShowPropertiesTab()
        {
            if (mainTabControl != null && mainTabControl.TabPages.Count > 1)
            {
                mainTabControl.SelectedIndex = 1;
            }
        }

        private void ShowTenantsTab()
        {
            if (mainTabControl != null && mainTabControl.TabPages.Count > 2)
            {
                mainTabControl.SelectedIndex = 2;
            }
        }

        private void ShowLeasesTab()
        {
            if (mainTabControl != null && mainTabControl.TabPages.Count > 3)
            {
                mainTabControl.SelectedIndex = 3;
            }
        }

        private void ShowPaymentsTab()
        {
            if (mainTabControl != null && mainTabControl.TabPages.Count > 4)
            {
                mainTabControl.SelectedIndex = 4;
            }
        }

        private void ShowRentRollReport()
        {
            MessageBox.Show("Rent Roll Report - Coming soon!", "Report",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowVacancyReport()
        {
            MessageBox.Show("Vacancy Report - Coming soon!", "Report",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowUserManagement()
        {
            MessageBox.Show("User Management - Coming soon!", "Admin",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAbout()
        {
            MessageBox.Show("Property Management System\nVersion 1.0\n\nDeveloped for efficient property management\n\nLogged in as: " + auth.CurrentUser.FullName,
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}