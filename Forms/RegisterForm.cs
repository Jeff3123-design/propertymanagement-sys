using System;
using System.Windows.Forms;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Helpers;
using PropertyManagementSystem.Models;

namespace PropertyManagementSystem.Forms
{
    public partial class RegisterForm : Form
    {
        private DatabaseHelper db;

        public RegisterForm(DatabaseHelper database)
        {
            db = database;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Register New User";
            this.Size = new System.Drawing.Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(243, 242, 241);

            // Top Panel with gradient effect
            Panel topPanel = new Panel
            {
                Height = 70,
                Dock = DockStyle.Top,
                BackColor = System.Drawing.Color.FromArgb(16, 124, 16) // Green
            };

            // Title
            Label lblTitle = new Label
            {
                Text = "Create New Account",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                Location = new System.Drawing.Point(150, 20),
                Size = new System.Drawing.Size(200, 30),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                BackColor = System.Drawing.Color.Transparent
            };

            topPanel.Controls.Add(lblTitle);

            // Main panel for registration controls
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                AutoScroll = true
            };

            int yPos = 20;
            int labelWidth = 120;
            int controlWidth = 250;
            int xPos = 40;

            // Full Name
            Label lblFullName = new Label
            {
                Text = "Full Name:",
                Location = new Point(xPos, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(50, 49, 48)
            };
            TextBox txtFullName = new TextBox
            {
                Name = "txtFullName",
                Location = new Point(xPos + labelWidth + 10, yPos),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            yPos += 45;

            // Username
            Label lblUsername = new Label
            {
                Text = "Username:*",
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

            // Email
            Label lblEmail = new Label
            {
                Text = "Email:*",
                Location = new Point(xPos, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(50, 49, 48)
            };
            TextBox txtEmail = new TextBox
            {
                Name = "txtEmail",
                Location = new Point(xPos + labelWidth + 10, yPos),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            yPos += 45;

            // Password
            Label lblPassword = new Label
            {
                Text = "Password:*",
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

            // Confirm Password
            Label lblConfirm = new Label
            {
                Text = "Confirm Password:*",
                Location = new Point(xPos, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(50, 49, 48)
            };
            TextBox txtConfirm = new TextBox
            {
                Name = "txtConfirm",
                Location = new Point(xPos + labelWidth + 10, yPos),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle
            };
            yPos += 55;

            // Register Button (Green)
            Button btnRegister = new Button
            {
                Text = "Register",
                Location = new Point(xPos + labelWidth + 10, yPos),
                Size = new Size(120, 40),
                BackColor = System.Drawing.Color.FromArgb(16, 124, 16),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;

            btnRegister.Click += (s, e) => {
                if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Please fill all required fields.", "Registration Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtPassword.Text != txtConfirm.Text)
                {
                    MessageBox.Show("Passwords do not match.", "Registration Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtPassword.Text.Length < 4)
                {
                    MessageBox.Show("Password must be at least 4 characters long.", "Registration Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var user = new User
                {
                    Username = txtUsername.Text,
                    Email = txtEmail.Text,
                    FullName = txtFullName.Text,
                    Role = "Staff" // Default role
                };

                var auth = new AuthHelper(db);
                if (auth.Register(user, txtPassword.Text))
                {
                    MessageBox.Show("Registration successful! You can now login.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username already exists. Please choose a different username.",
                        "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Cancel Button (Gray)
            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(xPos + labelWidth + 150, yPos),
                Size = new Size(100, 40),
                BackColor = System.Drawing.Color.FromArgb(161, 159, 157),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            yPos += 55;

            // Required fields note
            Label lblRequired = new Label
            {
                Text = "* Required fields",
                Location = new Point(xPos, yPos),
                Size = new Size(150, 20),
                ForeColor = System.Drawing.Color.Gray,
                Font = new Font("Segoe UI", 8)
            };

            mainPanel.Controls.AddRange(new Control[] {
                lblFullName, txtFullName, lblUsername, txtUsername,
                lblEmail, txtEmail, lblPassword, txtPassword,
                lblConfirm, txtConfirm, btnRegister, btnCancel, lblRequired
            });

            this.Controls.Add(topPanel);
            this.Controls.Add(mainPanel);
        }
    }
}