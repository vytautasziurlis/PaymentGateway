using System.Threading.Tasks;
using PaymentGateway.API.Models;

namespace PaymentGateway.Client
{
    public interface IPaymentGatewayClient
    {
        Task<PaymentResponse> ProcessPayment(PaymentRequest request);

        Task<PaymentDetailsResponse?> GetPaymentDetails(string reference);
    }
}
