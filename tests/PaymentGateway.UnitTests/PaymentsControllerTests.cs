using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using PaymentGateway.API.Controllers;
using PaymentGateway.API.Mappings;
using PaymentGateway.API.Models;
using PaymentGateway.Domain.Commands;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Queries;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class PaymentsControllerTests
    {
        private readonly PaymentsController _subject;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IMapper> _mapper;

        public PaymentsControllerTests()
        {
            _mediator = new Mock<IMediator>();
            _mediator
                .Setup(x => x.Send(It.IsAny<ProcessPaymentCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentProcessingResult("reference", PaymentStatus.Success));
            _mediator
                .Setup(x => x.Send(It.IsAny<GetPaymentDetailsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentDetails("reference", PaymentStatus.Success, TestHelper.GetPaymentCardDetails(), Currency.EUR, 361.38m));

            _mapper = new Mock<IMapper>();

            _subject = new PaymentsController(_mediator.Object, _mapper.Object);
        }

        [Fact]
        public async Task ProcessPaymentCallsMediator()
        {
            var cardDetails = TestHelper.GetPaymentCardDetails();
            var paymentRequest = new PaymentRequest
            {
                PaymentCardDetails = new PaymentCardDetailsRequest
                {
                    CardNumber = cardDetails.CardNumber.Value,
                    ExpiryYear = cardDetails.CardExpiry.Year,
                    ExpiryMonth = cardDetails.CardExpiry.Month,
                    Cvv = cardDetails.Cvv.Value
                },
                Currency = Currency.USD,
                Amount = 421.97m
            };

            await _subject.ProcessPayment(paymentRequest);

            _mediator.Verify(x => x.Send(It.IsAny<ProcessPaymentCommand>(),
                    CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task GetPaymentDetailsCallsMediator()
        {
            await _subject.GetPayment("reference");

            _mediator.Verify(x => x.Send(It.IsAny<GetPaymentDetailsQuery>(),
                    CancellationToken.None),
                Times.Once);
        }
    }
}
