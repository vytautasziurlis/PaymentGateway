using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using PaymentGateway.API.Models;
using PaymentGateway.Client;
using PaymentGateway.Domain.Entities;
using Shouldly;
using Xunit;
using IHttpClientFactory = PaymentGateway.Client.IHttpClientFactory;

namespace PaymentGateway.UnitTests
{
    public class PaymentGatewayClientTests
    {
        private readonly IPaymentGatewayClient _subject;
        private readonly Mock<DelegatingHandler> _httpHandler;

        public PaymentGatewayClientTests()
        {
            _httpHandler = new Mock<DelegatingHandler>();
            _httpHandler.As<IDisposable>().Setup(s => s.Dispose());
            var httpClient = new HttpClient(_httpHandler.Object);

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory
                .Setup(x => x.Create())
                .Returns(httpClient);

            _subject = new PaymentGatewayClient(httpClientFactory.Object, "http://localhost");
        }

        [Fact]
        public async Task ProcessPaymentCallsHttpClient()
        {
            HttpRequestMessage requestMessage = null;
            var mockResponse = new PaymentResponse
            {
                Reference = "reference",
                Status = PaymentStatus.Success
            };

            _httpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) => requestMessage = request)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(Serialize(mockResponse))
                });

            var paymentRequest = new PaymentRequest
            {
                PaymentCardDetails = new PaymentCardDetailsRequest
                {
                    CardNumber = "123456789",
                    ExpiryYear = 2021,
                    ExpiryMonth = 6,
                    Cvv = "123"
                },
                Currency = Currency.GBP,
                Amount = 42
            };

            var result = await _subject.ProcessPayment(paymentRequest);

            requestMessage.ShouldNotBeNull();
            requestMessage.Method.ShouldBe(HttpMethod.Post);
            requestMessage.RequestUri.ShouldBe(new Uri("http://localhost/payments"));
            var requestContent = await requestMessage.Content!.ReadAsStringAsync();
            requestContent.ShouldBe(Serialize(paymentRequest));

            result.ShouldNotBeNull();
            result.Reference.ShouldBe(mockResponse.Reference);
            result.Status.ShouldBe(mockResponse.Status);
        }

        [Fact]
        public async Task GetPaymentDetailsCallsHttpClient()
        {
            HttpRequestMessage requestMessage = null;
            var mockResponse = new PaymentDetailsResponse
            {
                Reference = "ref",
                Status = PaymentStatus.Success,
                PaymentCardDetails = new PaymentCardDetailsResponse
                {
                    CardNumber = "*****1234",
                    ExpiryYear = 2021,
                    ExpiryMonth = 6
                },
                Currency = Currency.USD,
                Amount = 2000
            };

            _httpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) => requestMessage = request)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(Serialize(mockResponse))
                });

            var result = await _subject.GetPaymentDetails("ref");

            requestMessage.ShouldNotBeNull();
            requestMessage.Method.ShouldBe(HttpMethod.Get);
            requestMessage.RequestUri.ShouldBe(new Uri("http://localhost/payments/ref"));

            result.ShouldNotBeNull();
            result.Reference.ShouldBe(mockResponse.Reference);
            result.Status.ShouldBe(mockResponse.Status);
            result.PaymentCardDetails!.CardNumber.ShouldBe(mockResponse.PaymentCardDetails.CardNumber);
            result.PaymentCardDetails!.ExpiryYear.ShouldBe(mockResponse.PaymentCardDetails.ExpiryYear);
            result.PaymentCardDetails!.ExpiryMonth.ShouldBe(mockResponse.PaymentCardDetails.ExpiryMonth);
            result.Currency.ShouldBe(mockResponse.Currency);
            result.Amount.ShouldBe(mockResponse.Amount);
        }

        [Fact]
        public async Task GetPaymentDetailsReturnsNullWhenClientReturns404()
        {
            _httpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound))
                .Verifiable();

            var result = await _subject.GetPaymentDetails("ref");

            result.ShouldBeNull();
        }

        private static string Serialize(object obj) =>
            JsonSerializer.Serialize(obj,
                new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                });
    }
}
