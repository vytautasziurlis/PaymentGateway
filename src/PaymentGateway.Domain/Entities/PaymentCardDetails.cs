namespace PaymentGateway.Domain.Entities
{
    public class PaymentCardDetails
    {
        public CardNumber CardNumber { get; }

        public CardExpiry CardExpiry { get; }

        public CardCvv Cvv { get; }

        public PaymentCardDetails(CardNumber cardNumber, CardExpiry cardExpiry, CardCvv cvv)
        {
            CardNumber = cardNumber;
            CardExpiry = cardExpiry;
            Cvv = cvv;
        }
    }
}
