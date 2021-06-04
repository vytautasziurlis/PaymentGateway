using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.API.Models
{
    public class PaymentRequest
    {
        [Required]
        [JsonPropertyName("paymentCardDetails")]
        public PaymentCardDetailsRequest? PaymentCardDetails { get; set; }

        [Required]
        [JsonPropertyName("currency")]
        public Currency Currency { get; set; }

        [Required]
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}
