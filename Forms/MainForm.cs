using System;
using System.Windows.Forms;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Models.Forms;

namespace PropertyManagementSystem.Forms
{
    public partial class MainForm : Form
    {
        private DatabaseHelper db;

        public MainForm()
        {
            InitializeComponent();
            db = new DatabaseHelper();
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
            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => Application.Exit();
            fileMenu.DropDownItems.Add(exitItem);

            // Management menu
            ToolStripMenuItem manageMenu = new ToolStripMenuItem("Management");
            ToolStripMenuItem propertiesItem = new ToolStripMenuItem("Properties");
            ToolStripMenuItem tenantsItem = new ToolStripMenuItem("Tenants");
            ToolStripMenuItem leasesItem = new ToolStripMenuItem("Leases");
            ToolStripMenuItem paymentsItem = new ToolStripMenuItem("Payments");

            propertiesItem.Click += PropertiesItem_Click;
            tenantsItem.Click += TenantsItem_Click;
            leasesItem.Click += LeasesItem_Click;
            paymentsItem.Click += PaymentsItem_Click;

            manageMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                propertiesItem, tenantsItem, leasesItem, paymentsItem
            });

            // Reports menu
            ToolStripMenuItem reportsMenu = new ToolStripMenuItem("Reports");
            ToolStripMenuItem rentRollItem = new ToolStripMenuItem("Rent Roll");
            ToolStripMenuItem vacancyReportItem = new ToolStripMenuItem("Vacancy Report");
            reportsMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                rentRollItem, vacancyReportItem
            });

            menuStrip.Items.AddRange(new ToolStripMenuItem[] {
                fileMenu, manageMenu, reportsMenu
            });

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Create TabControl for main content
            TabControl tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Padding = new System.Drawing.Point(10, 5)
            };

            // Dashboard Tab
            TabPage dashboardPage = new TabPage("Dashboard");
            CreateDashboard(dashboardPage);
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

            // Add statistics labels
            Label totalProperties = new Label { Text = "Total Properties: 0", Font = new Font("Arial", 12, FontStyle.Bold), AutoSize = true };
            Label occupiedProperties = new Label { Text = "Occupied Properties: 0", Font = new Font("Arial", 12, FontStyle.Bold), AutoSize = true };
            Label totalTenants = new Label { Text = "Total Tenants: 0", Font = new Font("Arial", 12, FontStyle.Bold), AutoSize = true };
            Label monthlyIncome = new Label { Text = "Monthly Income: $0", Font = new Font("Arial", 12, FontStyle.Bold), AutoSize = true };

            statsPanel.Controls.AddRange(new Control[] { totalProperties, occupiedProperties, totalTenants, monthlyIncome });
            statsBox.Controls.Add(statsPanel);

            // Recent Activities
            ListBox recentActivities = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 10)
            };

            recentActivities.Items.Add("Welcome to Property Management System");
            recentActivities.Items.Add("Add properties, tenants, and manage leases");

            GroupBox activitiesBox = new GroupBox
            {
                Text = "Recent Activities",
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            activitiesBox.Controls.Add(recentActivities);

            layout.Controls.Add(statsBox, 0, 0);
            layout.Controls.Add(activitiesBox, 1, 0);

            // Add upcoming payments
            DataGridView upcomingPayments = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

            GroupBox paymentsBox = new GroupBox
            {
                Text = "Upcoming Payments",
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
                ReadOnly = true
            };

            // Load properties data
            dgvProperties.DataSource = db.GetAllProperties();

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
                PropertyForm propertyForm = new PropertyForm(db);
                propertyForm.ShowDialog();
                dgvProperties.DataSource = db.GetAllProperties();
            };

            Button btnEdit = new Button
            {
                Text = "Edit Property",
                Location = new System.Drawing.Point(110, 5),
                Size = new System.Drawing.Size(100, 30)
            };

            Button btnDelete = new Button
            {
                Text = "Delete Property",
                Location = new System.Drawing.Point(215, 5),
                Size = new System.Drawing.Size(100, 30),
                BackColor = System.Drawing.Color.LightCoral
            };

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

            page.Controls.Add(dgvProperties);
            page.Controls.Add(buttonPanel);
        }

        private void CreateTenantsView(TabPage page)
        {
            DataGridView dgvTenants = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

            dgvTenants.DataSource = db.GetAllTenants();

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
                TenantForm tenantForm = new TenantForm(db);
                tenantForm.ShowDialog();
                dgvTenants.DataSource = db.GetAllTenants();
            };

            buttonPanel.Controls.Add(btnAdd);

            page.Controls.Add(dgvTenants);
            page.Controls.Add(buttonPanel);
        }

        private void CreateLeasesView(TabPage page)
        {
            DataGridView dgvLeases = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

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
                LeaseForm leaseForm = new LeaseForm(db);
                leaseForm.ShowDialog();
            };

            buttonPanel.Controls.Add(btnAdd);

            page.Controls.Add(dgvLeases);
            page.Controls.Add(buttonPanel);
        }

        private void CreatePaymentsView(TabPage page)
        {
            DataGridView dgvPayments = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

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
                PaymentForm paymentForm = new PaymentForm(db);
                paymentForm.ShowDialog();
            };

            buttonPanel.Controls.Add(btnAdd);

            page.Controls.Add(dgvPayments);
            page.Controls.Add(buttonPanel);
        }

        private void PropertiesItem_Click(object sender, EventArgs e)
        {
            // Switch to properties tab
        }

        private void TenantsItem_Click(object sender, EventArgs e)
        {
            // Switch to tenants tab
        }

        private void LeasesItem_Click(object sender, EventArgs e)
        {
            // Switch to leases tab
        }

        private void PaymentsItem_Click(object sender, EventArgs e)
        {
            // Switch to payments tab
        }
    }
}