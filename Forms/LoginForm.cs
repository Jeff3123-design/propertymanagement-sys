using System;
using System.Windows.Forms;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Helpers;

namespace PropertyManagementSystem.Forms
{
    public partial class LoginForm : Form
    {
        private DatabaseHelper db;
        private AuthHelper auth;

        public LoginForm()
        {
            db = new DatabaseHelper();
            auth = new AuthHelper(db);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Property Management System - Login";
            this.Size = new System.Drawing.Size(480, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(243, 242, 241);

            // Main Panel to hold everything
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(0)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120)); // Top panel
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Middle content
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Bottom panel

            // Top Panel with gradient effect
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212)
            };

            // Logo/Icon Panel
            Panel logoPanel = new Panel
            {
                Size = new Size(80, 80),
                Location = new Point(40, 20),
                BackColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None
            };

            // Create a simple building icon using a label
            Label buildingIcon = new Label
            {
                Text = "🏢",
                Font = new Font("Segoe UI", 48, FontStyle.Regular),
                ForeColor = System.Drawing.Color.FromArgb(0, 120, 212),
                Location = new Point(10, 10),
                Size = new Size(60, 60),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                BackColor = System.Drawing.Color.Transparent
            };
            logoPanel.Controls.Add(buildingIcon);

            // Title Label
            Label lblTitle = new Label
            {
                Text = "Property Management System",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                Location = new Point(140, 35),
                Size = new Size(300, 45),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = System.Drawing.Color.Transparent
            };

            topPanel.Controls.Add(logoPanel);
            topPanel.Controls.Add(lblTitle);

            // Middle Panel - Login Form
            Panel middlePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.White,
                Padding = new Padding(40, 30, 40, 30)
            };

            // Create a centered panel for login controls
            Panel loginPanel = new Panel
            {
                Width = 360,
                Height = 320,
                Location = new Point((middlePanel.Width - 360) / 2, 20),
                Anchor = AnchorStyles.Top
            };

            // Login Header
            Label lblLoginHeader = new Label
            {
                Text = "Welcome Back!",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 120, 212),
                Location = new Point(0, 0),
                Size = new Size(360, 40),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Label lblSubHeader = new Label
            {
                Text = "Please login to your account",
                Font = new Font("Segoe UI", 10),
                ForeColor = System.Drawing.Color.Gray,
                Location = new Point(0, 45),
                Size = new Size(360, 25),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            int yPos = 90;
            int labelWidth = 80;
            int controlWidth = 260;
            int xPos = 20;

            // Username Label and TextBox
            Label lblUsername = new Label
            {
                Text = "Username",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(50, 49, 48)
            };
            yPos += 25;

            TextBox txtUsername = new TextBox
            {
                Name = "txtUsername",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.FromArgb(250, 250, 250)
            };
            yPos += 50;

            // Password Label and TextBox
            Label lblPassword = new Label
            {
                Text = "Password",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(50, 49, 48)
            };
            yPos += 25;

            TextBox txtPassword = new TextBox
            {
                Name = "txtPassword",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 35),
                Font = new Font("Segoe UI", 11),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.FromArgb(250, 250, 250)
            };
            yPos += 50;

            // Remember Me Checkbox
            CheckBox chkRememberMe = new CheckBox
            {
                Text = "Remember me",
                Location = new Point(xPos, yPos),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = System.Drawing.Color.FromArgb(50, 49, 48)
            };
            yPos += 40;

            // Login Button (Blue)
            Button btnLogin = new Button
            {
                Text = "LOGIN",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 45),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 100, 180);

            btnLogin.Click += (s, e) => {
                if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Please enter username and password.", "Login Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsername.Focus();
                    return;
                }

                if (auth.Login(txtUsername.Text, txtPassword.Text))
                {
                    MessageBox.Show($"Welcome {auth.CurrentUser.FullName}!", "Login Successful",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    MainForm mainForm = new MainForm(db, auth);
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
            };

            yPos += 55;

            // Register link
            LinkLabel lnkRegister = new LinkLabel
            {
                Text = "Don't have an account? Register here",
                Location = new Point(xPos + 50, yPos),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                LinkColor = System.Drawing.Color.FromArgb(0, 120, 212)
            };

            lnkRegister.Click += (s, e) => {
                RegisterForm registerForm = new RegisterForm(db);
                registerForm.ShowDialog();
            };

            loginPanel.Controls.AddRange(new Control[] {
                lblLoginHeader, lblSubHeader,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                chkRememberMe, btnLogin, lnkRegister
            });

            // Center the login panel when form resizes
            middlePanel.Resize += (s, e) => {
                loginPanel.Location = new Point((middlePanel.Width - loginPanel.Width) / 2, 30);
            };

            middlePanel.Controls.Add(loginPanel);

            // Bottom Panel - Info
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(243, 242, 241),
                Padding = new Padding(10)
            };

            Label lblInfo = new Label
            {
                Text = "Default Admin Credentials:",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 120, 212),
                Location = new Point(20, 5),
                Size = new Size(150, 15)
            };

            Label lblAdmin = new Label
            {
                Text = "Username: admin  |  Password: admin123",
                Font = new Font("Segoe UI", 8),
                ForeColor = System.Drawing.Color.Gray,
                Location = new Point(20, 25),
                Size = new Size(300, 15)
            };

            Label lblVersion = new Label
            {
                Text = "Version 1.0",
                Font = new Font("Segoe UI", 7),
                ForeColor = System.Drawing.Color.Gray,
                Location = new Point(400, 5),
                Size = new Size(60, 15),
                TextAlign = System.Drawing.ContentAlignment.MiddleRight
            };

            bottomPanel.Controls.AddRange(new Control[] { lblInfo, lblAdmin, lblVersion });

            // Add all panels to main layout
            mainLayout.Controls.Add(topPanel, 0, 0);
            mainLayout.Controls.Add(middlePanel, 0, 1);
            mainLayout.Controls.Add(bottomPanel, 0, 2);

            this.Controls.Add(mainLayout);

            // Set focus to username field
            this.Shown += (s, e) => {
                txtUsername.Focus();
            };
        }
    }
}