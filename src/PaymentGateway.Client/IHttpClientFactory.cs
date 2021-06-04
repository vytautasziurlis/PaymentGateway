using System.Net.Http;

namespace PaymentGateway.Client
{
    public interface IHttpClientFactory
    {
        HttpClient Create();
    }
}
