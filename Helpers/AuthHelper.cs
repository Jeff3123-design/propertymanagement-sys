using System;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SQLite;
using PropertyManagementSystem.Models;
using PropertyManagementSystem.Data;

namespace PropertyManagementSystem.Helpers
{
    public class AuthHelper
    {
        private readonly DatabaseHelper _db;
        private User _currentUser;

        public AuthHelper(DatabaseHelper db)
        {
            _db = db;
        }

        public User CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;
        public bool IsAdmin => IsAuthenticated && _currentUser.Role == "Admin";

        // Hash password using SHA256
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Login method
        public bool Login(string username, string password)
        {
            string hashedPassword = HashPassword(password);

            string query = "SELECT * FROM Users WHERE Username = @username AND PasswordHash = @passwordHash AND IsActive = 1";
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@username", username),
                new SQLiteParameter("@passwordHash", hashedPassword)
            };

            DataTable result = _db.ExecuteQuery(query, parameters);

            if (result.Rows.Count > 0)
            {
                _currentUser = new User
                {
                    UserId = Convert.ToInt32(result.Rows[0]["UserId"]),
                    Username = result.Rows[0]["Username"].ToString(),
                    Email = result.Rows[0]["Email"].ToString(),
                    FullName = result.Rows[0]["FullName"].ToString(),
                    Role = result.Rows[0]["Role"].ToString(),
                    IsActive = Convert.ToBoolean(result.Rows[0]["IsActive"])
                };

                // Update last login
                UpdateLastLogin(_currentUser.UserId);
                return true;
            }

            return false;
        }

        // Register new user
        public bool Register(User user, string password)
        {
            // Check if username exists
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
            var checkParams = new SQLiteParameter[] { new SQLiteParameter("@username", user.Username) };
            int count = Convert.ToInt32(_db.ExecuteScalar(checkQuery, checkParams));

            if (count > 0)
                return false;

            // Insert new user
            string insertQuery = @"INSERT INTO Users (Username, Email, PasswordHash, FullName, Role, IsActive, CreatedDate)
                                  VALUES (@username, @email, @passwordHash, @fullName, @role, @isActive, @createdDate)";

            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@username", user.Username),
                new SQLiteParameter("@email", user.Email),
                new SQLiteParameter("@passwordHash", HashPassword(password)),
                new SQLiteParameter("@fullName", user.FullName ?? user.Username),
                new SQLiteParameter("@role", user.Role ?? "Staff"),
                new SQLiteParameter("@isActive", true),
                new SQLiteParameter("@createdDate", DateTime.Now)
            };

            return _db.ExecuteNonQuery(insertQuery, parameters) > 0;
        }

        // Change password
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            string oldHash = HashPassword(oldPassword);
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username AND PasswordHash = @oldHash";
            var checkParams = new SQLiteParameter[]
            {
                new SQLiteParameter("@username", username),
                new SQLiteParameter("@oldHash", oldHash)
            };

            int count = Convert.ToInt32(_db.ExecuteScalar(checkQuery, checkParams));
            if (count == 0)
                return false;

            string updateQuery = "UPDATE Users SET PasswordHash = @newHash WHERE Username = @username";
            var updateParams = new SQLiteParameter[]
            {
                new SQLiteParameter("@newHash", HashPassword(newPassword)),
                new SQLiteParameter("@username", username)
            };

            return _db.ExecuteNonQuery(updateQuery, updateParams) > 0;
        }

        // Update last login
        private void UpdateLastLogin(int userId)
        {
            string query = "UPDATE Users SET LastLoginDate = @loginDate WHERE UserId = @userId";
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@loginDate", DateTime.Now),
                new SQLiteParameter("@userId", userId)
            };
            _db.ExecuteNonQuery(query, parameters);
        }

        // Logout
        public void Logout()
        {
            _currentUser = null;
        }
    }
}