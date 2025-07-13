using System.ComponentModel.DataAnnotations;

namespace OracleMES.Core.DTOs
{
    public class CreateEmployeeDTO
    {
        [Required]
        public string EmployeeId { get; set; } = null!;
        
        [Required]
        public string Name { get; set; } = null!;
        
        public string? Role { get; set; }
        public string? ShiftId { get; set; }
        public decimal HourlyRate { get; set; }
        public string? Skills { get; set; }
        public string? HireDate { get; set; }
    }

    public class UpdateEmployeeDTO
    {
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? ShiftId { get; set; }
        public decimal? HourlyRate { get; set; }
        public string? Skills { get; set; }
        public string? HireDate { get; set; }
    }

    public class EmployeeResponseDTO
    {
        public string EmployeeId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Role { get; set; }
        public string? ShiftId { get; set; }
        public decimal HourlyRate { get; set; }
        public string? Skills { get; set; }
        public string? HireDate { get; set; }
    }
} 