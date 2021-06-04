using MediatR;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Queries
{
    public class GetPaymentDetailsQuery : IRequest<PaymentDetails?>
    {
        public string PaymentReference { get; }

        public GetPaymentDetailsQuery(string paymentReference)
        {
            PaymentReference = paymentReference;
        }
    }
}
