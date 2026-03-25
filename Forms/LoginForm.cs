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
        private Panel? loaderPanel;  // Made nullable
        private ComboBox? cmbRole;    // Made nullable
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
            this.Size = new Size(700, 750);
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

            // ===== HEADER WITH LOGO =====
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 140,
                BackColor = Color.FromArgb(0, 120, 212)
            };
            header.Paint += (s, e) => DrawGradientHeader(header, e);

            // Try to load logo, if not found use text icon
            PictureBox logo = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(70, 70),
                Location = new Point(30, 35),
                BackColor = Color.Transparent
            };

            try
            {
                if (System.IO.File.Exists("logo.png"))
                    logo.Image = Image.FromFile("logo.png");
                else
                {
                    // Create a simple colored square with text as fallback
                    Bitmap bmp = new Bitmap(70, 70);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.White);
                        using (Font drawFont = new Font("Segoe UI", 36))
                        using (SolidBrush drawBrush = new SolidBrush(Color.FromArgb(0, 120, 212)))
                        {
                            g.DrawString("🏢", drawFont, drawBrush, new PointF(15, 10));
                        }
                    }
                    logo.Image = bmp;
                }
            }
            catch { /* Use default */ }

            Label title = new Label
            {
                Text = "Property Management System",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(120, 50),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Dark Mode Toggle Button
            Button btnDarkMode = new Button
            {
                Text = "🌙",
                Location = new Point(620, 20),
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
                Location = new Point(100, 180),
                Size = new Size(500, 380),
                BackColor = Color.White
            };

            // Welcome Message
            Label lblWelcome = new Label
            {
                Text = "Welcome Back!",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 212),
                Location = new Point(50, 10),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Username Field
            Label lblUsername = new Label
            {
                Text = "USERNAME",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(50, 70),
                Size = new Size(400, 20)
            };

            txtUsername = new TextBox
            {
                Location = new Point(50, 95),
                Width = 400,
                Height = 40,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            // Password Field
            Label lblPassword = new Label
            {
                Text = "PASSWORD",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(50, 150),
                Size = new Size(400, 20)
            };

            txtPassword = new TextBox
            {
                Location = new Point(50, 175),
                Width = 400,
                Height = 40,
                Font = new Font("Segoe UI", 11),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            // ROLE SELECTION
            Label lblRole = new Label
            {
                Text = "SELECT ROLE",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(50, 230),
                Size = new Size(400, 20)
            };

            cmbRole = new ComboBox
            {
                Location = new Point(50, 255),
                Width = 400,
                Height = 35,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White
            };
            cmbRole.Items.AddRange(new string[] { "Admin", "Manager", "Staff", "Tenant" });
            cmbRole.SelectedIndex = 0;

            // Login Button
            Button btnLogin = new Button
            {
                Text = "LOGIN",
                Location = new Point(50, 310),
                Width = 400,
                Height = 48,
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
                if (txtUsername == null || txtPassword == null || cmbRole == null)
                    return;

                if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Please enter username and password.", "Login Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ShowLoader();

                System.Windows.Forms.Timer t = new System.Windows.Forms.Timer { Interval = 1500 }; // Use fully qualified name
                t.Tick += (s2, e2) =>
                {
                    t.Stop();
                    if (loaderPanel != null)
                        loaderPanel.Visible = false;

                    if (auth.Login(txtUsername.Text, txtPassword.Text))
                    {
                        string selectedRole = cmbRole.SelectedItem.ToString();
                        string userRole = auth.CurrentUser?.Role ?? "Staff";

                        // Verify role matches or allow based on permissions
                        if (selectedRole != userRole && userRole != "Admin")
                        {
                            MessageBox.Show($"You are logged in as {userRole}. Please select the correct role.",
                                "Role Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        OpenDashboard(selectedRole);
                    }
                    else
                    {
                        MessageBox.Show("Invalid credentials. Please check your username and password.",
                            "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                t.Start();
            };

            // Enter key support
            txtPassword.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                    btnLogin.PerformClick();
            };

            // Demo Credentials Label
            Label lblDemo = new Label
            {
                Text = "Demo Credentials: admin / admin123",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                Location = new Point(50, 370),
                Size = new Size(400, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            loginPanel.Controls.AddRange(new Control[] {
                lblWelcome, lblUsername, txtUsername, lblPassword, txtPassword,
                lblRole, cmbRole, btnLogin, lblDemo
            });

            // ===== LOADER PANEL =====
            loaderPanel = new Panel
            {
                Size = new Size(200, 80),
                BackColor = Color.FromArgb(240, 240, 240),
                Visible = false,
                Location = new Point(250, 320),
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
                Height = 45,
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
                loginPanel.Location = new Point((this.Width - loginPanel.Width) / 2, 180);
                if (loaderPanel != null)
                    loaderPanel.Location = new Point((this.Width - loaderPanel.Width) / 2, 320);
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

        // ===== DARK MODE TOGGLE =====
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