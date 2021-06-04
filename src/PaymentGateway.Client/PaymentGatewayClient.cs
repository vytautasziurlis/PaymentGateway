using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PaymentGateway.API.Models;

namespace PaymentGateway.Client
{
    public class PaymentGatewayClient : IPaymentGatewayClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _serializerOptions;

        internal PaymentGatewayClient(IHttpClientFactory httpClientFactory, string baseUrl)
        {
            _httpClient = httpClientFactory.Create();
            _baseUrl = baseUrl.TrimEnd('/');

            _serializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
        }

        public PaymentGatewayClient(string baseUrl) : this(new HttpClientFactory(), baseUrl)
        {
        }

        public async Task<PaymentResponse> ProcessPayment(PaymentRequest request)
        {
            var requestUri = new Uri($"{_baseUrl}/payments");
            var content = new StringContent(JsonSerializer.Serialize(request, _serializerOptions),
                Encoding.UTF8, "application/json");

            var httpResponse = await SendAsync(HttpMethod.Post, requestUri, content);

            if (httpResponse.IsSuccessStatusCode == false)
            {
                throw new Exception($"Error processing payment request, response was {httpResponse.StatusCode}");
            }

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentResponse>(responseContent, _serializerOptions)
                   ?? throw new Exception($"Error deserializing response: {responseContent}");
        }

        public async Task<PaymentDetailsResponse?> GetPaymentDetails(string reference)
        {
            var requestUri = new Uri($"{_baseUrl}/payments/{reference}");
            var httpResponse = await SendAsync(HttpMethod.Get, requestUri);
            if (httpResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (httpResponse.IsSuccessStatusCode == false)
            {
                throw new Exception($"Error processing request, response was {httpResponse.StatusCode}");
            }

            var content = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentDetailsResponse>(content, _serializerOptions)
                ?? throw new Exception($"Error deserializing response: {content}");
        }

        private Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod,
            Uri requestUri, HttpContent? content = null)
        {
            var httpRequest = new HttpRequestMessage(httpMethod, requestUri);
            if (content != null)
            {
                httpRequest.Content = content;
            }
            return _httpClient.SendAsync(httpRequest);
        }
    }
}
