using System.Threading;
using System.Threading.Tasks;
using Moq;
using PaymentGateway.Domain.Commands;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Handlers;
using PaymentGateway.Domain.Services;
using Shouldly;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class ProcessPaymentCommandHandlerTests
    {
        private readonly ProcessPaymentCommandHandler _subject;
        private readonly Mock<IPaymentService> _paymentService;
        private readonly PaymentProcessingResult _paymentProcessingResult;

        public ProcessPaymentCommandHandlerTests()
        {
            _paymentProcessingResult = new PaymentProcessingResult("ref", PaymentStatus.Success);
            _paymentService = new Mock<IPaymentService>();
            _paymentService
                .Setup(x => x.ProcessPayment(It.IsAny<PaymentCardDetails>(),
                    It.IsAny<Currency>(), It.IsAny<decimal>()))
                .ReturnsAsync(_paymentProcessingResult);

            _subject = new ProcessPaymentCommandHandler(_paymentService.Object);
        }

        [Fact]
        public async Task HandleCallsPaymentService()
        {
            var command = new ProcessPaymentCommand(TestHelper.GetPaymentCardDetails(),
                Currency.USD, 42.01m);

            await _subject.Handle(command, CancellationToken.None);

            _paymentService.Verify(x => x.ProcessPayment(It.IsAny<PaymentCardDetails>(),
                    It.IsAny<Currency>(), It.IsAny<decimal>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleReturnsResultFromPaymentService()
        {
            _paymentService
                .Setup(x => x.ProcessPayment(It.IsAny<PaymentCardDetails>(),
                    It.IsAny<Currency>(), It.IsAny<decimal>()))
                .ReturnsAsync(new PaymentProcessingResult("ref", PaymentStatus.Success));

            var command = new ProcessPaymentCommand(TestHelper.GetPaymentCardDetails(),
                Currency.USD, 42.01m);

            var result = await _subject.Handle(command, CancellationToken.None);

            result.Reference.ShouldBe(_paymentProcessingResult.Reference);
            result.Status.ShouldBe(_paymentProcessingResult.Status);
        }
    }
}
