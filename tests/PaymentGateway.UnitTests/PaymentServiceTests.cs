using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Persistence;
using PaymentGateway.Domain.Services;
using Shouldly;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class PaymentServiceTests
    {
        private readonly PaymentService _subject;
        private readonly Mock<IBankService> _bankService;
        private readonly Mock<IPaymentRepository> _paymentRepository;
        private readonly Mock<IDateTimeProvider> _dateTimeProvider;

        private const string PaymentReference = "ref123456";

        public PaymentServiceTests()
        {
            _bankService = new Mock<IBankService>();
            _bankService
                .Setup(x => x.ProcessPayment(It.IsAny<PaymentCardDetails>(),
                    It.IsAny<Currency>(), It.IsAny<decimal>()))
                .ReturnsAsync(new PaymentProcessingResult(PaymentReference, PaymentStatus.Success));

            _paymentRepository = new Mock<IPaymentRepository>();

            _dateTimeProvider = new Mock<IDateTimeProvider>();
            _dateTimeProvider
                .Setup(x => x.UtcNow())
                .Returns(new DateTime(2021, 6, 1));

            _subject = new PaymentService(_bankService.Object, _paymentRepository.Object,
                _dateTimeProvider.Object, new NullLogger<PaymentService>());
        }

        [Fact]
        public async Task ProcessPaymentCallsBankService()
        {
            var paymentCardDetails = TestHelper.GetPaymentCardDetails();

            await _subject.ProcessPayment(paymentCardDetails, Currency.GBP, 9.99m);

            _bankService.Verify(x => x.ProcessPayment(paymentCardDetails,
                Currency.GBP, 9.99m), Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentReturnsSuccessWhenBankReturnsSuccess()
        {
            var paymentCardDetails = TestHelper.GetPaymentCardDetails();

            var result = await _subject.ProcessPayment(paymentCardDetails, Currency.GBP, 9.99m);

            result.Reference.ShouldBe(PaymentReference);
            result.Status.ShouldBe(PaymentStatus.Success);
        }

        [Fact]
        public async Task ProcessPaymentReturnsFailureWhenBankReturnsFailure()
        {
            _bankService
                .Setup(x => x.ProcessPayment(It.IsAny<PaymentCardDetails>(),
                    It.IsAny<Currency>(), It.IsAny<decimal>()))
                .ReturnsAsync(new PaymentProcessingResult(PaymentReference, PaymentStatus.Failure));
            var paymentCardDetails = TestHelper.GetPaymentCardDetails();

            var result = await _subject.ProcessPayment(paymentCardDetails, Currency.GBP, 9.99m);

            result.Reference.ShouldBe(PaymentReference);
            result.Status.ShouldBe(PaymentStatus.Failure);
        }

        [Fact]
        public async Task ProcessPaymentCallsPaymentRepository()
        {
            var paymentCardDetails = TestHelper.GetPaymentCardDetails();

            await _subject.ProcessPayment(paymentCardDetails, Currency.GBP, 9.99m);

            _paymentRepository.Verify(x => x.AddPayment(
                It.Is<PaymentDetails>(payment => payment.Reference == PaymentReference)),
                Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentThrowsExceptionWhenCardIsExpired()
        {
            var lastMonth = _dateTimeProvider.Object.UtcNow().AddMonths(-1);
            var paymentCardDetails = TestHelper.GetPaymentCardDetails(expiryYear: lastMonth.Year,
                expiryMonth: lastMonth.Month);

            await Assert.ThrowsAsync<PaymentCardExpiredException>(() =>
                _subject.ProcessPayment(paymentCardDetails, Currency.GBP, 9.99m));
        }

        [Fact]
        public async Task ProcessPaymentThrowsExceptionWhenBankServiceThrows()
        {
            _bankService
                .Setup(x => x.ProcessPayment(It.IsAny<PaymentCardDetails>(),
                    It.IsAny<Currency>(), It.IsAny<decimal>()))
                .ThrowsAsync(new Exception());
            var paymentCardDetails = TestHelper.GetPaymentCardDetails();

            await Assert.ThrowsAsync<Exception>(() =>
                _subject.ProcessPayment(paymentCardDetails, Currency.GBP, 9.99m));
        }

        [Fact]
        public async Task ProcessPaymentThrowsExceptionWhenPaymentRepositoryThrows()
        {
            _paymentRepository
                .Setup(x => x.AddPayment(It.IsAny<PaymentDetails>()))
                .ThrowsAsync(new Exception());
            var paymentCardDetails = TestHelper.GetPaymentCardDetails();

            await Assert.ThrowsAsync<Exception>(() =>
                _subject.ProcessPayment(paymentCardDetails, Currency.GBP, 9.99m));
        }

        [Fact]
        public async Task GetPaymentReturnsNullWhenRepositoryReturnsNull()
        {
            _paymentRepository
                .Setup(x => x.GetPayment(PaymentReference))
                .ReturnsAsync((PaymentDetails)null);

            var result = await _subject.GetPayment(PaymentReference);

            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetPaymentReturnsPaymentWhenRepositoryReturnsData()
        {
            var paymentCardDetails = TestHelper.GetPaymentCardDetails();
            var paymentDetails = new PaymentDetails(PaymentReference,
                PaymentStatus.Success, paymentCardDetails, Currency.GBP, 9.99m);
            _paymentRepository
                .Setup(x => x.GetPayment(PaymentReference))
                .ReturnsAsync(paymentDetails);

            var result = await _subject.GetPayment(PaymentReference);

            result.ShouldNotBeNull();
            result.Reference.ShouldBe(paymentDetails.Reference);
            result.Status.ShouldBe(paymentDetails.Status);
            result.CardDetails.CardNumber.ShouldBe(paymentCardDetails.CardNumber);
            result.CardDetails.CardExpiry.ShouldBe(paymentCardDetails.CardExpiry);
            result.CardDetails.Cvv.ShouldBe(paymentCardDetails.Cvv);
            result.Currency.ShouldBe(paymentDetails.Currency);
            result.Amount.ShouldBe(paymentDetails.Amount);
        }

        [Fact]
        public async Task GetPaymentThrowsExceptionWhenPaymentRepositoryThrows()
        {
            _paymentRepository
                .Setup(x => x.GetPayment(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _subject.GetPayment(PaymentReference));
        }
    }
}
