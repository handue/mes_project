using System.ComponentModel.DataAnnotations;

namespace OracleMES.Core.DTOs
{
    public class CreateWorkorderDTO
    {
        [Required]
        public string OrderId { get; set; } = null!;
        
        [Required]
        public string ProductId { get; set; } = null!;
        
        [Required]
        public string WorkcenterId { get; set; } = null!;
        
        [Required]
        public string MachineId { get; set; } = null!;
        
        [Required]
        public string EmployeeId { get; set; } = null!;
        
        [Required]
        public int Quantity { get; set; }
        
        public string? PlannedStartTime { get; set; }
        public string? PlannedEndTime { get; set; }
        public string? Status { get; set; }
        public int Priority { get; set; }
        public int LeadTime { get; set; }
        public string? LotNumber { get; set; }
    }

    public class UpdateWorkorderDTO
    {
        public string? ProductId { get; set; }
        public string? WorkcenterId { get; set; }
        public string? MachineId { get; set; }
        public string? EmployeeId { get; set; }
        public int? Quantity { get; set; }
        public string? PlannedStartTime { get; set; }
        public string? PlannedEndTime { get; set; }
        public string? Status { get; set; }
        public int? Priority { get; set; }
        public int? LeadTime { get; set; }
        public string? LotNumber { get; set; }
    }

    public class WorkorderResponseDTO
    {
        public string OrderId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public string WorkcenterId { get; set; } = null!;
        public string MachineId { get; set; } = null!;
        public string EmployeeId { get; set; } = null!;
        public int Quantity { get; set; }
        public string? PlannedStartTime { get; set; }
        public string? PlannedEndTime { get; set; }
        public string? ActualStartTime { get; set; }
        public string? ActualEndTime { get; set; }
        public string? Status { get; set; }
        public int Priority { get; set; }
        public int LeadTime { get; set; }
        public string? LotNumber { get; set; }
        public decimal? ActualProduction { get; set; }
        public decimal? Scrap { get; set; }
        public decimal? SetupTimeActual { get; set; }
    }
} 