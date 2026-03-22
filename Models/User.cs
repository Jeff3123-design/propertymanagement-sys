using System;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Store hashed password

        public string FullName { get; set; }
        public string Role { get; set; } // Admin, Manager, Staff
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastLoginDate { get; set; }

        public User()
        {
            Role = "Staff";
            IsActive = true;
            CreatedDate = DateTime.Now;
        }
    }

    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}