using System.Threading.Tasks;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Services
{
    public interface IBankService
    {
        Task<PaymentProcessingResult> ProcessPayment(PaymentCardDetails paymentCardDetails,
            Currency currency, decimal amount);
    }
}
