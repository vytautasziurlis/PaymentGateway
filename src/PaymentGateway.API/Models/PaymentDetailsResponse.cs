using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.API.Models
{
    public class PaymentDetailsResponse
    {
        [Required]
        [JsonPropertyName("reference")]
        public string Reference { get; set; } = "";

        [Required]
        [JsonPropertyName("status")]
        public PaymentStatus Status { get; set; }

        [Required]
        [JsonPropertyName("paymentCardDetails")]
        public PaymentCardDetailsResponse? PaymentCardDetails { get; set; }

        [Required]
        [JsonPropertyName("currency")]
        public Currency Currency { get; set; }

        [Required]
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}
