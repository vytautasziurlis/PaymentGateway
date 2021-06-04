using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.API.Models
{
    public class PaymentDetailsResponse
    {
        [Required]
        [JsonPropertyName("reference")]
        public string Reference { get; init; } = "";

        [Required]
        [JsonPropertyName("status")]
        public PaymentStatus Status { get; init; }

        [Required]
        [JsonPropertyName("paymentCardDetails")]
        public PaymentCardDetailsResponse? PaymentCardDetails { get; init; }

        [Required]
        [JsonPropertyName("currency")]
        public Currency Currency { get; init; }

        [Required]
        [JsonPropertyName("amount")]
        public int Amount { get; init; }
    }
}
