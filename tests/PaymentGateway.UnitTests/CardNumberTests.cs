using System;
using PaymentGateway.Domain.Entities;
using Shouldly;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class CardNumberTests
    {
        [Theory]
        [InlineData("4435956745108548")]
        [InlineData("6479044318331490")]
        [InlineData("5094374184912280")]
        [InlineData("6280209982074556")]
        public void ConstructorSucceedsWhenCardNumberIsValid(string cardNumber)
        {
            var subject = new CardNumber(cardNumber);

            subject.Value.ShouldBe(cardNumber);
        }

        [Theory]
        [InlineData("4435956745108548")]
        [InlineData("6479044318331490")]
        [InlineData("5094374184912280")]
        [InlineData("6280209982074556")]
        public void IsValidReturnsTrueWhenCardNumberIsValid(string cardNumber)
        {
            CardNumber.IsValid(cardNumber).ShouldBeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abcdefghijklmnop")]
        [InlineData("12345678")]
        [InlineData("4435956745108540")]
        [InlineData("12345678234234234233")]
        public void IsValidReturnsFalseWhenCardNumberIsInvalid(string cardNumber)
        {
            CardNumber.IsValid(cardNumber).ShouldBeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abcdefghijklmnop")]
        [InlineData("12345678")]
        [InlineData("4435956745108540")]
        [InlineData("12345678234234234233")]
        public void ConstructorThrowsExceptionWhenCardNumberIsInvalid(string cardNumber)
        {
            Assert.Throws<ArgumentException>(() => new CardNumber(cardNumber));
        }

        [Theory]
        [InlineData("4953089013607", "*********3607")]
        [InlineData("6479044318331490", "************1490")]
        public void MaskedValueReturnsMaskedCardNumber(string cardNumber, string expectedMaskedNumber)
        {
            var result = new CardNumber(cardNumber);

            result.MaskedValue.Value.ShouldBe(expectedMaskedNumber);
        }
    }
}
