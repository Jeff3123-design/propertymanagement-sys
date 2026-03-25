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
        private AuthHelper auth;  // ADD THIS LINE

        public RegisterForm(DatabaseHelper database)
        {
            db = database;
            auth = new AuthHelper(db);  // ADD THIS LINE - Initialize auth
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Register New User";
            this.Size = new System.Drawing.Size(500, 620);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.White;

            // Main Panel with scrolling
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = System.Drawing.Color.White,
                AutoScroll = true
            };

            // Title
            Label lblTitle = new Label
            {
                Text = "Create New Account",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(16, 124, 16),
                Location = new Point(0, 0),
                Size = new Size(440, 45),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblSubTitle = new Label
            {
                Text = "Join us to start managing properties",
                Font = new Font("Segoe UI", 10),
                ForeColor = System.Drawing.Color.Gray,
                Location = new Point(0, 50),
                Size = new Size(440, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            int yPos = 100;
            int labelWidth = 120;
            int controlWidth = 280;
            int xPos = 20;

            // Full Name
            Label lblFullName = new Label
            {
                Text = "Full Name",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(80, 80, 80)
            };
            yPos += 25;

            TextBox txtFullName = new TextBox
            {
                Name = "txtFullName",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 35),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.FromArgb(250, 250, 250)
            };
            yPos += 50;

            // Username
            Label lblUsername = new Label
            {
                Text = "Username *",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(80, 80, 80)
            };
            yPos += 25;

            TextBox txtUsername = new TextBox
            {
                Name = "txtUsername",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 35),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.FromArgb(250, 250, 250)
            };
            yPos += 50;

            // Email
            Label lblEmail = new Label
            {
                Text = "Email *",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(80, 80, 80)
            };
            yPos += 25;

            TextBox txtEmail = new TextBox
            {
                Name = "txtEmail",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 35),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.FromArgb(250, 250, 250)
            };
            yPos += 50;

            // Password
            Label lblPassword = new Label
            {
                Text = "Password *",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(80, 80, 80)
            };
            yPos += 25;

            TextBox txtPassword = new TextBox
            {
                Name = "txtPassword",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 35),
                Font = new Font("Segoe UI", 10),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.FromArgb(250, 250, 250)
            };
            yPos += 50;

            // Confirm Password
            Label lblConfirm = new Label
            {
                Text = "Confirm Password *",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(80, 80, 80)
            };
            yPos += 25;

            TextBox txtConfirm = new TextBox
            {
                Name = "txtConfirm",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 35),
                Font = new Font("Segoe UI", 10),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.FromArgb(250, 250, 250)
            };
            yPos += 50;

            // ROLE SELECTION
            Label lblRole = new Label
            {
                Text = "Select Role *",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(80, 80, 80)
            };
            yPos += 25;

            ComboBox cmbRole = new ComboBox
            {
                Name = "cmbRole",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 35),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                BackColor = System.Drawing.Color.White
            };
            cmbRole.Items.AddRange(new string[] { "Staff", "Manager", "Tenant" });
            cmbRole.SelectedIndex = 0;
            yPos += 55;

            // Info note about roles
            Label lblRoleInfo = new Label
            {
                Text = "💡 Role Information:\n   Staff: Full access to properties, tenants, leases, payments\n   Manager: All staff access except user management\n   Tenant: View-only access to your property and payments",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 65),
                Font = new Font("Segoe UI", 8),
                ForeColor = System.Drawing.Color.Gray
            };
            yPos += 80;

            // Register Button
            Button btnRegister = new Button
            {
                Text = "REGISTER",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 45),
                BackColor = System.Drawing.Color.FromArgb(16, 124, 16),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(13, 100, 13);

            btnRegister.Click += (s, e) => {
                // Validation
                if (string.IsNullOrEmpty(txtUsername.Text))
                {
                    MessageBox.Show("Username is required.", "Registration Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsername.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtEmail.Text) || !txtEmail.Text.Contains("@"))
                {
                    MessageBox.Show("Valid email address is required.", "Registration Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Password is required.", "Registration Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                if (txtPassword.Text != txtConfirm.Text)
                {
                    MessageBox.Show("Passwords do not match.", "Registration Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                if (txtPassword.Text.Length < 4)
                {
                    MessageBox.Show("Password must be at least 4 characters long.", "Registration Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                // Create user with selected role
                var user = new User
                {
                    Username = txtUsername.Text,
                    Email = txtEmail.Text,
                    FullName = txtFullName.Text,
                    Role = cmbRole.SelectedItem.ToString()  // Role selected during registration
                };

                // Use the auth helper to register
                if (auth.Register(user, txtPassword.Text))
                {
                    MessageBox.Show($"Registration successful! You are registered as {user.Role}.\n\nYou can now login.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username already exists. Please choose a different username.",
                        "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtUsername.Focus();
                    txtUsername.SelectAll();
                }
            };

            yPos += 60;

            // Cancel Button
            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 40),
                BackColor = System.Drawing.Color.FromArgb(161, 159, 157),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            mainPanel.Controls.AddRange(new Control[] {
                lblTitle, lblSubTitle,
                lblFullName, txtFullName,
                lblUsername, txtUsername,
                lblEmail, txtEmail,
                lblPassword, txtPassword,
                lblConfirm, txtConfirm,
                lblRole, cmbRole, lblRoleInfo,
                btnRegister, btnCancel
            });

            this.Controls.Add(mainPanel);

            // Set focus
            this.Shown += (s, e) => {
                txtFullName.Focus();
            };
        }
    }
}