using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaymentGateway.Domain.Commands;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Services;

namespace PaymentGateway.Domain.Handlers
{
    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentProcessingResult>
    {
        private readonly IPaymentService _paymentService;

        public ProcessPaymentCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public Task<PaymentProcessingResult> Handle(ProcessPaymentCommand request,
            CancellationToken cancellationToken) =>
            _paymentService.ProcessPayment(request.PaymentCardDetails,
                request.Currency, request.Amount);
    }
}
