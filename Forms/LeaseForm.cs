using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Helpers;
using PropertyManagementSystem.Models;

namespace PropertyManagementSystem.Forms
{
    public partial class LeaseForm : Form
    {
        private DatabaseHelper db;
        private AuthHelper auth;
        private Lease lease;
        private bool isEditMode = false;

        public LeaseForm(DatabaseHelper database, AuthHelper authHelper, Lease existingLease = null)
        {
            db = database;
            auth = authHelper;
            lease = existingLease ?? new Lease();
            isEditMode = existingLease != null;
            InitializeComponent();
            LoadLeaseData();
        }

        private void InitializeComponent()
        {
            this.Text = isEditMode ? "Edit Lease" : "Create New Lease";
            this.Size = new System.Drawing.Size(550, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblTitle = new Label
            {
                Text = isEditMode ? "Edit Lease Agreement" : "Create New Lease Agreement",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new System.Drawing.Point(120, 15),
                Size = new System.Drawing.Size(300, 30),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            int yPos = 60;
            int labelWidth = 120;
            int controlWidth = 280;
            int xPos = 110;

            // Property Selection
            Label lblProperty = new Label { Text = "Property:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            ComboBox cboProperty = new ComboBox
            {
                Name = "cboProperty",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Address",
                ValueMember = "PropertyId"
            };

            // Load available properties
            try
            {
                DataTable properties = db.GetAvailableProperties();
                cboProperty.DataSource = properties;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading properties: {ex.Message}");
            }
            yPos += 35;

            // Tenant Selection
            Label lblTenant = new Label { Text = "Tenant:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            ComboBox cboTenant = new ComboBox
            {
                Name = "cboTenant",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "FullName",
                ValueMember = "TenantId"
            };

            // Load all tenants
            try
            {
                DataTable tenants = db.GetAllTenants();
                // Add FullName column for display
                if (!tenants.Columns.Contains("FullName"))
                    tenants.Columns.Add("FullName", typeof(string), "FirstName || ' ' || LastName");
                cboTenant.DataSource = tenants;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading tenants: {ex.Message}");
            }
            yPos += 35;

            // Start Date
            Label lblStartDate = new Label { Text = "Start Date:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            DateTimePicker dtpStartDate = new DateTimePicker
            {
                Name = "dtpStartDate",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                Format = DateTimePickerFormat.Short
            };
            dtpStartDate.Value = DateTime.Now;
            yPos += 35;

            // End Date
            Label lblEndDate = new Label { Text = "End Date:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            DateTimePicker dtpEndDate = new DateTimePicker
            {
                Name = "dtpEndDate",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                Format = DateTimePickerFormat.Short
            };
            dtpEndDate.Value = DateTime.Now.AddYears(1);
            yPos += 35;

            // Monthly Rent
            Label lblMonthlyRent = new Label { Text = "Monthly Rent:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            NumericUpDown numMonthlyRent = new NumericUpDown
            {
                Name = "numMonthlyRent",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 2,
                Increment = 100
            };
            yPos += 35;

            // Security Deposit Paid
            Label lblSecurityDeposit = new Label { Text = "Security Deposit:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            NumericUpDown numSecurityDeposit = new NumericUpDown
            {
                Name = "numSecurityDeposit",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 2,
                Increment = 100
            };
            yPos += 35;

            // Status
            Label lblStatus = new Label { Text = "Status:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            ComboBox cboStatus = new ComboBox
            {
                Name = "cboStatus",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new string[] { "Active", "Expired", "Terminated" });
            cboStatus.SelectedIndex = 0;
            yPos += 35;

            // Terms
            Label lblTerms = new Label { Text = "Terms & Conditions:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtTerms = new TextBox
            {
                Name = "txtTerms",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            txtTerms.Text = "Standard lease terms apply. Tenant agrees to maintain the property in good condition and pay rent on time.";
            yPos += 70;

            // Auto-fill rent when property selected
            cboProperty.SelectedIndexChanged += (s, e) => {
                if (cboProperty.SelectedValue != null)
                {
                    try
                    {
                        int propertyId = Convert.ToInt32(cboProperty.SelectedValue);
                        DataTable propData = db.ExecuteQuery("SELECT MonthlyRent, SecurityDeposit FROM Properties WHERE PropertyId = @id",
                            new SqliteParameter[] { new SqliteParameter("@id", propertyId) });

                        if (propData.Rows.Count > 0 && propData.Rows[0]["MonthlyRent"] != DBNull.Value)
                        {
                            numMonthlyRent.Value = Convert.ToDecimal(propData.Rows[0]["MonthlyRent"]);
                            if (propData.Rows[0]["SecurityDeposit"] != DBNull.Value)
                                numSecurityDeposit.Value = Convert.ToDecimal(propData.Rows[0]["SecurityDeposit"]);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading property rent: {ex.Message}");
                    }
                }
            };

            // Buttons
            Button btnSave = new Button
            {
                Text = "Create Lease",
                Location = new Point(xPos - 50, yPos),
                Size = new Size(120, 35),
                BackColor = System.Drawing.Color.LightGreen
            };

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(xPos + 80, yPos),
                Size = new Size(100, 35)
            };

            btnSave.Click += (s, e) => {
                if (ValidateForm(cboProperty, cboTenant, dtpStartDate, dtpEndDate, numMonthlyRent))
                {
                    lease.PropertyId = Convert.ToInt32(cboProperty.SelectedValue);
                    lease.TenantId = Convert.ToInt32(cboTenant.SelectedValue);
                    lease.StartDate = dtpStartDate.Value;
                    lease.EndDate = dtpEndDate.Value;
                    lease.MonthlyRent = numMonthlyRent.Value;
                    lease.SecurityDepositPaid = numSecurityDeposit.Value;
                    lease.Status = cboStatus.SelectedItem?.ToString() ?? "Active";
                    lease.Terms = txtTerms.Text;

                    if (isEditMode)
                    {
                        // Update logic here
                        MessageBox.Show("Lease updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        db.AddLease(lease);
                        MessageBox.Show("Lease created successfully!", "Success",
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
                lblTitle, lblProperty, cboProperty, lblTenant, cboTenant,
                lblStartDate, dtpStartDate, lblEndDate, dtpEndDate,
                lblMonthlyRent, numMonthlyRent, lblSecurityDeposit, numSecurityDeposit,
                lblStatus, cboStatus, lblTerms, txtTerms, btnSave, btnCancel, lblRequired
            });
        }

        private void LoadLeaseData()
        {
            if (isEditMode && lease != null)
            {
                var dtpStartDate = this.Controls["dtpStartDate"] as DateTimePicker;
                var dtpEndDate = this.Controls["dtpEndDate"] as DateTimePicker;
                var numMonthlyRent = this.Controls["numMonthlyRent"] as NumericUpDown;
                var numSecurityDeposit = this.Controls["numSecurityDeposit"] as NumericUpDown;
                var cboStatus = this.Controls["cboStatus"] as ComboBox;
                var txtTerms = this.Controls["txtTerms"] as TextBox;

                if (dtpStartDate != null) dtpStartDate.Value = lease.StartDate;
                if (dtpEndDate != null) dtpEndDate.Value = lease.EndDate;
                if (numMonthlyRent != null) numMonthlyRent.Value = lease.MonthlyRent;
                if (numSecurityDeposit != null) numSecurityDeposit.Value = lease.SecurityDepositPaid;
                if (cboStatus != null && lease.Status != null) cboStatus.SelectedItem = lease.Status;
                if (txtTerms != null) txtTerms.Text = lease.Terms ?? "";
            }
        }

        private bool ValidateForm(ComboBox property, ComboBox tenant, DateTimePicker startDate, DateTimePicker endDate, NumericUpDown monthlyRent)
        {
            if (property.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a property.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                property.Focus();
                return false;
            }

            if (tenant.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a tenant.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tenant.Focus();
                return false;
            }

            if (startDate.Value >= endDate.Value)
            {
                MessageBox.Show("End date must be after start date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                endDate.Focus();
                return false;
            }

            if (monthlyRent.Value <= 0)
            {
                MessageBox.Show("Monthly rent must be greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                monthlyRent.Focus();
                return false;
            }

            return true;
        }
    }
}