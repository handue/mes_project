using System.ComponentModel.DataAnnotations;

namespace OracleMES.Core.DTOs
{
    public class CreateOEEDTO
    {
        [Required]
        public string MetricId { get; set; } = null!;
        
        [Required]
        public string MachineId { get; set; } = null!;
        
        public string? Date { get; set; }
        public decimal? Availability { get; set; }
        public decimal? Performance { get; set; }
        public decimal? Quality { get; set; }
        public decimal? Oee { get; set; }
        public decimal? PlannedProductionTime { get; set; }
        public decimal? ActualProductionTime { get; set; }
        public decimal? Downtime { get; set; }
    }

    public class UpdateOEEDTO
    {
        public string? MachineId { get; set; }
        public string? Date { get; set; }
        public decimal? Availability { get; set; }
        public decimal? Performance { get; set; }
        public decimal? Quality { get; set; }
        public decimal? Oee { get; set; }
        public decimal? PlannedProductionTime { get; set; }
        public decimal? ActualProductionTime { get; set; }
        public decimal? Downtime { get; set; }
    }

    public class OEEResponseDTO
    {
        public string MetricId { get; set; } = null!;
        public string MachineId { get; set; } = null!;
        public string? Date { get; set; }
        public decimal? Availability { get; set; }
        public decimal? Performance { get; set; }
        public decimal? Quality { get; set; }
        public decimal? Oee { get; set; }
        public decimal? PlannedProductionTime { get; set; }
        public decimal? ActualProductionTime { get; set; }
        public decimal? Downtime { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
} 