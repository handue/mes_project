using System.ComponentModel.DataAnnotations;

namespace OracleMES.Core.DTOs
{
    public class CreateProductDTO
    {
        [Required]
        public string ProductId { get; set; } = null!;
        
        [Required]
        public string Name { get; set; } = null!;
        
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal Cost { get; set; }
        public decimal? StandardProcessTime { get; set; }
        public decimal? IsActive { get; set; }
    }

    public class UpdateProductDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal? Cost { get; set; }
        public decimal? StandardProcessTime { get; set; }
        public decimal? IsActive { get; set; }
    }

    public class ProductResponseDTO
    {
        public string ProductId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal Cost { get; set; }
        public decimal? StandardProcessTime { get; set; }
        public decimal? IsActive { get; set; }
    }
} 