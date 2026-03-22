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
            InitializeComponent();
            db = new DatabaseHelper();
            auth = new AuthHelper(db);
        }

        private void InitializeComponent()
        {
            this.Text = "Property Management System - Login";
            this.Size = new System.Drawing.Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title Label
            Label lblTitle = new Label
            {
                Text = "Property Management System",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new System.Drawing.Point(80, 20),
                Size = new System.Drawing.Size(240, 30),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Username Label and TextBox
            Label lblUsername = new Label
            {
                Text = "Username:",
                Location = new System.Drawing.Point(50, 80),
                Size = new System.Drawing.Size(100, 25),
                Font = new Font("Arial", 10)
            };

            TextBox txtUsername = new TextBox
            {
                Location = new System.Drawing.Point(150, 80),
                Size = new System.Drawing.Size(200, 25),
                Font = new Font("Arial", 10)
            };

            // Password Label and TextBox
            Label lblPassword = new Label
            {
                Text = "Password:",
                Location = new System.Drawing.Point(50, 120),
                Size = new System.Drawing.Size(100, 25),
                Font = new Font("Arial", 10)
            };

            TextBox txtPassword = new TextBox
            {
                Location = new System.Drawing.Point(150, 120),
                Size = new System.Drawing.Size(200, 25),
                Font = new Font("Arial", 10),
                PasswordChar = '*'
            };

            // Remember Me Checkbox
            CheckBox chkRememberMe = new CheckBox
            {
                Text = "Remember Me",
                Location = new System.Drawing.Point(150, 155),
                Size = new System.Drawing.Size(120, 25)
            };

            // Login Button
            Button btnLogin = new Button
            {
                Text = "Login",
                Location = new System.Drawing.Point(150, 195),
                Size = new System.Drawing.Size(100, 35),
                BackColor = System.Drawing.Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };

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

            // Register Button
            Button btnRegister = new Button
            {
                Text = "Register",
                Location = new System.Drawing.Point(150, 240),
                Size = new System.Drawing.Size(100, 35),
                FlatStyle = FlatStyle.Flat
            };

            btnRegister.Click += (s, e) => {
                RegisterForm registerForm = new RegisterForm(db);
                registerForm.ShowDialog();
            };

            // Status Label
            Label lblStatus = new Label
            {
                Text = "Default Admin: admin / admin123",
                Location = new System.Drawing.Point(50, 290),
                Size = new System.Drawing.Size(300, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ForeColor = System.Drawing.Color.Gray,
                Font = new Font("Arial", 8)
            };

            this.Controls.AddRange(new Control[] {
                lblTitle, lblUsername, txtUsername, lblPassword, txtPassword,
                chkRememberMe, btnLogin, btnRegister, lblStatus
            });
        }
    }
}