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
            this.Size = new System.Drawing.Size(450, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label lblTitle = new Label
            {
                Text = "Create New Account",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new System.Drawing.Point(120, 15),
                Size = new System.Drawing.Size(200, 30)
            };

            int yPos = 60;
            int labelWidth = 120;
            int controlWidth = 200;
            int xPos = 100;

            // Full Name
            Label lblFullName = new Label { Text = "Full Name:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtFullName = new TextBox { Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 40;

            // Username
            Label lblUsername = new Label { Text = "Username:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtUsername = new TextBox { Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 40;

            // Email
            Label lblEmail = new Label { Text = "Email:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtEmail = new TextBox { Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 40;

            // Password
            Label lblPassword = new Label { Text = "Password:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtPassword = new TextBox { Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25), PasswordChar = '*' };
            yPos += 40;

            // Confirm Password
            Label lblConfirm = new Label { Text = "Confirm Password:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtConfirm = new TextBox { Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25), PasswordChar = '*' };
            yPos += 50;

            // Register Button
            Button btnRegister = new Button
            {
                Text = "Register",
                Location = new Point(xPos + 50, yPos),
                Size = new Size(100, 35),
                BackColor = System.Drawing.Color.LightGreen
            };

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

            // Cancel Button
            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(xPos + 50, yPos + 45),
                Size = new Size(100, 35)
            };

            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblFullName, txtFullName, lblUsername, txtUsername,
                lblEmail, txtEmail, lblPassword, txtPassword, lblConfirm, txtConfirm,
                btnRegister, btnCancel
            });
        }
    }
}