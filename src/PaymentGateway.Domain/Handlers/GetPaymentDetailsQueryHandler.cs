using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Queries;
using PaymentGateway.Domain.Services;

namespace PaymentGateway.Domain.Handlers
{
    public class GetPaymentDetailsQueryHandler : IRequestHandler<GetPaymentDetailsQuery, PaymentDetails?>
    {
        private readonly IPaymentService _paymentService;

        public GetPaymentDetailsQueryHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public Task<PaymentDetails?> Handle(GetPaymentDetailsQuery request,
            CancellationToken cancellationToken) =>
            _paymentService.GetPayment(request.PaymentReference);
    }
}
