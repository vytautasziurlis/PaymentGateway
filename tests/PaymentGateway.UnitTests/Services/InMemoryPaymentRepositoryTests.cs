using System;
using System.Threading.Tasks;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Persistence;
using Shouldly;
using Xunit;

namespace PaymentGateway.UnitTests.Services
{
    public class InMemoryPaymentRepositoryTests
    {
        private readonly InMemoryPaymentRepository _subject;

        public InMemoryPaymentRepositoryTests()
        {
            _subject = new InMemoryPaymentRepository();
        }

        [Fact]
        public async Task GetPaymentReturnsNullWhenPaymentDoesNotExist()
        {
            var result = await _subject.GetPayment("42");

            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetPaymentReturnsDataWhenPaymentExists()
        {
            var reference = Guid.NewGuid().ToString();
            var paymentDetails = new PaymentDetails(reference, PaymentStatus.Success,
                TestHelper.GetPaymentCardDetails(), Currency.GBP, 9.99m);
            await _subject.AddPayment(paymentDetails);

            var result = await _subject.GetPayment(reference);

            result.ShouldBeSameAs(paymentDetails);
        }

        [Fact]
        public async Task AddPaymentThrowsExceptionWhenReferenceAlreadyExists()
        {
            var reference = Guid.NewGuid().ToString();
            var paymentDetails = new PaymentDetails(reference, PaymentStatus.Success,
                TestHelper.GetPaymentCardDetails(), Currency.GBP, 9.99m);
            await _subject.AddPayment(paymentDetails);

            await Assert.ThrowsAsync<DuplicatePaymentReferenceException>(() => _subject.AddPayment(paymentDetails));
        }
    }
}
