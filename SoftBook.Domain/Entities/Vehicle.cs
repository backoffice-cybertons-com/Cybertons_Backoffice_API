using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("tab_vehicle_master")]
    public class Vehicle
    {
        [Key]
        [Column("vehicle_id")]
        public int VehicleId { get; set; }

        [Column("company_id")]
        public int CompanyId { get; set; }

        [Column("transporter_id")]
        public int TransporterId { get; set; }

        [Column("vehicle_no")]
        public string VehicleNo { get; set; } = string.Empty;

        [Column("vehicle_type")]
        public string? VehicleType { get; set; }

        [Column("registration_expiry")]
        public DateTime? RegistrationExpiry { get; set; }

        [Column("driver_name")]
        public string? DriverName { get; set; }

        [Column("driver_phone")]
        public string? DriverPhone { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_on")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
