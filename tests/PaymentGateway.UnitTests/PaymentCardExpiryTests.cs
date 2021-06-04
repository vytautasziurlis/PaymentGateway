using System;
using PaymentGateway.Domain.Entities;
using Shouldly;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class PaymentCardExpiryTests
    {
        [Fact]
        public void ConstructorSucceedsWhenYearAndMonthAreValid()
        {
            var cardExpiry = new CardExpiry(2021, 6);

            cardExpiry.Year.ShouldBe(2021);
            cardExpiry.Month.ShouldBe(6);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        [InlineData(2000)]
        public void ConstructorThrowsWhenMonthIsInvalid(int month)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CardExpiry(2020, month));
        }

        [Theory]
        [InlineData(1899)]
        [InlineData(2101)]
        public void ConstructorThrowsWhenYearIsInvalid(int year)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CardExpiry(year, 1));
        }
    }
}
