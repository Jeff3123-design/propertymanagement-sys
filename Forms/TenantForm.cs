using System;
using System.Windows.Forms;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Helpers;
using PropertyManagementSystem.Models;

namespace PropertyManagementSystem.Forms
{
    public partial class TenantForm : Form
    {
        private DatabaseHelper db;
        private AuthHelper auth;
        private Tenant tenant;
        private bool isEditMode = false;

        public TenantForm(DatabaseHelper database, AuthHelper authHelper, Tenant existingTenant = null)
        {
            db = database;
            auth = authHelper;
            tenant = existingTenant ?? new Tenant();
            isEditMode = existingTenant != null;
            InitializeComponent();
            LoadTenantData();
        }

        private void InitializeComponent()
        {
            this.Text = isEditMode ? "Edit Tenant" : "Add New Tenant";
            this.Size = new System.Drawing.Size(550, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblTitle = new Label
            {
                Text = isEditMode ? "Edit Tenant Details" : "Add New Tenant",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new System.Drawing.Point(150, 15),
                Size = new System.Drawing.Size(250, 30),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            int yPos = 60;
            int labelWidth = 130;
            int controlWidth = 280;
            int xPos = 110;

            // First Name
            Label lblFirstName = new Label { Text = "First Name:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtFirstName = new TextBox { Name = "txtFirstName", Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 35;

            // Last Name
            Label lblLastName = new Label { Text = "Last Name:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtLastName = new TextBox { Name = "txtLastName", Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 35;

            // Email
            Label lblEmail = new Label { Text = "Email:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtEmail = new TextBox { Name = "txtEmail", Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 35;

            // Phone
            Label lblPhone = new Label { Text = "Phone Number:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtPhone = new TextBox { Name = "txtPhone", Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 35;

            // Date of Birth
            Label lblDOB = new Label { Text = "Date of Birth:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            DateTimePicker dtpDOB = new DateTimePicker
            {
                Name = "dtpDOB",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                Format = DateTimePickerFormat.Short
            };
            dtpDOB.MaxDate = DateTime.Now.AddYears(-18);
            yPos += 35;

            // Occupation
            Label lblOccupation = new Label { Text = "Occupation:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtOccupation = new TextBox { Name = "txtOccupation", Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 35;

            // Monthly Income
            Label lblIncome = new Label { Text = "Monthly Income:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            NumericUpDown numIncome = new NumericUpDown
            {
                Name = "numIncome",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 2,
                Increment = 500
            };
            yPos += 35;

            // Emergency Contact Name
            Label lblEmergencyName = new Label { Text = "Emergency Contact:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtEmergencyName = new TextBox { Name = "txtEmergencyName", Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 35;

            // Emergency Contact Phone
            Label lblEmergencyPhone = new Label { Text = "Emergency Phone:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtEmergencyPhone = new TextBox { Name = "txtEmergencyPhone", Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 45;

            // Buttons
            Button btnSave = new Button
            {
                Text = "Save",
                Location = new Point(xPos - 50, yPos),
                Size = new Size(100, 35),
                BackColor = System.Drawing.Color.LightGreen
            };

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(xPos + 60, yPos),
                Size = new Size(100, 35)
            };

            btnSave.Click += (s, e) => {
                if (ValidateForm(txtFirstName, txtLastName, txtEmail, txtPhone, dtpDOB))
                {
                    tenant.FirstName = txtFirstName.Text;
                    tenant.LastName = txtLastName.Text;
                    tenant.Email = txtEmail.Text;
                    tenant.PhoneNumber = txtPhone.Text;
                    tenant.DateOfBirth = dtpDOB.Value;
                    tenant.Occupation = txtOccupation.Text;
                    tenant.MonthlyIncome = numIncome.Value;
                    tenant.EmergencyContactName = txtEmergencyName.Text;
                    tenant.EmergencyContactPhone = txtEmergencyPhone.Text;

                    if (isEditMode)
                    {
                        db.UpdateTenant(tenant);
                        MessageBox.Show("Tenant updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        db.AddTenant(tenant);
                        MessageBox.Show("Tenant added successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };

            btnCancel.Click += (s, e) => {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            Label lblRequired = new Label
            {
                Text = "* Required fields",
                Location = new Point(50, yPos + 50),
                Size = new Size(150, 20),
                ForeColor = System.Drawing.Color.Gray,
                Font = new Font("Arial", 8)
            };

            this.Controls.AddRange(new Control[] {
                lblTitle, lblFirstName, txtFirstName, lblLastName, txtLastName,
                lblEmail, txtEmail, lblPhone, txtPhone, lblDOB, dtpDOB,
                lblOccupation, txtOccupation, lblIncome, numIncome,
                lblEmergencyName, txtEmergencyName, lblEmergencyPhone, txtEmergencyPhone,
                btnSave, btnCancel, lblRequired
            });
        }

        private void LoadTenantData()
        {
            if (isEditMode && tenant != null)
            {
                var txtFirstName = this.Controls["txtFirstName"] as TextBox;
                var txtLastName = this.Controls["txtLastName"] as TextBox;
                var txtEmail = this.Controls["txtEmail"] as TextBox;
                var txtPhone = this.Controls["txtPhone"] as TextBox;
                var txtOccupation = this.Controls["txtOccupation"] as TextBox;
                var txtEmergencyName = this.Controls["txtEmergencyName"] as TextBox;
                var txtEmergencyPhone = this.Controls["txtEmergencyPhone"] as TextBox;
                var dtpDOB = this.Controls["dtpDOB"] as DateTimePicker;
                var numIncome = this.Controls["numIncome"] as NumericUpDown;

                if (txtFirstName != null) txtFirstName.Text = tenant.FirstName ?? "";
                if (txtLastName != null) txtLastName.Text = tenant.LastName ?? "";
                if (txtEmail != null) txtEmail.Text = tenant.Email ?? "";
                if (txtPhone != null) txtPhone.Text = tenant.PhoneNumber ?? "";
                if (txtOccupation != null) txtOccupation.Text = tenant.Occupation ?? "";
                if (txtEmergencyName != null) txtEmergencyName.Text = tenant.EmergencyContactName ?? "";
                if (txtEmergencyPhone != null) txtEmergencyPhone.Text = tenant.EmergencyContactPhone ?? "";
                if (dtpDOB != null) dtpDOB.Value = tenant.DateOfBirth;
                if (numIncome != null) numIncome.Value = tenant.MonthlyIncome;
            }
        }

        private bool ValidateForm(TextBox firstName, TextBox lastName, TextBox email, TextBox phone, DateTimePicker dob)
        {
            if (string.IsNullOrWhiteSpace(firstName.Text))
            {
                MessageBox.Show("First Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(lastName.Text))
            {
                MessageBox.Show("Last Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lastName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(email.Text) || !email.Text.Contains("@"))
            {
                MessageBox.Show("Valid email address is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                email.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(phone.Text))
            {
                MessageBox.Show("Phone number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                phone.Focus();
                return false;
            }

            if (dob.Value > DateTime.Now.AddYears(-18))
            {
                MessageBox.Show("Tenant must be at least 18 years old.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dob.Focus();
                return false;
            }

            return true;
        }
    }
}