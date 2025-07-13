using System.ComponentModel.DataAnnotations;

namespace OracleMES.Core.DTOs
{
    public class CreateMachineDTO
    {
        [Required]
        public string MachineId { get; set; } = null!;
        
        [Required]
        public string Name { get; set; } = null!;
        
        public string? Type { get; set; }
        public string? WorkcenterId { get; set; }
        public string? Status { get; set; }
        public decimal NominalCapacity { get; set; }
        public string CapacityUOM { get; set; } = null!;
        public decimal SetupTime { get; set; }
        public decimal EfficiencyFactor { get; set; }
        public decimal MaintenanceFrequency { get; set; }
        public decimal ProductChangeoverTime { get; set; }
        public decimal CostPerHour { get; set; }
        public string? InstallationDate { get; set; }
        public string? ModelNumber { get; set; }
    }

    public class UpdateMachineDTO
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? WorkcenterId { get; set; }
        public string? Status { get; set; }
        public decimal? NominalCapacity { get; set; }
        public string? CapacityUOM { get; set; }
        public decimal? SetupTime { get; set; }
        public decimal? EfficiencyFactor { get; set; }
        public decimal? MaintenanceFrequency { get; set; }
        public decimal? ProductChangeoverTime { get; set; }
        public decimal? CostPerHour { get; set; }
        public string? InstallationDate { get; set; }
        public string? ModelNumber { get; set; }
    }

    public class MachineResponseDTO
    {
        public string MachineId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Type { get; set; }
        public string? WorkcenterId { get; set; }
        public string? Status { get; set; }
        public decimal NominalCapacity { get; set; }
        public string CapacityUOM { get; set; } = null!;
        public decimal SetupTime { get; set; }
        public decimal EfficiencyFactor { get; set; }
        public decimal MaintenanceFrequency { get; set; }
        public string? LastMaintenanceDate { get; set; }
        public string? NextMaintenanceDate { get; set; }
        public decimal ProductChangeoverTime { get; set; }
        public decimal CostPerHour { get; set; }
        public string? InstallationDate { get; set; }
        public string? ModelNumber { get; set; }
    }
} 