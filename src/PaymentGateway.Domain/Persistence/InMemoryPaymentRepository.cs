using System.Collections.Generic;
using System.Threading.Tasks;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Domain.Persistence
{
    public class InMemoryPaymentRepository : IPaymentRepository
    {
        private readonly Dictionary<string, PaymentDetails> _store;

        public InMemoryPaymentRepository()
        {
            _store = new Dictionary<string, PaymentDetails>();
        }

        public Task AddPayment(PaymentDetails paymentDetails)
        {
            if (_store.ContainsKey(paymentDetails.Reference))
            {
                throw new DuplicatePaymentReferenceException();
            }

            _store.Add(paymentDetails.Reference, paymentDetails);
            return Task.CompletedTask;
        }

        public Task<PaymentDetails?> GetPayment(string paymentReference) =>
            _store.ContainsKey(paymentReference)
                ? Task.FromResult<PaymentDetails?>(_store[paymentReference])
                : Task.FromResult<PaymentDetails?>(null);
    }
}
