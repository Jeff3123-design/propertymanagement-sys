using System;
using System.Windows.Forms;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Helpers;
using PropertyManagementSystem.Models;

namespace PropertyManagementSystem.Forms
{
    public partial class PropertyForm : Form
    {
        private DatabaseHelper db;
        private AuthHelper auth;
        private Property property;
        private bool isEditMode = false;

        public PropertyForm(DatabaseHelper database, AuthHelper authHelper, Property existingProperty = null)
        {
            db = database;
            auth = authHelper;
            property = existingProperty ?? new Property();
            isEditMode = existingProperty != null;
            InitializeComponent();
            LoadPropertyData();
        }

        private void InitializeComponent()
        {
            this.Text = isEditMode ? "Edit Property" : "Add New Property";
            this.Size = new System.Drawing.Size(550, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label lblTitle = new Label
            {
                Text = isEditMode ? "Edit Property Details" : "Add New Property",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new System.Drawing.Point(150, 15),
                Size = new System.Drawing.Size(250, 30),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            int yPos = 60;
            int labelWidth = 120;
            int controlWidth = 300;
            int xPos = 100;

            // Address
            Label lblAddress = new Label { Text = "Address:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtAddress = new TextBox { Name = "txtAddress", Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 35;

            // City
            Label lblCity = new Label { Text = "City:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtCity = new TextBox { Name = "txtCity", Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 35;

            // Postal Code
            Label lblPostalCode = new Label { Text = "Postal Code:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtPostalCode = new TextBox { Name = "txtPostalCode", Location = new Point(xPos, yPos), Size = new Size(controlWidth, 25) };
            yPos += 35;

            // Property Type
            Label lblPropertyType = new Label { Text = "Property Type:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            ComboBox cboPropertyType = new ComboBox
            {
                Name = "cboPropertyType",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboPropertyType.Items.AddRange(new string[] { "Apartment", "House", "Condo", "Townhouse", "Commercial", "Land" });
            yPos += 35;

            // Bedrooms
            Label lblBedrooms = new Label { Text = "Bedrooms:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            NumericUpDown numBedrooms = new NumericUpDown
            {
                Name = "numBedrooms",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                Minimum = 0,
                Maximum = 10
            };
            yPos += 35;

            // Bathrooms
            Label lblBathrooms = new Label { Text = "Bathrooms:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            NumericUpDown numBathrooms = new NumericUpDown
            {
                Name = "numBathrooms",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                Minimum = 0,
                Maximum = 10,
                DecimalPlaces = 1,
                Increment = 0.5M
            };
            yPos += 35;

            // Square Footage
            Label lblSqFt = new Label { Text = "Square Footage:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            NumericUpDown numSqFt = new NumericUpDown
            {
                Name = "numSqFt",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                Minimum = 0,
                Maximum = 10000,
                Increment = 50
            };
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

            // Security Deposit
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
            Label lblStatus = new Label { Text = "Status:*", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            ComboBox cboStatus = new ComboBox
            {
                Name = "cboStatus",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new string[] { "Available", "Rented", "Maintenance", "Pending" });
            yPos += 35;

            // Description
            Label lblDescription = new Label { Text = "Description:", Location = new Point(50, yPos), Size = new Size(labelWidth, 25) };
            TextBox txtDescription = new TextBox
            {
                Name = "txtDescription",
                Location = new Point(xPos, yPos),
                Size = new Size(controlWidth, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            yPos += 70;

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
                if (ValidateForm(txtAddress, txtCity, txtPostalCode, cboPropertyType, numMonthlyRent, cboStatus))
                {
                    property.Address = txtAddress.Text;
                    property.City = txtCity.Text;
                    property.PostalCode = txtPostalCode.Text;
                    property.PropertyType = cboPropertyType.SelectedItem?.ToString() ?? "";
                    property.Bedrooms = (int)numBedrooms.Value;
                    property.Bathrooms = Convert.ToDouble(numBathrooms.Value);
                    property.SquareFootage = Convert.ToDouble(numSqFt.Value);
                    property.MonthlyRent = numMonthlyRent.Value;
                    property.SecurityDeposit = numSecurityDeposit.Value;
                    property.Status = cboStatus.SelectedItem?.ToString() ?? "Available";
                    property.Description = txtDescription.Text;

                    if (isEditMode)
                    {
                        db.UpdateProperty(property);
                        MessageBox.Show("Property updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        db.AddProperty(property);
                        MessageBox.Show("Property added successfully!", "Success",
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

            // Required fields note
            Label lblRequired = new Label
            {
                Text = "* Required fields",
                Location = new Point(50, yPos + 50),
                Size = new Size(150, 20),
                ForeColor = System.Drawing.Color.Gray,
                Font = new Font("Arial", 8)
            };

            this.Controls.AddRange(new Control[] {
                lblTitle, lblAddress, txtAddress, lblCity, txtCity, lblPostalCode, txtPostalCode,
                lblPropertyType, cboPropertyType, lblBedrooms, numBedrooms, lblBathrooms, numBathrooms,
                lblSqFt, numSqFt, lblMonthlyRent, numMonthlyRent, lblSecurityDeposit, numSecurityDeposit,
                lblStatus, cboStatus, lblDescription, txtDescription, btnSave, btnCancel, lblRequired
            });
        }

        private void LoadPropertyData()
        {
            if (isEditMode && property != null)
            {
                // Find controls by name
                var txtAddress = this.Controls["txtAddress"] as TextBox;
                var txtCity = this.Controls["txtCity"] as TextBox;
                var txtPostalCode = this.Controls["txtPostalCode"] as TextBox;
                var txtDescription = this.Controls["txtDescription"] as TextBox;
                var cboPropertyType = this.Controls["cboPropertyType"] as ComboBox;
                var cboStatus = this.Controls["cboStatus"] as ComboBox;
                var numBedrooms = this.Controls["numBedrooms"] as NumericUpDown;
                var numBathrooms = this.Controls["numBathrooms"] as NumericUpDown;
                var numSqFt = this.Controls["numSqFt"] as NumericUpDown;
                var numMonthlyRent = this.Controls["numMonthlyRent"] as NumericUpDown;
                var numSecurityDeposit = this.Controls["numSecurityDeposit"] as NumericUpDown;

                if (txtAddress != null) txtAddress.Text = property.Address ?? "";
                if (txtCity != null) txtCity.Text = property.City ?? "";
                if (txtPostalCode != null) txtPostalCode.Text = property.PostalCode ?? "";
                if (txtDescription != null) txtDescription.Text = property.Description ?? "";
                if (cboPropertyType != null && property.PropertyType != null)
                    cboPropertyType.SelectedItem = property.PropertyType;
                if (cboStatus != null && property.Status != null)
                    cboStatus.SelectedItem = property.Status;
                if (numBedrooms != null) numBedrooms.Value = property.Bedrooms;
                if (numBathrooms != null) numBathrooms.Value = (decimal)property.Bathrooms;
                if (numSqFt != null) numSqFt.Value = (decimal)property.SquareFootage;
                if (numMonthlyRent != null) numMonthlyRent.Value = property.MonthlyRent;
                if (numSecurityDeposit != null) numSecurityDeposit.Value = property.SecurityDeposit;
            }
            else
            {
                // Set default values for new property
                var cboStatus = this.Controls["cboStatus"] as ComboBox;
                var cboPropertyType = this.Controls["cboPropertyType"] as ComboBox;

                if (cboStatus != null) cboStatus.SelectedIndex = 0;
                if (cboPropertyType != null) cboPropertyType.SelectedIndex = 0;
            }
        }

        private bool ValidateForm(TextBox address, TextBox city, TextBox postalCode, ComboBox propertyType, NumericUpDown monthlyRent, ComboBox status)
        {
            if (string.IsNullOrWhiteSpace(address.Text))
            {
                MessageBox.Show("Address is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                address.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(city.Text))
            {
                MessageBox.Show("City is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                city.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(postalCode.Text))
            {
                MessageBox.Show("Postal Code is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                postalCode.Focus();
                return false;
            }

            if (propertyType.SelectedIndex == -1)
            {
                MessageBox.Show("Property Type is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                propertyType.Focus();
                return false;
            }

            if (monthlyRent.Value <= 0)
            {
                MessageBox.Show("Monthly Rent must be greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                monthlyRent.Focus();
                return false;
            }

            if (status.SelectedIndex == -1)
            {
                MessageBox.Show("Status is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                status.Focus();
                return false;
            }

            return true;
        }
    }
}