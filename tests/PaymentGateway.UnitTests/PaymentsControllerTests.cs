using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentGateway.API.Controllers;
using PaymentGateway.API.Mapping;
using PaymentGateway.API.Models;
using PaymentGateway.Domain.Commands;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Queries;
using Shouldly;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class PaymentsControllerTests
    {
        private readonly PaymentsController _subject;
        private readonly Mock<IMediator> _mediator;

        private readonly PaymentProcessingResult _paymentProcessingResult;
        private readonly PaymentDetails _paymentDetails;

        public PaymentsControllerTests()
        {
            _mediator = new Mock<IMediator>();
            _paymentProcessingResult = new PaymentProcessingResult("reference", PaymentStatus.Success);
            _mediator
                .Setup(x => x.Send(It.IsAny<ProcessPaymentCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_paymentProcessingResult);
            _paymentDetails = new PaymentDetails("reference", PaymentStatus.Success,
                TestHelper.GetPaymentCardDetails(), Currency.EUR, 36138);
            _mediator
                .Setup(x => x.Send(It.IsAny<GetPaymentDetailsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_paymentDetails);

            var mapper = new Mock<IMapper>();
            mapper
                .Setup(x => x.Map(It.IsAny<PaymentProcessingResult>()))
                .Returns<PaymentProcessingResult>( x =>
                    new PaymentResponse { Reference = x.Reference, Status = x.Status });
            mapper
                .Setup(x => x.Map(It.IsAny<PaymentDetails>()))
                .Returns<PaymentDetails>( x =>
                    new PaymentDetailsResponse {
                        Reference = x.Reference,
                        Status = x.Status,
                        PaymentCardDetails = new PaymentCardDetailsResponse
                        {
                            CardNumber = x.CardDetails!.CardNumber.MaskedValue.Value,
                            ExpiryYear = x.CardDetails.CardExpiry.Year,
                            ExpiryMonth = x.CardDetails.CardExpiry.Month
                        },
                        Currency = x.Currency,
                        Amount = x.Amount
                    });

            _subject = new PaymentsController(_mediator.Object, mapper.Object);
        }

        [Fact]
        public async Task ProcessPaymentCallsMediator()
        {
            var paymentRequest = GetPaymentRequest();

            await _subject.ProcessPayment(paymentRequest);

            _mediator.Verify(x => x.Send(It.IsAny<ProcessPaymentCommand>(),
                    CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentReturnsResponse()
        {
            var paymentRequest = GetPaymentRequest();

            var result = await _subject.ProcessPayment(paymentRequest);

            result.ShouldNotBeNull();
            result.ShouldBeOfType<OkObjectResult>();
            var response = ((OkObjectResult) result).Value as PaymentResponse;
            response.ShouldNotBeNull();
            response.Reference.ShouldBe(_paymentProcessingResult.Reference);
            response.Status.ShouldBe(_paymentProcessingResult.Status);
        }

        [Fact]
        public async Task GetPaymentDetailsCallsMediator()
        {
            await _subject.GetPayment("reference");

            _mediator.Verify(x => x.Send(It.IsAny<GetPaymentDetailsQuery>(),
                    CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task GetPaymentDetailsReturnsResponseWhenPaymentExists()
        {
            var result = await _subject.GetPayment("reference");

            result.ShouldNotBeNull();
            result.ShouldBeOfType<OkObjectResult>();
            var response = ((OkObjectResult) result).Value as PaymentDetailsResponse;
            response.ShouldNotBeNull();
            response.Reference.ShouldBe(_paymentDetails.Reference);
            response.Status.ShouldBe(_paymentDetails.Status);
            response.PaymentCardDetails!.CardNumber.ShouldBe(_paymentDetails.CardDetails.CardNumber.MaskedValue.Value);
            response.PaymentCardDetails!.ExpiryYear.ShouldBe(_paymentDetails.CardDetails.CardExpiry.Year);
            response.PaymentCardDetails!.ExpiryMonth.ShouldBe(_paymentDetails.CardDetails.CardExpiry.Month);
            response.Currency.ShouldBe(_paymentDetails.Currency);
            response.Amount.ShouldBe(_paymentDetails.Amount);
        }

        [Fact]
        public async Task GetPaymentDetailsReturns404WhenPaymentDoesNotExist()
        {
            _mediator
                .Setup(x => x.Send(It.IsAny<GetPaymentDetailsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((PaymentDetails)null);

            var response = await _subject.GetPayment("reference");

            response.ShouldNotBeNull();
            response.ShouldBeOfType<NotFoundResult>();
        }

        private static PaymentRequest GetPaymentRequest()
        {
            var cardDetails = TestHelper.GetPaymentCardDetails();
            return new PaymentRequest
            {
                PaymentCardDetails = new PaymentCardDetailsRequest
                {
                    CardNumber = cardDetails.CardNumber.Value,
                    ExpiryYear = cardDetails.CardExpiry.Year,
                    ExpiryMonth = cardDetails.CardExpiry.Month,
                    Cvv = cardDetails.Cvv.Value
                },
                Currency = Currency.USD,
                Amount = 42197
            };
        }
    }
}
