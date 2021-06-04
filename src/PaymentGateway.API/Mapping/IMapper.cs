using PaymentGateway.API.Models;
using PaymentGateway.Domain.Commands;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.API.Mapping
{
    public interface IMapper
    {
        ProcessPaymentCommand Map(PaymentRequest request);

        PaymentResponse Map(PaymentProcessingResult processingResult);

        PaymentDetailsResponse Map(PaymentDetails paymentDetails);
    }
}
