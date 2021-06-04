using FluentValidation.TestHelper;
using PaymentGateway.API.Models;
using PaymentGateway.API.Validation;
using PaymentGateway.Domain.Entities;
using Shouldly;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class PaymentRequestValidatorTests
    {
        private readonly PaymentRequestValidator _subject;

        public PaymentRequestValidatorTests()
        {
            _subject = new PaymentRequestValidator();
        }

        [Fact]
        public void ValidationPassesWhenRequestIsValid()
        {
            var request = GetValidRequest();

            var result = _subject.TestValidate(request);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void ValidationFailsWhenCardDetailsIsNull()
        {
            var request = GetValidRequest();
            request.PaymentCardDetails = null;

            var result = _subject.TestValidate(request);

            result.ShouldHaveValidationErrorFor(nameof(request.PaymentCardDetails));
        }

        [Theory]
        [InlineData("")]
        [InlineData("123456789")]
        [InlineData("4689387567820")]
        public void ValidationFailsWhenCardNumberIsInvalid(string cardNumber)
        {
            var request = GetValidRequest();
            request.PaymentCardDetails!.CardNumber = cardNumber;

            var result = _subject.TestValidate(request);

            result.ShouldHaveValidationErrorFor("PaymentCardDetails.CardNumber");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1899)]
        [InlineData(2101)]
        public void ValidationFailsWhenCardExpiryYearIsInvalid(int year)
        {
            var request = GetValidRequest();
            request.PaymentCardDetails!.ExpiryYear = year;

            var result = _subject.TestValidate(request);

            result.ShouldHaveValidationErrorFor("PaymentCardDetails.ExpiryYear");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        public void ValidationFailsWhenCardExpiryMonthIsInvalid(int month)
        {
            var request = GetValidRequest();
            request.PaymentCardDetails!.ExpiryMonth = month;

            var result = _subject.TestValidate(request);

            result.ShouldHaveValidationErrorFor("PaymentCardDetails.ExpiryMonth");
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("abc")]
        [InlineData("1234")]
        public void ValidationFailsWhenCardCvvIsInvalid(string cvv)
        {
            var request = GetValidRequest();
            request.PaymentCardDetails!.Cvv = cvv;

            var result = _subject.TestValidate(request);

            result.ShouldHaveValidationErrorFor("PaymentCardDetails.Cvv");
        }

        [Fact]
        public void ValidationFailsWhenCurrencyIsInvalid()
        {
            var request = GetValidRequest();
            request.Currency = (Currency)42;

            var result = _subject.TestValidate(request);

            result.ShouldHaveValidationErrorFor(nameof(request.Currency));
        }

        [Fact]
        public void ValidationFailsWhenAmountIsZero()
        {
            var request = GetValidRequest();
            request.Amount = 0;

            var result = _subject.TestValidate(request);

            result.ShouldHaveValidationErrorFor(nameof(request.Amount));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ValidationFailsWhenAmountIsNotValid(int amount)
        {
            var request = GetValidRequest();
            request.Amount = amount;

            var result = _subject.TestValidate(request);

            result.ShouldHaveValidationErrorFor(nameof(request.Amount));
        }

        private static PaymentRequest GetValidRequest() =>
            new PaymentRequest
            {
                PaymentCardDetails = new PaymentCardDetailsRequest
                {
                    CardNumber = TestHelper.ValidCardNumber,
                    ExpiryYear = 2021,
                    ExpiryMonth = 6,
                    Cvv = "123"
                },
                Currency = Currency.EUR,
                Amount = 83898
            };
    }
}
