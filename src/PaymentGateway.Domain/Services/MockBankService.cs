using System;
using System.Threading.Tasks;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Services
{
    public class MockBankService : IBankService
    {
        public Task<PaymentProcessingResult> ProcessPayment(PaymentCardDetails paymentCardDetails,
            Currency currency, decimal amount)
        {
            var paymentReference = Guid.NewGuid().ToString();

            var status = paymentCardDetails.CardNumber.Value.StartsWith("5")
                ? PaymentStatus.Failure
                : PaymentStatus.Success;

            return Task.FromResult(new PaymentProcessingResult(paymentReference, status));
        }
    }
}
