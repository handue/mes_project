using System.ComponentModel.DataAnnotations;

namespace OracleMES.Core.DTOs
{
    public class CreateInventoryDTO
    {
        [Required]
        public string ItemId { get; set; } = null!;
        
        [Required]
        public string Name { get; set; } = null!;
        
        public string? Category { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReorderLevel { get; set; }
        public string? SupplierId { get; set; }
        public decimal LeadTime { get; set; }
        public decimal Cost { get; set; }
        public string? LotNumber { get; set; }
        public string? Location { get; set; }
        public string? LastReceivedDate { get; set; }
    }

    public class UpdateInventoryDTO
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReorderLevel { get; set; }
        public string? SupplierId { get; set; }
        public decimal? LeadTime { get; set; }
        public decimal? Cost { get; set; }
        public string? LotNumber { get; set; }
        public string? Location { get; set; }
        public string? LastReceivedDate { get; set; }
    }

    public class InventoryResponseDTO
    {
        public string ItemId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReorderLevel { get; set; }
        public string? SupplierId { get; set; }
        public decimal LeadTime { get; set; }
        public decimal Cost { get; set; }
        public string? LotNumber { get; set; }
        public string? Location { get; set; }
        public string? LastReceivedDate { get; set; }
    }
} 