namespace PaymentGateway.Domain.Entities
{
    public class PaymentProcessingResult
    {
        public string Reference { get; }

        public PaymentStatus Status { get; }

        public PaymentProcessingResult(string reference, PaymentStatus status)
        {
            Reference = reference;
            Status = status;
        }
    }
}
