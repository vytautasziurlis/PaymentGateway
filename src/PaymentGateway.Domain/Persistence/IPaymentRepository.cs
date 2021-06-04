using System.Threading.Tasks;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Persistence
{
    public interface IPaymentRepository
    {
        Task AddPayment(PaymentDetails paymentDetails);

        Task<PaymentDetails?> GetPayment(string paymentReference);
    }
}
