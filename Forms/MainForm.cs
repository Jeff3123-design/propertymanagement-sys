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
            this.BackColor = System.Drawing.Color.FromArgb(243, 242, 241);

            // Create MenuStrip with styling
            MenuStrip menuStrip = new MenuStrip();
            menuStrip.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
            menuStrip.ForeColor = System.Drawing.Color.White;
            menuStrip.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // File menu
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            fileMenu.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
            fileMenu.ForeColor = System.Drawing.Color.White;

            ToolStripMenuItem logoutItem = new ToolStripMenuItem("Logout");
            logoutItem.BackColor = System.Drawing.Color.White;
            logoutItem.ForeColor = System.Drawing.Color.FromArgb(50, 49, 48);
            logoutItem.Click += (s, e) => {
                auth.Logout();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            };

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.BackColor = System.Drawing.Color.White;
            exitItem.ForeColor = System.Drawing.Color.FromArgb(50, 49, 48);
            exitItem.Click += (s, e) => Application.Exit();

            fileMenu.DropDownItems.Add(logoutItem);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(exitItem);

            // Management menu
            ToolStripMenuItem manageMenu = new ToolStripMenuItem("Management");
            manageMenu.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
            manageMenu.ForeColor = System.Drawing.Color.White;

            ToolStripMenuItem propertiesItem = new ToolStripMenuItem("Properties");
            ToolStripMenuItem tenantsItem = new ToolStripMenuItem("Tenants");
            ToolStripMenuItem leasesItem = new ToolStripMenuItem("Leases");
            ToolStripMenuItem paymentsItem = new ToolStripMenuItem("Payments");

            // Style management menu items
            foreach (var item in new[] { propertiesItem, tenantsItem, leasesItem, paymentsItem })
            {
                item.BackColor = System.Drawing.Color.White;
                item.ForeColor = System.Drawing.Color.FromArgb(50, 49, 48);
            }

            propertiesItem.Click += (s, e) => ShowPropertiesTab();
            tenantsItem.Click += (s, e) => ShowTenantsTab();
            leasesItem.Click += (s, e) => ShowLeasesTab();
            paymentsItem.Click += (s, e) => ShowPaymentsTab();

            manageMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                propertiesItem, tenantsItem, leasesItem, paymentsItem
            });

            // Reports menu
            ToolStripMenuItem reportsMenu = new ToolStripMenuItem("Reports");
            reportsMenu.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
            reportsMenu.ForeColor = System.Drawing.Color.White;

            ToolStripMenuItem rentRollItem = new ToolStripMenuItem("Rent Roll");
            ToolStripMenuItem vacancyReportItem = new ToolStripMenuItem("Vacancy Report");

            rentRollItem.BackColor = System.Drawing.Color.White;
            rentRollItem.ForeColor = System.Drawing.Color.FromArgb(50, 49, 48);
            vacancyReportItem.BackColor = System.Drawing.Color.White;
            vacancyReportItem.ForeColor = System.Drawing.Color.FromArgb(50, 49, 48);

            rentRollItem.Click += (s, e) => ShowRentRollReport();
            vacancyReportItem.Click += (s, e) => ShowVacancyReport();
            reportsMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                rentRollItem, vacancyReportItem
            });

            // Admin menu (visible only for admin)
            if (auth.IsAdmin)
            {
                ToolStripMenuItem adminMenu = new ToolStripMenuItem("Admin");
                adminMenu.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
                adminMenu.ForeColor = System.Drawing.Color.White;

                ToolStripMenuItem usersItem = new ToolStripMenuItem("Manage Users");
                usersItem.BackColor = System.Drawing.Color.White;
                usersItem.ForeColor = System.Drawing.Color.FromArgb(50, 49, 48);
                usersItem.Click += (s, e) => ShowUserManagement();
                adminMenu.DropDownItems.Add(usersItem);
                menuStrip.Items.Add(adminMenu);
            }

            // Help menu
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Help");
            helpMenu.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
            helpMenu.ForeColor = System.Drawing.Color.White;

            ToolStripMenuItem aboutItem = new ToolStripMenuItem("About");
            aboutItem.BackColor = System.Drawing.Color.White;
            aboutItem.ForeColor = System.Drawing.Color.FromArgb(50, 49, 48);
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
                Name = "mainTabControl",
                Font = new Font("Segoe UI", 9)
            };

            // Dashboard Tab
            TabPage dashboardPage = new TabPage("Dashboard");
            dashboardPage.BackColor = System.Drawing.Color.FromArgb(243, 242, 241);
            CreateDashboard(dashboardPage);
            mainTabControl.TabPages.Add(dashboardPage);

            // Properties Tab
            TabPage propertiesPage = new TabPage("Properties");
            propertiesPage.BackColor = System.Drawing.Color.FromArgb(243, 242, 241);
            CreatePropertiesView(propertiesPage);
            mainTabControl.TabPages.Add(propertiesPage);

            // Tenants Tab
            TabPage tenantsPage = new TabPage("Tenants");
            tenantsPage.BackColor = System.Drawing.Color.FromArgb(243, 242, 241);
            CreateTenantsView(tenantsPage);
            mainTabControl.TabPages.Add(tenantsPage);

            // Leases Tab
            TabPage leasesPage = new TabPage("Leases");
            leasesPage.BackColor = System.Drawing.Color.FromArgb(243, 242, 241);
            CreateLeasesView(leasesPage);
            mainTabControl.TabPages.Add(leasesPage);

            // Payments Tab
            TabPage paymentsPage = new TabPage("Payments");
            paymentsPage.BackColor = System.Drawing.Color.FromArgb(243, 242, 241);
            CreatePaymentsView(paymentsPage);
            mainTabControl.TabPages.Add(paymentsPage);

            this.Controls.Add(mainTabControl);

            // Set dashboard as active tab
            mainTabControl.SelectedIndex = 0;

            // Style the TabControl
            mainTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            mainTabControl.DrawItem += (sender, e) => {
                TabPage page = mainTabControl.TabPages[e.Index];
                Rectangle rect = e.Bounds;
                rect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 2);

                // Fill background
                using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(0, 120, 212)))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                // Draw text
                TextRenderer.DrawText(e.Graphics, page.Text, new Font("Segoe UI", 9, FontStyle.Bold), rect,
                    System.Drawing.Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
        }

        private void CreateDashboard(TabPage page)
        {
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(15),
                BackColor = System.Drawing.Color.FromArgb(243, 242, 241)
            };

            // Summary Cards Panel
            Panel statsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };

            FlowLayoutPanel cardContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10),
                AutoScroll = true
            };

            // Get real statistics from database
            int totalProps = db.GetTotalProperties();
            int occupiedProps = db.GetOccupiedProperties();
            int totalTenants = db.GetTotalTenants();
            decimal monthlyIncome = db.GetMonthlyIncome();

            int occupancyPercentage = totalProps > 0 ? (occupiedProps * 100 / totalProps) : 0;

            // Create styled cards
            Panel CreateCard(string title, string value, string unit, System.Drawing.Color color)
            {
                Panel card = new Panel
                {
                    Width = 280,
                    Height = 110,
                    Margin = new Padding(8),
                    BackColor = System.Drawing.Color.White,
                    BorderStyle = BorderStyle.None
                };

                // Add shadow effect
                card.Paint += (s, e) => {
                    ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle,
                        System.Drawing.Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                        System.Drawing.Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                        System.Drawing.Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                        System.Drawing.Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid);
                };

                Label lblTitle = new Label
                {
                    Text = title,
                    Location = new Point(12, 12),
                    Size = new Size(256, 28),
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = color
                };

                Label lblValue = new Label
                {
                    Text = $"{value}{unit}",
                    Location = new Point(12, 45),
                    Size = new Size(256, 45),
                    Font = new Font("Segoe UI", 22, FontStyle.Bold),
                    ForeColor = System.Drawing.Color.FromArgb(50, 49, 48),
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                };

                card.Controls.Add(lblTitle);
                card.Controls.Add(lblValue);

                return card;
            }

            cardContainer.Controls.Add(CreateCard("Total Properties", totalProps.ToString(), "", System.Drawing.Color.FromArgb(0, 120, 212)));
            cardContainer.Controls.Add(CreateCard("Occupied Properties", $"{occupiedProps}", $" ({occupancyPercentage}%)", System.Drawing.Color.FromArgb(0, 120, 212)));
            cardContainer.Controls.Add(CreateCard("Total Tenants", totalTenants.ToString(), "", System.Drawing.Color.FromArgb(16, 124, 16)));
            cardContainer.Controls.Add(CreateCard("Monthly Income", $"${monthlyIncome:N2}", "", System.Drawing.Color.FromArgb(16, 124, 16)));

            statsPanel.Controls.Add(cardContainer);

            // Welcome Panel
            Panel welcomePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BackColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None
            };

            welcomePanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, welcomePanel.ClientRectangle,
                    System.Drawing.Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                    System.Drawing.Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                    System.Drawing.Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                    System.Drawing.Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid);
            };

            Label lblWelcome = new Label
            {
                Text = $"Welcome back, {auth.CurrentUser.FullName}!",
                Location = new Point(20, 20),
                Size = new Size(350, 35),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 120, 212)
            };

            Label lblStats = new Label
            {
                Text = $"You have {totalProps} properties, {totalTenants} tenants, generating ${monthlyIncome:N2} monthly.",
                Location = new Point(20, 65),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 11),
                ForeColor = System.Drawing.Color.FromArgb(50, 49, 48)
            };

            Label lblRole = new Label
            {
                Text = $"Role: {auth.CurrentUser.Role}",
                Location = new Point(20, 105),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = System.Drawing.Color.Gray
            };

            // Quick actions
            Panel quickActionsPanel = new Panel
            {
                Location = new Point(20, 140),
                Size = new Size(400, 80)
            };

            Button btnAddProperty = new Button
            {
                Text = "Add Property",
                Location = new Point(0, 0),
                Size = new Size(120, 35),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAddProperty.FlatAppearance.BorderSize = 0;
            btnAddProperty.Click += (s, e) => ShowPropertiesTab();

            Button btnAddTenant = new Button
            {
                Text = "Add Tenant",
                Location = new Point(130, 0),
                Size = new Size(120, 35),
                BackColor = System.Drawing.Color.FromArgb(16, 124, 16),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAddTenant.FlatAppearance.BorderSize = 0;
            btnAddTenant.Click += (s, e) => ShowTenantsTab();

            Button btnRecordPayment = new Button
            {
                Text = "Record Payment",
                Location = new Point(260, 0),
                Size = new Size(120, 35),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRecordPayment.FlatAppearance.BorderSize = 0;
            btnRecordPayment.Click += (s, e) => ShowPaymentsTab();

            quickActionsPanel.Controls.AddRange(new Control[] { btnAddProperty, btnAddTenant, btnRecordPayment });

            welcomePanel.Controls.AddRange(new Control[] { lblWelcome, lblStats, lblRole, quickActionsPanel });

            // Upcoming payments with styling
            DataGridView upcomingPayments = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = System.Drawing.Color.FromArgb(225, 223, 221),
                Font = new Font("Segoe UI", 9)
            };

            // Style the DataGridView
            upcomingPayments.EnableHeadersVisualStyles = false;
            upcomingPayments.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
            upcomingPayments.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            upcomingPayments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            upcomingPayments.RowHeadersVisible = false;
            upcomingPayments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            upcomingPayments.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 248, 248);

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
                Text = " Upcoming Rent Due ",
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 120, 212)
            };
            paymentsBox.Controls.Add(upcomingPayments);

            layout.Controls.Add(statsPanel, 0, 0);
            layout.Controls.Add(welcomePanel, 1, 0);
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
                Name = "dgvProperties",
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = System.Drawing.Color.FromArgb(225, 223, 221),
                Font = new Font("Segoe UI", 9)
            };

            // Style the DataGridView
            dgvProperties.EnableHeadersVisualStyles = false;
            dgvProperties.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
            dgvProperties.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvProperties.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvProperties.RowHeadersVisible = false;
            dgvProperties.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProperties.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 248, 248);

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
                Height = 50,
                Padding = new Padding(10),
                BackColor = System.Drawing.Color.FromArgb(243, 242, 241)
            };

            Button btnAdd = new Button
            {
                Text = "Add Property",
                Location = new System.Drawing.Point(10, 8),
                Size = new System.Drawing.Size(120, 35),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;

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
                Location = new System.Drawing.Point(140, 8),
                Size = new System.Drawing.Size(100, 35),
                BackColor = System.Drawing.Color.FromArgb(161, 159, 157),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
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
                Name = "dgvTenants",
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = System.Drawing.Color.FromArgb(225, 223, 221),
                Font = new Font("Segoe UI", 9)
            };

            // Style the DataGridView
            dgvTenants.EnableHeadersVisualStyles = false;
            dgvTenants.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
            dgvTenants.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvTenants.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvTenants.RowHeadersVisible = false;
            dgvTenants.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTenants.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 248, 248);

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
                Height = 50,
                Padding = new Padding(10),
                BackColor = System.Drawing.Color.FromArgb(243, 242, 241)
            };

            Button btnAdd = new Button
            {
                Text = "Add Tenant",
                Location = new System.Drawing.Point(10, 8),
                Size = new System.Drawing.Size(120, 35),
                BackColor = System.Drawing.Color.FromArgb(16, 124, 16),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;

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
                Location = new System.Drawing.Point(140, 8),
                Size = new System.Drawing.Size(100, 35),
                BackColor = System.Drawing.Color.FromArgb(161, 159, 157),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
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
                Name = "dgvLeases",
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = System.Drawing.Color.FromArgb(225, 223, 221),
                Font = new Font("Segoe UI", 9)
            };

            // Style the DataGridView
            dgvLeases.EnableHeadersVisualStyles = false;
            dgvLeases.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
            dgvLeases.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvLeases.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvLeases.RowHeadersVisible = false;
            dgvLeases.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLeases.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 248, 248);

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
                Height = 50,
                Padding = new Padding(10),
                BackColor = System.Drawing.Color.FromArgb(243, 242, 241)
            };

            Button btnAdd = new Button
            {
                Text = "Create Lease",
                Location = new System.Drawing.Point(10, 8),
                Size = new System.Drawing.Size(120, 35),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;

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
                Location = new System.Drawing.Point(140, 8),
                Size = new System.Drawing.Size(100, 35),
                BackColor = System.Drawing.Color.FromArgb(161, 159, 157),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
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
                Name = "dgvPayments",
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = System.Drawing.Color.FromArgb(225, 223, 221),
                Font = new Font("Segoe UI", 9)
            };

            // Style the DataGridView
            dgvPayments.EnableHeadersVisualStyles = false;
            dgvPayments.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
            dgvPayments.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvPayments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvPayments.RowHeadersVisible = false;
            dgvPayments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPayments.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 248, 248);

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
                Height = 50,
                Padding = new Padding(10),
                BackColor = System.Drawing.Color.FromArgb(243, 242, 241)
            };

            Button btnAdd = new Button
            {
                Text = "Record Payment",
                Location = new System.Drawing.Point(10, 8),
                Size = new System.Drawing.Size(130, 35),
                BackColor = System.Drawing.Color.FromArgb(16, 124, 16),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;

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
                Location = new System.Drawing.Point(150, 8),
                Size = new System.Drawing.Size(100, 35),
                BackColor = System.Drawing.Color.FromArgb(161, 159, 157),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
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
            MessageBox.Show("Rent Roll Report - Coming soon!\n\nThis feature will generate detailed rent roll reports.", "Report",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowVacancyReport()
        {
            MessageBox.Show("Vacancy Report - Coming soon!\n\nThis feature will show property vacancy analysis.", "Report",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowUserManagement()
        {
            MessageBox.Show("User Management - Coming soon!\n\nAdmin users will be able to manage user accounts here.", "Admin",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAbout()
        {
            MessageBox.Show("Property Management System\nVersion 1.0\n\nDeveloped with ❤️ for efficient property management\n\n" +
                $"Logged in as: {auth.CurrentUser.FullName} ({auth.CurrentUser.Role})\n" +
                $"© 2024 Property Management System",
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}