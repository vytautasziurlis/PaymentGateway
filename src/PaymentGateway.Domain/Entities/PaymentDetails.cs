namespace PaymentGateway.Domain.Entities
{
    public class PaymentDetails
    {
        public string Reference { get; }

        public PaymentCardDetails CardDetails { get; }

        public Currency Currency { get; }

        public decimal Amount { get; }

        public PaymentDetails(string reference, PaymentCardDetails cardDetails,
            Currency currency, decimal amount)
        {
            Reference = reference;
            CardDetails = cardDetails;
            Currency = currency;
            Amount = amount;
        }
    }
}
