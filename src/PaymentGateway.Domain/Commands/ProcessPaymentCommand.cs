using MediatR;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Commands
{
    public class ProcessPaymentCommand : IRequest<PaymentProcessingResult>
    {
        public PaymentCardDetails PaymentCardDetails { get; }

        public Currency Currency { get; }

        public int Amount { get; }

        public ProcessPaymentCommand(PaymentCardDetails paymentCardDetails, Currency currency, int amount)
        {
            PaymentCardDetails = paymentCardDetails;
            Currency = currency;
            Amount = amount;
        }
    }
}
