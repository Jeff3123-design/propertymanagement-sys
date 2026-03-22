using System;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        [Required]
        public int LeaseId { get; set; }

        public DateTime PaymentDate { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public string? PaymentMethod { get; set; }
        public string? CheckNumber { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }

        // Navigation property
        public virtual Lease? Lease { get; set; }

        public Payment()
        {
            PaymentDate = DateTime.Now;
            Status = "Completed";
        }

        public string MonthYear => PaymentDate.ToString("MMMM yyyy");
    }
}