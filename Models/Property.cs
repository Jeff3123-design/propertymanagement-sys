using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Models
{
    public class Property
    {
        public int PropertyId { get; set; }

        [Required]
        [StringLength(100)]
        public string? Address { get; set; }

        [Required]
        [StringLength(50)]
        public string? City { get; set; }

        [Required]
        [StringLength(20)]
        public string? PostalCode { get; set; }

        [Required]
        public string? PropertyType { get; set; }

        public int Bedrooms { get; set; }
        public double Bathrooms { get; set; }
        public double SquareFootage { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal SecurityDeposit { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public DateTime DateAdded { get; set; }

        // Navigation property
        public virtual ICollection<Lease>? Leases { get; set; }

        public Property()
        {
            Leases = new List<Lease>();
            DateAdded = DateTime.Now;
            Status = "Available";
        }
    }
}