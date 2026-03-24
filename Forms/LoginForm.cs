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
            this.Size = new System.Drawing.Size(450, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(243, 242, 241); // Light gray background

            // Top Panel with gradient effect
            Panel topPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212) // Blue
            };

            // Title Label
            Label lblTitle = new Label
            {
                Text = "Property Management System",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                Location = new System.Drawing.Point(80, 25),
                Size = new System.Drawing.Size(290, 35),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                BackColor = System.Drawing.Color.Transparent
            };

            topPanel.Controls.Add(lblTitle);

            // Main panel for login controls
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30)
            };

            int yPos = 30;
            int labelWidth = 100;
            int controlWidth = 250;
            int xPos = 30;

            // Username Label and TextBox
            Label lblUsername = new Label
            {
                Text = "Username:",
                Location = new Point(xPos, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(50, 49, 48)
            };

            TextBox txtUsername = new TextBox
            {
                Name = "txtUsername",
                Location = new Point(xPos + labelWidth + 10, yPos),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            yPos += 45;

            // Password Label and TextBox
            Label lblPassword = new Label
            {
                Text = "Password:",
                Location = new Point(xPos, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(50, 49, 48)
            };

            TextBox txtPassword = new TextBox
            {
                Name = "txtPassword",
                Location = new Point(xPos + labelWidth + 10, yPos),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle
            };
            yPos += 45;

            // Remember Me Checkbox
            CheckBox chkRememberMe = new CheckBox
            {
                Text = "Remember Me",
                Location = new Point(xPos + labelWidth + 10, yPos),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = System.Drawing.Color.FromArgb(50, 49, 48)
            };
            yPos += 45;

            // Login Button (Blue)
            Button btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(xPos + labelWidth + 10, yPos),
                Size = new Size(120, 40),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;

            btnLogin.Click += (s, e) => {
                if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Please enter username and password.", "Login Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    txtPassword.Focus();
                }
            };

            // Register Button (Green)
            Button btnRegister = new Button
            {
                Text = "Register",
                Location = new Point(xPos + labelWidth + 150, yPos),
                Size = new Size(100, 40),
                BackColor = System.Drawing.Color.FromArgb(16, 124, 16),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;

            btnRegister.Click += (s, e) => {
                RegisterForm registerForm = new RegisterForm(db);
                registerForm.ShowDialog();
            };
            yPos += 55;

            // Status Label
            Label lblStatus = new Label
            {
                Text = "Default Admin: admin / admin123",
                Location = new Point(xPos, yPos),
                Size = new Size(350, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ForeColor = System.Drawing.Color.Gray,
                Font = new Font("Segoe UI", 8)
            };

            mainPanel.Controls.AddRange(new Control[] {
                lblUsername, txtUsername, lblPassword, txtPassword,
                chkRememberMe, btnLogin, btnRegister, lblStatus
            });

            this.Controls.Add(topPanel);
            this.Controls.Add(mainPanel);
        }
    }
}