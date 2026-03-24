using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using PropertyManagementSystem.Models;
using PropertyManagementSystem.Helpers;

namespace PropertyManagementSystem.Data
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            string dbPath = Path.Combine(Application.StartupPath, "PropertyManagement.db");
            connectionString = $"Data Source={dbPath}";

            try
            {
                InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization error: {ex.Message}\n\nPath: {dbPath}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void InitializeDatabase()
        {
            string dbPath = Path.Combine(Application.StartupPath, "PropertyManagement.db");

            if (!File.Exists(dbPath))
            {
                CreateTables();
                CreateDefaultAdminUser();
                InsertSampleData();
            }
        }

        private void CreateTables()
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                // Users table
                string createUsers = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Email TEXT NOT NULL,
                        PasswordHash TEXT NOT NULL,
                        FullName TEXT,
                        Role TEXT,
                        IsActive INTEGER,
                        CreatedDate DATETIME,
                        LastLoginDate DATETIME
                    )";

                // Properties table
                string createProperties = @"
                    CREATE TABLE IF NOT EXISTS Properties (
                        PropertyId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Address TEXT NOT NULL,
                        City TEXT NOT NULL,
                        PostalCode TEXT NOT NULL,
                        PropertyType TEXT NOT NULL,
                        Bedrooms INTEGER,
                        Bathrooms REAL,
                        SquareFootage REAL,
                        MonthlyRent REAL NOT NULL,
                        SecurityDeposit REAL,
                        Status TEXT,
                        Description TEXT,
                        DateAdded DATETIME
                    )";

                // Tenants table
                string createTenants = @"
                    CREATE TABLE IF NOT EXISTS Tenants (
                        TenantId INTEGER PRIMARY KEY AUTOINCREMENT,
                        FirstName TEXT NOT NULL,
                        LastName TEXT NOT NULL,
                        Email TEXT NOT NULL,
                        PhoneNumber TEXT NOT NULL,
                        EmergencyContactName TEXT,
                        EmergencyContactPhone TEXT,
                        DateOfBirth DATETIME,
                        Occupation TEXT,
                        MonthlyIncome REAL,
                        DateAdded DATETIME
                    )";

                // Leases table
                string createLeases = @"
                    CREATE TABLE IF NOT EXISTS Leases (
                        LeaseId INTEGER PRIMARY KEY AUTOINCREMENT,
                        PropertyId INTEGER NOT NULL,
                        TenantId INTEGER NOT NULL,
                        StartDate DATETIME NOT NULL,
                        EndDate DATETIME NOT NULL,
                        MonthlyRent REAL,
                        SecurityDepositPaid REAL,
                        Status TEXT,
                        Terms TEXT,
                        DateCreated DATETIME,
                        FOREIGN KEY(PropertyId) REFERENCES Properties(PropertyId),
                        FOREIGN KEY(TenantId) REFERENCES Tenants(TenantId)
                    )";

                // Payments table
                string createPayments = @"
                    CREATE TABLE IF NOT EXISTS Payments (
                        PaymentId INTEGER PRIMARY KEY AUTOINCREMENT,
                        LeaseId INTEGER NOT NULL,
                        PaymentDate DATETIME NOT NULL,
                        Amount REAL NOT NULL,
                        PaymentMethod TEXT,
                        CheckNumber TEXT,
                        ReferenceNumber TEXT,
                        Notes TEXT,
                        Status TEXT,
                        FOREIGN KEY(LeaseId) REFERENCES Leases(LeaseId)
                    )";

                using (var cmd = new SqliteCommand(createUsers, conn))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SqliteCommand(createProperties, conn))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SqliteCommand(createTenants, conn))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SqliteCommand(createLeases, conn))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SqliteCommand(createPayments, conn))
                    cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

        private void CreateDefaultAdminUser()
        {
            try
            {
                string hashedPassword = AuthHelper.HashPassword("admin123");
                string query = @"INSERT INTO Users (Username, Email, PasswordHash, FullName, Role, IsActive, CreatedDate)
                                VALUES (@username, @email, @password, @fullName, @role, @isActive, @createdDate)";

                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqliteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", "admin");
                        cmd.Parameters.AddWithValue("@email", "admin@property.com");
                        cmd.Parameters.AddWithValue("@password", hashedPassword);
                        cmd.Parameters.AddWithValue("@fullName", "System Administrator");
                        cmd.Parameters.AddWithValue("@role", "Admin");
                        cmd.Parameters.AddWithValue("@isActive", 1);
                        cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating admin user: {ex.Message}");
            }
        }

        private void InsertSampleData()
        {
            try
            {
                // Insert sample properties
                var properties = new List<Property>
                {
                    new Property { Address = "123 Main St", City = "Springfield", PostalCode = "62701",
                        PropertyType = "Apartment", Bedrooms = 2, Bathrooms = 1, SquareFootage = 850,
                        MonthlyRent = 1200, SecurityDeposit = 1200, Description = "Cozy 2-bedroom apartment" },

                    new Property { Address = "456 Oak Ave", City = "Springfield", PostalCode = "62702",
                        PropertyType = "House", Bedrooms = 3, Bathrooms = 2, SquareFootage = 1500,
                        MonthlyRent = 1800, SecurityDeposit = 1800, Description = "Spacious family home" }
                };

                foreach (var prop in properties)
                {
                    AddProperty(prop);
                }

                // Insert sample tenants
                var tenants = new List<Tenant>
                {
                    new Tenant { FirstName = "John", LastName = "Doe", Email = "john@example.com",
                        PhoneNumber = "555-0101", EmergencyContactName = "Jane Doe",
                        EmergencyContactPhone = "555-0102", DateOfBirth = new DateTime(1985, 5, 15),
                        Occupation = "Software Developer", MonthlyIncome = 5000 },

                    new Tenant { FirstName = "Sarah", LastName = "Smith", Email = "sarah@example.com",
                        PhoneNumber = "555-0103", EmergencyContactName = "Mike Smith",
                        EmergencyContactPhone = "555-0104", DateOfBirth = new DateTime(1990, 8, 22),
                        Occupation = "Teacher", MonthlyIncome = 4200 }
                };

                foreach (var tenant in tenants)
                {
                    AddTenant(tenant);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inserting sample data: {ex.Message}");
            }
        }

        // Generic Execute Methods
        public DataTable ExecuteQuery(string query, SqliteParameter[] parameters = null)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqliteCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Query error: {ex.Message}\n{query}");
                throw;
            }
            return dt;
        }

        public object ExecuteScalar(string query, SqliteParameter[] parameters = null)
        {
            try
            {
                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqliteCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Scalar error: {ex.Message}\n{query}");
                throw;
            }
        }

        public int ExecuteNonQuery(string query, SqliteParameter[] parameters = null)
        {
            try
            {
                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqliteCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NonQuery error: {ex.Message}\n{query}");
                throw;
            }
        }

        // Property CRUD
        public void AddProperty(Property property)
        {
            string sql = @"INSERT INTO Properties (Address, City, PostalCode, PropertyType, 
                          Bedrooms, Bathrooms, SquareFootage, MonthlyRent, SecurityDeposit, 
                          Status, Description, DateAdded)
                          VALUES (@Address, @City, @PostalCode, @PropertyType, @Bedrooms, 
                          @Bathrooms, @SquareFootage, @MonthlyRent, @SecurityDeposit, 
                          @Status, @Description, @DateAdded)";

            var parameters = new SqliteParameter[]
            {
                new SqliteParameter("@Address", property.Address ?? ""),
                new SqliteParameter("@City", property.City ?? ""),
                new SqliteParameter("@PostalCode", property.PostalCode ?? ""),
                new SqliteParameter("@PropertyType", property.PropertyType ?? ""),
                new SqliteParameter("@Bedrooms", property.Bedrooms),
                new SqliteParameter("@Bathrooms", property.Bathrooms),
                new SqliteParameter("@SquareFootage", property.SquareFootage),
                new SqliteParameter("@MonthlyRent", property.MonthlyRent),
                new SqliteParameter("@SecurityDeposit", property.SecurityDeposit),
                new SqliteParameter("@Status", property.Status ?? "Available"),
                new SqliteParameter("@Description", property.Description ?? ""),
                new SqliteParameter("@DateAdded", property.DateAdded)
            };

            ExecuteNonQuery(sql, parameters);
        }

        public DataTable GetAllProperties()
        {
            return ExecuteQuery("SELECT * FROM Properties ORDER BY PropertyId DESC");
        }

        public DataTable GetAvailableProperties()
        {
            return ExecuteQuery("SELECT * FROM Properties WHERE Status = 'Available' ORDER BY PropertyId DESC");
        }

        public void UpdateProperty(Property property)
        {
            string sql = @"UPDATE Properties SET Address=@Address, City=@City, PostalCode=@PostalCode,
                          PropertyType=@PropertyType, Bedrooms=@Bedrooms, Bathrooms=@Bathrooms,
                          SquareFootage=@SquareFootage, MonthlyRent=@MonthlyRent,
                          SecurityDeposit=@SecurityDeposit, Status=@Status, Description=@Description
                          WHERE PropertyId=@PropertyId";

            var parameters = new SqliteParameter[]
            {
                new SqliteParameter("@PropertyId", property.PropertyId),
                new SqliteParameter("@Address", property.Address ?? ""),
                new SqliteParameter("@City", property.City ?? ""),
                new SqliteParameter("@PostalCode", property.PostalCode ?? ""),
                new SqliteParameter("@PropertyType", property.PropertyType ?? ""),
                new SqliteParameter("@Bedrooms", property.Bedrooms),
                new SqliteParameter("@Bathrooms", property.Bathrooms),
                new SqliteParameter("@SquareFootage", property.SquareFootage),
                new SqliteParameter("@MonthlyRent", property.MonthlyRent),
                new SqliteParameter("@SecurityDeposit", property.SecurityDeposit),
                new SqliteParameter("@Status", property.Status ?? "Available"),
                new SqliteParameter("@Description", property.Description ?? "")
            };

            ExecuteNonQuery(sql, parameters);
        }

        public void DeleteProperty(int propertyId)
        {
            string sql = "DELETE FROM Properties WHERE PropertyId=@PropertyId";
            var parameters = new SqliteParameter[] { new SqliteParameter("@PropertyId", propertyId) };
            ExecuteNonQuery(sql, parameters);
        }

        // Tenant CRUD
        public void AddTenant(Tenant tenant)
        {
            string sql = @"INSERT INTO Tenants (FirstName, LastName, Email, PhoneNumber, 
                          EmergencyContactName, EmergencyContactPhone, DateOfBirth, 
                          Occupation, MonthlyIncome, DateAdded)
                          VALUES (@FirstName, @LastName, @Email, @PhoneNumber, 
                          @EmergencyContactName, @EmergencyContactPhone, @DateOfBirth, 
                          @Occupation, @MonthlyIncome, @DateAdded)";

            var parameters = new SqliteParameter[]
            {
                new SqliteParameter("@FirstName", tenant.FirstName ?? ""),
                new SqliteParameter("@LastName", tenant.LastName ?? ""),
                new SqliteParameter("@Email", tenant.Email ?? ""),
                new SqliteParameter("@PhoneNumber", tenant.PhoneNumber ?? ""),
                new SqliteParameter("@EmergencyContactName", tenant.EmergencyContactName ?? ""),
                new SqliteParameter("@EmergencyContactPhone", tenant.EmergencyContactPhone ?? ""),
                new SqliteParameter("@DateOfBirth", tenant.DateOfBirth),
                new SqliteParameter("@Occupation", tenant.Occupation ?? ""),
                new SqliteParameter("@MonthlyIncome", tenant.MonthlyIncome),
                new SqliteParameter("@DateAdded", tenant.DateAdded)
            };

            ExecuteNonQuery(sql, parameters);
        }

        public DataTable GetAllTenants()
        {
            return ExecuteQuery("SELECT * FROM Tenants ORDER BY TenantId DESC");
        }

        public DataTable SearchTenants(string searchTerm)
        {
            string sql = "SELECT * FROM Tenants WHERE FirstName LIKE @search OR LastName LIKE @search OR Email LIKE @search";
            var parameters = new SqliteParameter[] { new SqliteParameter("@search", $"%{searchTerm}%") };
            return ExecuteQuery(sql, parameters);
        }

        public void UpdateTenant(Tenant tenant)
        {
            string sql = @"UPDATE Tenants SET FirstName=@FirstName, LastName=@LastName, Email=@Email,
                          PhoneNumber=@PhoneNumber, EmergencyContactName=@EmergencyContactName,
                          EmergencyContactPhone=@EmergencyContactPhone, DateOfBirth=@DateOfBirth,
                          Occupation=@Occupation, MonthlyIncome=@MonthlyIncome
                          WHERE TenantId=@TenantId";

            var parameters = new SqliteParameter[]
            {
                new SqliteParameter("@TenantId", tenant.TenantId),
                new SqliteParameter("@FirstName", tenant.FirstName ?? ""),
                new SqliteParameter("@LastName", tenant.LastName ?? ""),
                new SqliteParameter("@Email", tenant.Email ?? ""),
                new SqliteParameter("@PhoneNumber", tenant.PhoneNumber ?? ""),
                new SqliteParameter("@EmergencyContactName", tenant.EmergencyContactName ?? ""),
                new SqliteParameter("@EmergencyContactPhone", tenant.EmergencyContactPhone ?? ""),
                new SqliteParameter("@DateOfBirth", tenant.DateOfBirth),
                new SqliteParameter("@Occupation", tenant.Occupation ?? ""),
                new SqliteParameter("@MonthlyIncome", tenant.MonthlyIncome)
            };

            ExecuteNonQuery(sql, parameters);
        }

        public void DeleteTenant(int tenantId)
        {
            string sql = "DELETE FROM Tenants WHERE TenantId=@TenantId";
            var parameters = new SqliteParameter[] { new SqliteParameter("@TenantId", tenantId) };
            ExecuteNonQuery(sql, parameters);
        }

        // Lease CRUD
        public void AddLease(Lease lease)
        {
            string sql = @"INSERT INTO Leases (PropertyId, TenantId, StartDate, EndDate, 
                          MonthlyRent, SecurityDepositPaid, Status, Terms, DateCreated)
                          VALUES (@PropertyId, @TenantId, @StartDate, @EndDate, 
                          @MonthlyRent, @SecurityDepositPaid, @Status, @Terms, @DateCreated)";

            var parameters = new SqliteParameter[]
            {
                new SqliteParameter("@PropertyId", lease.PropertyId),
                new SqliteParameter("@TenantId", lease.TenantId),
                new SqliteParameter("@StartDate", lease.StartDate),
                new SqliteParameter("@EndDate", lease.EndDate),
                new SqliteParameter("@MonthlyRent", lease.MonthlyRent),
                new SqliteParameter("@SecurityDepositPaid", lease.SecurityDepositPaid),
                new SqliteParameter("@Status", lease.Status ?? "Active"),
                new SqliteParameter("@Terms", lease.Terms ?? ""),
                new SqliteParameter("@DateCreated", lease.DateCreated)
            };

            ExecuteNonQuery(sql, parameters);

            // Update property status to Rented
            string updateProperty = "UPDATE Properties SET Status='Rented' WHERE PropertyId=@PropertyId";
            var propParams = new SqliteParameter[] { new SqliteParameter("@PropertyId", lease.PropertyId) };
            ExecuteNonQuery(updateProperty, propParams);
        }

        public DataTable GetAllLeases()
        {
            string sql = @"SELECT l.*, p.Address as PropertyAddress, t.FirstName || ' ' || t.LastName as TenantName 
                          FROM Leases l
                          JOIN Properties p ON l.PropertyId = p.PropertyId
                          JOIN Tenants t ON l.TenantId = t.TenantId
                          ORDER BY l.LeaseId DESC";
            return ExecuteQuery(sql);
        }

        public DataTable GetActiveLeases()
        {
            string sql = @"SELECT l.*, p.Address as PropertyAddress, t.FirstName || ' ' || t.LastName as TenantName,
                          l.MonthlyRent
                          FROM Leases l
                          JOIN Properties p ON l.PropertyId = p.PropertyId
                          JOIN Tenants t ON l.TenantId = t.TenantId
                          WHERE l.Status = 'Active' AND l.EndDate >= date('now')
                          ORDER BY l.EndDate ASC";
            return ExecuteQuery(sql);
        }

        // Payment CRUD
        public void AddPayment(Payment payment)
        {
            string sql = @"INSERT INTO Payments (LeaseId, PaymentDate, Amount, PaymentMethod, 
                          CheckNumber, ReferenceNumber, Notes, Status)
                          VALUES (@LeaseId, @PaymentDate, @Amount, @PaymentMethod, 
                          @CheckNumber, @ReferenceNumber, @Notes, @Status)";

            var parameters = new SqliteParameter[]
            {
                new SqliteParameter("@LeaseId", payment.LeaseId),
                new SqliteParameter("@PaymentDate", payment.PaymentDate),
                new SqliteParameter("@Amount", payment.Amount),
                new SqliteParameter("@PaymentMethod", payment.PaymentMethod ?? ""),
                new SqliteParameter("@CheckNumber", payment.CheckNumber ?? ""),
                new SqliteParameter("@ReferenceNumber", payment.ReferenceNumber ?? ""),
                new SqliteParameter("@Notes", payment.Notes ?? ""),
                new SqliteParameter("@Status", payment.Status ?? "Completed")
            };

            ExecuteNonQuery(sql, parameters);
        }

        public DataTable GetAllPayments()
        {
            string sql = @"SELECT p.*, t.FirstName || ' ' || t.LastName as TenantName, pr.Address as PropertyAddress
                          FROM Payments p
                          JOIN Leases l ON p.LeaseId = l.LeaseId
                          JOIN Tenants t ON l.TenantId = t.TenantId
                          JOIN Properties pr ON l.PropertyId = pr.PropertyId
                          ORDER BY p.PaymentDate DESC";
            return ExecuteQuery(sql);
        }

        // Dashboard Statistics
        public int GetTotalProperties()
        {
            string sql = "SELECT COUNT(*) FROM Properties";
            return Convert.ToInt32(ExecuteScalar(sql));
        }

        public int GetOccupiedProperties()
        {
            string sql = "SELECT COUNT(*) FROM Properties WHERE Status = 'Rented'";
            return Convert.ToInt32(ExecuteScalar(sql));
        }

        public int GetTotalTenants()
        {
            string sql = "SELECT COUNT(*) FROM Tenants";
            return Convert.ToInt32(ExecuteScalar(sql));
        }

        public decimal GetMonthlyIncome()
        {
            string sql = "SELECT SUM(MonthlyRent) FROM Leases WHERE Status = 'Active' AND EndDate >= date('now')";
            object result = ExecuteScalar(sql);

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToDecimal(result);
            }
            return 0;
        }

        public DataTable GetUpcomingPayments()
        {
            string sql = @"SELECT t.FirstName || ' ' || t.LastName as Tenant, p.Address, l.MonthlyRent, l.EndDate
                          FROM Leases l
                          JOIN Tenants t ON l.TenantId = t.TenantId
                          JOIN Properties p ON l.PropertyId = p.PropertyId
                          WHERE l.Status = 'Active' AND l.EndDate >= date('now')
                          ORDER BY l.EndDate ASC
                          LIMIT 10";
            return ExecuteQuery(sql);
        }
    }
}