using PaymentGateway.Domain.Entities;

namespace PaymentGateway.UnitTests
{
    internal static class TestHelper
    {
        internal const string ValidCardNumber = "4689387567825";

        internal static PaymentCardDetails GetPaymentCardDetails(int expiryYear = 2021, int expiryMonth = 6) =>
            new PaymentCardDetails(new CardNumber(ValidCardNumber),
                new CardExpiry(expiryYear, expiryMonth), new CardCvv("123"));
    }
}
