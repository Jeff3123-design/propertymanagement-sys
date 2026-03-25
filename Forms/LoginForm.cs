using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Helpers;
using PropertyManagementSystem.Models;
using PropertyManagementSystem.Dashboards;

namespace PropertyManagementSystem.Forms
{
    public partial class LoginForm : Form
    {
        private DatabaseHelper db;
        private AuthHelper auth;
        private bool isDarkMode = false;
        private Panel? loaderPanel;
        private TextBox? txtUsername;
        private TextBox? txtPassword;

        public LoginForm()
        {
            db = new DatabaseHelper();
            auth = new AuthHelper(db);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Property Management System";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Main Panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // ===== HEADER WITH GRADIENT =====
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(0, 120, 212)
            };
            header.Paint += (s, e) => DrawGradientHeader(header, e);

            // Logo/Icon
            PictureBox logo = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(60, 60),
                Location = new Point(30, 30),
                BackColor = Color.Transparent
            };

            try
            {
                if (System.IO.File.Exists("logo.png"))
                    logo.Image = Image.FromFile("logo.png");
                else
                {
                    Bitmap bmp = new Bitmap(60, 60);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.White);
                        using (Font drawFont = new Font("Segoe UI", 32))
                        using (SolidBrush drawBrush = new SolidBrush(Color.FromArgb(0, 120, 212)))
                        {
                            g.DrawString("🏢", drawFont, drawBrush, new PointF(12, 8));
                        }
                    }
                    logo.Image = bmp;
                }
            }
            catch { }

            Label title = new Label
            {
                Text = "Property Management System",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(110, 40),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Dark Mode Toggle
            Button btnDarkMode = new Button
            {
                Text = "🌙",
                Location = new Point(430, 15),
                Size = new Size(40, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14),
                Cursor = Cursors.Hand
            };
            btnDarkMode.FlatAppearance.BorderSize = 0;
            btnDarkMode.Click += (s, e) => ToggleDarkMode(mainPanel);

            header.Controls.Add(logo);
            header.Controls.Add(title);
            header.Controls.Add(btnDarkMode);

            // ===== LOGIN PANEL =====
            Panel loginPanel = new Panel
            {
                Location = new Point(50, 150),
                Size = new Size(400, 350),
                BackColor = Color.White
            };

            // Welcome Message
            Label lblWelcome = new Label
            {
                Text = "Welcome Back!",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 212),
                Location = new Point(0, 0),
                Size = new Size(400, 45),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblSubWelcome = new Label
            {
                Text = "Please login to your account",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Location = new Point(0, 50),
                Size = new Size(400, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            int yPos = 100;
            int controlWidth = 360;
            int xPos = 20;

            // Username Field
            Label lblUsername = new Label
            {
                Text = "USERNAME",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 20)
            };
            yPos += 25;

            txtUsername = new TextBox
            {
                Name = "txtUsername",
                Location = new Point(xPos, yPos),
                Width = controlWidth,
                Height = 40,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250)
            };
            yPos += 55;

            // Password Field
            Label lblPassword = new Label
            {
                Text = "PASSWORD",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 20)
            };
            yPos += 25;

            txtPassword = new TextBox
            {
                Name = "txtPassword",
                Location = new Point(xPos, yPos),
                Width = controlWidth,
                Height = 40,
                Font = new Font("Segoe UI", 11),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250)
            };
            yPos += 55;

            // Remember Me Checkbox
            CheckBox chkRememberMe = new CheckBox
            {
                Text = "Remember me",
                Location = new Point(xPos, yPos),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            yPos += 45;

            // Login Button
            Button btnLogin = new Button
            {
                Text = "LOGIN",
                Location = new Point(xPos, yPos),
                Width = controlWidth,
                Height = 45,
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 100, 180);

            btnLogin.Click += (s, e) =>
            {
                if (txtUsername == null || txtPassword == null)
                    return;

                if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Please enter username and password.", "Login Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ShowLoader();

                System.Windows.Forms.Timer t = new System.Windows.Forms.Timer { Interval = 1000 };
                t.Tick += (s2, e2) =>
                {
                    t.Stop();
                    if (loaderPanel != null)
                        loaderPanel.Visible = false;

                    if (auth.Login(txtUsername.Text, txtPassword.Text))
                    {
                        string userRole = auth.CurrentUser?.Role ?? "Staff";
                        OpenDashboard(userRole);
                    }
                    else
                    {
                        MessageBox.Show("Invalid credentials. Please check your username and password.",
                            "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                t.Start();
            };

            yPos += 55;

            // Register Link
            LinkLabel lnkRegister = new LinkLabel
            {
                Text = "Don't have an account? Register here",
                Location = new Point(xPos + 100, yPos),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter,
                LinkColor = Color.FromArgb(0, 120, 212)
            };
            lnkRegister.Click += (s, e) => {
                RegisterForm registerForm = new RegisterForm(db);
                registerForm.ShowDialog();
            };

            loginPanel.Controls.AddRange(new Control[] {
                lblWelcome, lblSubWelcome,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                chkRememberMe, btnLogin, lnkRegister
            });

            // ===== LOADER PANEL =====
            loaderPanel = new Panel
            {
                Size = new Size(200, 80),
                BackColor = Color.FromArgb(240, 240, 240),
                Visible = false,
                Location = new Point(150, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label loading = new Label
            {
                Text = "Loading... ⏳",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(0, 120, 212)
            };
            loaderPanel.Controls.Add(loading);

            // ===== FOOTER =====
            Panel footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(248, 248, 248)
            };

            Label footerText = new Label
            {
                Text = "© 2024 Property Management System | All Rights Reserved",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };
            footer.Controls.Add(footerText);

            // Add all to main panel
            mainPanel.Controls.Add(header);
            mainPanel.Controls.Add(loginPanel);
            mainPanel.Controls.Add(loaderPanel);
            mainPanel.Controls.Add(footer);

            this.Controls.Add(mainPanel);

            // Center login panel on resize
            this.Resize += (s, e) => {
                loginPanel.Location = new Point((this.Width - loginPanel.Width) / 2, 150);
                if (loaderPanel != null)
                    loaderPanel.Location = new Point((this.Width - loaderPanel.Width) / 2, 250);
            };

            // Set focus to username field
            this.Shown += (s, e) => {
                txtUsername?.Focus();
            };
        }

        // ===== ROLE DASHBOARD ROUTER =====
        private void OpenDashboard(string role)
        {
            Form? dashboard = null;

            switch (role)
            {
                case "Admin":
                    dashboard = new AdminDashboard();
                    break;
                case "Manager":
                    dashboard = new ManagerDashboard();
                    break;
                case "Staff":
                    dashboard = new StaffDashboard();
                    break;
                case "Tenant":
                    dashboard = new TenantDashboard();
                    break;
                default:
                    dashboard = new StaffDashboard();
                    break;
            }

            if (dashboard != null)
            {
                dashboard.Show();
                this.Hide();
            }
        }

        private void ToggleDarkMode(Control parent)
        {
            isDarkMode = !isDarkMode;
            Color bg = isDarkMode ? Color.FromArgb(30, 30, 30) : Color.White;
            Color fg = isDarkMode ? Color.White : Color.Black;
            ApplyTheme(parent, bg, fg);
        }

        private void ApplyTheme(Control control, Color bg, Color fg)
        {
            control.BackColor = bg;
            control.ForeColor = fg;
            foreach (Control c in control.Controls)
                ApplyTheme(c, bg, fg);
        }

        private void ShowLoader()
        {
            if (loaderPanel != null)
            {
                loaderPanel.Visible = true;
                loaderPanel.BringToFront();
            }
        }

        private void DrawGradientHeader(Panel panel, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(0, 100, 180),
                Color.FromArgb(0, 150, 220),
                45F))
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }
    }
}