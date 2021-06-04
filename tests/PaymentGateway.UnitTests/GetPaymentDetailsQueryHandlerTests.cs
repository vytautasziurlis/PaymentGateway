using System.Threading;
using System.Threading.Tasks;
using Moq;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Handlers;
using PaymentGateway.Domain.Queries;
using PaymentGateway.Domain.Services;
using Shouldly;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class GetPaymentDetailsQueryHandlerTests
    {
        private readonly GetPaymentDetailsQueryHandler _subject;
        private readonly Mock<IPaymentService> _paymentService;
        private const string PaymentReference = "ref123";
        private readonly PaymentDetails _paymentDetails;

        public GetPaymentDetailsQueryHandlerTests()
        {
            _paymentDetails = new PaymentDetails(PaymentReference,
                TestHelper.GetPaymentCardDetails(), Currency.EUR, 7363.93m);
            _paymentService = new Mock<IPaymentService>();
            _paymentService
                .Setup(x => x.GetPayment(PaymentReference))
                .ReturnsAsync(_paymentDetails);

            _subject = new GetPaymentDetailsQueryHandler(_paymentService.Object);
        }

        [Fact]
        public async Task HandleCallsPaymentService()
        {
            var command = new GetPaymentDetailsQuery(PaymentReference);

            await _subject.Handle(command, CancellationToken.None);

            _paymentService.Verify(x => x.GetPayment(PaymentReference),
                Times.Once);
        }

        [Fact]
        public async Task HandleReturnsResultFromPaymentService()
        {
            var command = new GetPaymentDetailsQuery(PaymentReference);

            var result = await _subject.Handle(command, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Reference.ShouldBe(_paymentDetails.Reference);
            result.CardDetails.CardNumber.ShouldBe(_paymentDetails.CardDetails.CardNumber);
            result.CardDetails.CardExpiry.ShouldBe(_paymentDetails.CardDetails.CardExpiry);
            result.CardDetails.Cvv.ShouldBe(_paymentDetails.CardDetails.Cvv);
            result.Currency.ShouldBe(_paymentDetails.Currency);
            result.Amount.ShouldBe(_paymentDetails.Amount);
        }

        [Fact]
        public async Task HandleReturnsNullWhenPaymentServiceReturnsNull()
        {
            _paymentService
                .Setup(x => x.GetPayment(PaymentReference))
                .ReturnsAsync((PaymentDetails)null);
            var command = new GetPaymentDetailsQuery(PaymentReference);

            var result = await _subject.Handle(command, CancellationToken.None);

            result.ShouldBeNull();
        }
    }
}
