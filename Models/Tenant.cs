using System;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Models
{
    public class Tenant
    {
        public int TenantId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string Occupation { get; set; }
        public decimal MonthlyIncome { get; set; }
        public DateTime DateAdded { get; set; }

        // Navigation property
        public virtual ICollection<Lease> Leases { get; set; }

        public Tenant()
        {
            Leases = new List<Lease>();
            DateAdded = DateTime.Now;
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}