using System.Threading.Tasks;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Services
{
    public interface IPaymentService
    {
        Task<PaymentProcessingResult> ProcessPayment(PaymentCardDetails paymentCardDetails,
            Currency currency, decimal amount);

        Task<PaymentDetails?> GetPayment(string paymentReference);
    }
}
