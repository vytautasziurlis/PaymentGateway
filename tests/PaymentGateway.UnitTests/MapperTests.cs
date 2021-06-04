using System;
using PaymentGateway.API.Mapping;
using PaymentGateway.API.Models;
using PaymentGateway.Domain.Entities;
using Shouldly;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class MapperTests
    {
        private readonly Mapper _subject;

        public MapperTests()
        {
            _subject = new Mapper();
        }

        [Fact]
        public void MapPaymentRequestMapsToCommand()
        {
            var request = new PaymentRequest
            {
                PaymentCardDetails = new PaymentCardDetailsRequest
                {
                    CardNumber = TestHelper.ValidCardNumber,
                    ExpiryYear = 2021,
                    ExpiryMonth = 6,
                    Cvv = "123",
                },
                Currency = Currency.USD,
                Amount = 8374
            };

            var command = _subject.Map(request);

            command.PaymentCardDetails.CardNumber.Value.ShouldBe(request.PaymentCardDetails.CardNumber);
            command.PaymentCardDetails.CardExpiry.Year.ShouldBe(request.PaymentCardDetails.ExpiryYear);
            command.PaymentCardDetails.CardExpiry.Month.ShouldBe(request.PaymentCardDetails.ExpiryMonth);
            command.PaymentCardDetails.Cvv.Value.ShouldBe(request.PaymentCardDetails.Cvv);
            command.Currency.ShouldBe(request.Currency);
            command.Amount.ShouldBe(request.Amount);
        }

        [Fact]
        public void MapPaymentRequestThrowsExceptionWhenCardDetailsIsNull()
        {
            var request = new PaymentRequest
            {
                PaymentCardDetails = null,
                Currency = Currency.USD,
                Amount = 83794
            };

            Assert.Throws<ArgumentNullException>(() => _subject.Map(request));
        }

        [Fact]
        public void MapPaymentProcessingResultMapsToPaymentResult()
        {
            var processingResult = new PaymentProcessingResult("reference", PaymentStatus.Success);

            var response = _subject.Map(processingResult);

            response.Reference.ShouldBe(processingResult.Reference);
            response.Status.ShouldBe(processingResult.Status);
        }

        [Fact]
        public void MapPaymentDetailsMapsToPaymentDetailsResponse()
        {
            var paymentDetails = new PaymentDetails("reference", PaymentStatus.Success,
                TestHelper.GetPaymentCardDetails(), Currency.GBP, 84790);

            var response = _subject.Map(paymentDetails);

            response.Reference.ShouldBe(paymentDetails.Reference);
            response.Status.ShouldBe(paymentDetails.Status);
            response.PaymentCardDetails?.CardNumber.ShouldBe(paymentDetails.CardDetails.CardNumber.MaskedValue.Value);
            response.PaymentCardDetails?.ExpiryYear.ShouldBe(paymentDetails.CardDetails.CardExpiry.Year);
            response.PaymentCardDetails?.ExpiryMonth.ShouldBe(paymentDetails.CardDetails.CardExpiry.Month);
            response.Currency.ShouldBe(paymentDetails.Currency);
            response.Amount.ShouldBe(paymentDetails.Amount);
        }
    }
}
