using System.Threading.Tasks;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Services;
using Shouldly;
using Xunit;

namespace PaymentGateway.UnitTests.Services
{
    public class MockBankServiceTests
    {
        private readonly MockBankService _subject = new MockBankService();

        [Fact]
        public async Task ProcessPaymentReturnsSuccessWhenPaymentIsValid()
        {
            var cardDetails = new PaymentCardDetails(new CardNumber(TestHelper.ValidCardNumber),
                new CardExpiry(2021, 6), new CardCvv("123"));

            var result = await _subject.ProcessPayment(cardDetails, Currency.GBP, 9.99m);

            result.Status.ShouldBe(PaymentStatus.Success);
        }

        [Fact]
        public async Task ProcessPaymentReturnsFailureWhenCardIsInvalid()
        {
            var cardDetails = new PaymentCardDetails(new CardNumber("5201294442453002"),
                new CardExpiry(2021, 6), new CardCvv("123"));

            var result = await _subject.ProcessPayment(cardDetails, Currency.GBP, 9.99m);

            result.Status.ShouldBe(PaymentStatus.Failure);
        }
    }
}
