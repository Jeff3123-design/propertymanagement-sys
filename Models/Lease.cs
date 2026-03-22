using System;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Models
{
    public class Lease
    {
        public int LeaseId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public decimal MonthlyRent { get; set; }
        public decimal SecurityDepositPaid { get; set; }
        public string Status { get; set; } // Active, Expired, Terminated
        public string Terms { get; set; }
        public DateTime DateCreated { get; set; }

        // Navigation properties
        public virtual Property Property { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }

        public Lease()
        {
            Payments = new List<Payment>();
            DateCreated = DateTime.Now;
            Status = "Active";
        }

        public bool IsActive => Status == "Active" && StartDate <= DateTime.Now && EndDate >= DateTime.Now;
        public int DaysRemaining => (EndDate - DateTime.Now).Days;
    }
}