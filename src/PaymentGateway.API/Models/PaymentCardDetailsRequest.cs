using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentGateway.API.Models
{
    public class PaymentCardDetailsRequest
    {
        [Required]
        [JsonPropertyName("cardNumber")]
        public string CardNumber { get; set; } = "";

        [Required]
        [JsonPropertyName("expiryYear")]
        public int ExpiryYear { get; set; }

        [Required]
        [JsonPropertyName("expiryMonth")]
        public int ExpiryMonth { get; set; }

        [Required]
        [JsonPropertyName("cvv")]
        public string Cvv { get; set; } = "";
    }
}
