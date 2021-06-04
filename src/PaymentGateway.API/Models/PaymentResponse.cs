using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.API.Models
{
    public class PaymentResponse
    {
        [Required]
        [JsonPropertyName("reference")]
        public string Reference { get; set; } = "";

        [Required]
        [JsonPropertyName("status")]
        public PaymentStatus Status { get; set; }
    }
}
