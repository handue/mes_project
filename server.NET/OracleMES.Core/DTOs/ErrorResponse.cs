using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OracleMES.Core.DTOs
{
    public class ErrorResponse
    {
        [Required]
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("details")]
        public object? Details { get; set; }
    }
} 