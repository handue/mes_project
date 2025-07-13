using System.ComponentModel.DataAnnotations;

namespace OracleMES.Core.DTOs
{
    public class CreateQualityControlDTO
    {
        [Required]
        public string CheckId { get; set; } = null!;
        
        [Required]
        public string OrderId { get; set; } = null!;
        
        public string? Date { get; set; }
        public string? Result { get; set; }
        public string? Comments { get; set; }
        public decimal? DefectRate { get; set; }
        public decimal? ReworkRate { get; set; }
        public decimal? YieldRate { get; set; }
        public string? InspectorId { get; set; }
    }

    public class UpdateQualityControlDTO
    {
        public string? OrderId { get; set; }
        public string? Date { get; set; }
        public string? Result { get; set; }
        public string? Comments { get; set; }
        public decimal? DefectRate { get; set; }
        public decimal? ReworkRate { get; set; }
        public decimal? YieldRate { get; set; }
        public string? InspectorId { get; set; }
    }

    public class QualityControlResponseDTO
    {
        public string CheckId { get; set; } = null!;
        public string OrderId { get; set; } = null!;
        public string? Date { get; set; }
        public string? Result { get; set; }
        public string? Comments { get; set; }
        public decimal? DefectRate { get; set; }
        public decimal? ReworkRate { get; set; }
        public decimal? YieldRate { get; set; }
        public string? InspectorId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
} 