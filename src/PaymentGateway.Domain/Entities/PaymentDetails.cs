namespace PaymentGateway.Domain.Entities
{
    public class PaymentDetails
    {
        public string Reference { get; }

        public PaymentStatus Status { get; }

        public PaymentCardDetails CardDetails { get; }

        public Currency Currency { get; }

        public decimal Amount { get; }

        public PaymentDetails(string reference, PaymentStatus status,
            PaymentCardDetails cardDetails, Currency currency, decimal amount)
        {
            Reference = reference;
            Status = status;
            CardDetails = cardDetails;
            Currency = currency;
            Amount = amount;
        }
    }
}
