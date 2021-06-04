using System;
using PaymentGateway.Domain.Entities;
using Shouldly;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class CardCvvTests
    {
        [Theory]
        [InlineData("000")]
        [InlineData("123")]
        public void ConstructorSucceedsWhenCardCvvIsValid(string cvv)
        {
            var subject = new CardCvv(cvv);

            subject.Value.ShouldBe(cvv);
        }

        [Theory]
        [InlineData("000")]
        [InlineData("123")]
        [InlineData("735")]
        public void IsValidReturnsTrueWhenCardCvvIsValid(string cvv)
        {
            CardCvv.IsValid(cvv).ShouldBeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abc")]
        [InlineData("12")]
        [InlineData("1234")]
        public void IsValidReturnsFalseWhenCardCvvIsInvalid(string cvv)
        {
            CardCvv.IsValid(cvv).ShouldBeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abc")]
        [InlineData("12")]
        [InlineData("1234")]
        public void ConstructorThrowsExceptionWhenCardCvvIsInvalid(string cvv)
        {
            Assert.Throws<ArgumentException>(() => new CardCvv(cvv));
        }
    }
}
