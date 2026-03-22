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

            // Show/hide admin-only features
            if (!auth.IsAdmin)
            {
                // Hide admin-only menu items if any
                // Disable certain features for non-admin users
            }
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
            ToolStripMenuItem logoutItem = new ToolStripMenuItem("Log