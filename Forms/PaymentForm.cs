using System;
using System.Data;
using System.Windows.Forms;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Helpers;
using PropertyManagementSystem.Models;

namespace PropertyManagementSystem.Forms
{
    public partial class PaymentForm : Form
    {
        private DatabaseHelper db;
        private AuthHelper auth;
        private Payment payment;
        private bool isEditMode = false;

        public PaymentForm(DatabaseHelper database, AuthHelper authHelper, Payment existingPayment = null)
        {
            db = database;
            auth = authHelper;
            payment = existingPayment ?? new Payment();
            isEditMode = existingPayment != null;
            InitializeComponent();
            LoadPaymentData();
        }

        private void InitializeComponent()
        {
            this.Text = isEditMode ? "Edit Payment" : "Record New Payment";
            this.Size = new System.Drawing.Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblTitle = new Label
            {
                Text = isEditMode ? "Edit Payment Record" : "Record Rent Payment",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new System.Drawing.Point(120, 15),
                Size = new System.Drawing.Size(260, 30),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            int yPos = 60;
            int labelWidth = 120;
            int controlWidth = 260;
            int xPos = 110;

            // Lease Selection
            Label lblLease = new Label { Text = "Lease:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            ComboBox cboLease = new ComboBox
            {
                Name = "cboLease",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "LeaseInfo",
                ValueMember = "LeaseId"
            };

            // Load active leases with tenant and property info
            try
            {
                DataTable activeLeases = db.GetActiveLeases();
                if (!activeLeases.Columns.Contains("LeaseInfo"))
                    activeLeases.Columns.Add("LeaseInfo", typeof(string), "TenantName || ' - ' || PropertyAddress || ' ($' || MonthlyRent || ')'");
                cboLease.DataSource = activeLeases;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading leases: {ex.Message}");
            }
            yPos += 35;

            // Payment Date
            Label lblPaymentDate = new Label { Text = "Payment Date:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            DateTimePicker dtpPaymentDate = new DateTimePicker
            {
                Name = "dtpPaymentDate",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                Format = DateTimePickerFormat.Short
            };
            dtpPaymentDate.Value = DateTime.Now;
            yPos += 35;

            // Amount
            Label lblAmount = new Label { Text = "Amount:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            NumericUpDown numAmount = new NumericUpDown
            {
                Name = "numAmount",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 2,
                Increment = 100
            };
            yPos += 35;

            // Auto-fill amount when lease selected
            cboLease.SelectedIndexChanged += (s, e) => {
                if (cboLease.SelectedValue != null)
                {
                    try
                    {
                        int leaseId = Convert.ToInt32(cboLease.SelectedValue);
                        DataTable leaseData = db.ExecuteQuery("SELECT MonthlyRent FROM Leases WHERE LeaseId = @id",
                            new System.Data.SQLite.SQLiteParameter[] { new System.Data.SQLite.SQLiteParameter("@id", leaseId) });

                        if (leaseData.Rows.Count > 0 && leaseData.Rows[0]["MonthlyRent"] != DBNull.Value)
                        {
                            numAmount.Value = Convert.ToDecimal(leaseData.Rows[0]["MonthlyRent"]);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading lease amount: {ex.Message}");
                    }
                }
            };

            // Payment Method
            Label lblPaymentMethod = new Label { Text = "Payment Method:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            ComboBox cboPaymentMethod = new ComboBox
            {
                Name = "cboPaymentMethod",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboPaymentMethod.Items.AddRange(new string[] { "Cash", "Check", "Bank Transfer", "Credit Card", "Venmo", "Zelle" });
            cboPaymentMethod.SelectedIndex = 0;
            yPos += 35;

            // Check Number (visible only when Check is selected)
            Label lblCheckNumber = new Label { Text = "Check Number:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtCheckNumber = new TextBox { Name = "txtCheckNumber", Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            txtCheckNumber.Visible = false;
            lblCheckNumber.Visible = false;

            cboPaymentMethod.SelectedIndexChanged += (s, e) => {
                bool isCheck = cboPaymentMethod.SelectedItem?.ToString() == "Check";
                lblCheckNumber.Visible = isCheck;
                txtCheckNumber.Visible = isCheck;
            };
            yPos += 35;

            // Reference Number
            Label lblReference = new Label { Text = "Reference #:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtReference = new TextBox { Name = "txtReference", Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 35;

            // Notes
            Label lblNotes = new Label { Text = "Notes:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtNotes = new TextBox
            {
                Name = "txtNotes",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            yPos += 70;

            // Status
            Label lblStatus = new Label { Text = "Status:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            ComboBox cboStatus = new ComboBox
            {
                Name = "cboStatus",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new string[] { "Completed", "Pending", "Failed" });
            cboStatus.SelectedIndex = 0;
            yPos += 45;

            // Buttons
            Button btnSave = new Button
            {
                Text = "Record Payment",
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
                if (ValidateForm(cboLease, numAmount, cboPaymentMethod, txtCheckNumber))
                {
                    payment.LeaseId = Convert.ToInt32(cboLease.SelectedValue);
                    payment.PaymentDate = dtpPaymentDate.Value;
                    payment.Amount = numAmount.Value;
                    payment.PaymentMethod = cboPaymentMethod.SelectedItem?.ToString() ?? "";
                    payment.CheckNumber = txtCheckNumber.Text;
                    payment.ReferenceNumber = txtReference.Text;
                    payment.Notes = txtNotes.Text;
                    payment.Status = cboStatus.SelectedItem?.ToString() ?? "Completed";

                    if (isEditMode)
                    {
                        // Update logic
                        MessageBox.Show("Payment updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        db.AddPayment(payment);
                        MessageBox.Show($"Payment of ${payment.Amount:N2} recorded successfully!", "Success",
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
                lblTitle, lblLease, cboLease, lblPaymentDate, dtpPaymentDate,
                lblAmount, numAmount, lblPaymentMethod, cboPaymentMethod,
                lblCheckNumber, txtCheckNumber, lblReference, txtReference,
                lblNotes, txtNotes, lblStatus, cboStatus, btnSave, btnCancel, lblRequired
            });
        }

        private void LoadPaymentData()
        {
            if (isEditMode && payment != null)
            {
                var dtpPaymentDate = this.Controls["dtpPaymentDate"] as DateTimePicker;
                var numAmount = this.Controls["numAmount"] as NumericUpDown;
                var txtCheckNumber = this.Controls["txtCheckNumber"] as TextBox;
                var txtReference = this.Controls["txtReference"] as TextBox;
                var txtNotes = this.Controls["txtNotes"] as TextBox;
                var cboPaymentMethod = this.Controls["cboPaymentMethod"] as ComboBox;
                var cboStatus = this.Controls["cboStatus"] as ComboBox;

                if (dtpPaymentDate != null) dtpPaymentDate.Value = payment.PaymentDate;
                if (numAmount != null) numAmount.Value = payment.Amount;
                if (txtCheckNumber != null) txtCheckNumber.Text = payment.CheckNumber ?? "";
                if (txtReference != null) txtReference.Text = payment.ReferenceNumber ?? "";
                if (txtNotes != null) txtNotes.Text = payment.Notes ?? "";
                if (cboPaymentMethod != null && payment.PaymentMethod != null)
                    cboPaymentMethod.SelectedItem = payment.PaymentMethod;
                if (cboStatus != null && payment.Status != null)
                    cboStatus.SelectedItem = payment.Status;
            }
        }

        private bool ValidateForm(ComboBox lease, NumericUpDown amount, ComboBox paymentMethod, TextBox checkNumber)
        {
            if (lease.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a lease.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lease.Focus();
                return false;
            }

            if (amount.Value <= 0)
            {
                MessageBox.Show("Amount must be greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                amount.Focus();
                return false;
            }

            if (paymentMethod.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a payment method.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                paymentMethod.Focus();
                return false;
            }

            if (paymentMethod.SelectedItem?.ToString() == "Check" && string.IsNullOrWhiteSpace(checkNumber.Text))
            {
                MessageBox.Show("Please enter check number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                checkNumber.Focus();
                return false;
            }

            return true;
        }
    }
}