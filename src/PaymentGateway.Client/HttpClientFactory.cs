using System.Net.Http;

namespace PaymentGateway.Client
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient Create() => new HttpClient();
    }
}
