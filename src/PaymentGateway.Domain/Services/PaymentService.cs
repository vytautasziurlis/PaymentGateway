using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Persistence;

namespace PaymentGateway.Domain.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBankService _bankService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IBankService bankService,
            IPaymentRepository paymentRepository,
            IDateTimeProvider dateTimeProvider,
            ILogger<PaymentService> logger)
        {
            _bankService = bankService;
            _paymentRepository = paymentRepository;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task<PaymentProcessingResult> ProcessPayment(PaymentCardDetails paymentCardDetails,
            Currency currency, decimal amount)
        {
            ValidatePaymentCard(paymentCardDetails);

            var paymentProcessingResult = await _bankService.ProcessPayment(paymentCardDetails,
                currency, amount);

            await PersistPaymentDetails(paymentProcessingResult, paymentCardDetails, currency, amount);

            return paymentProcessingResult;
        }

        public Task<PaymentDetails?> GetPayment(string paymentReference)
        {
            return _paymentRepository.GetPayment(paymentReference);
        }

        private Task PersistPaymentDetails(PaymentProcessingResult paymentProcessingResult,
            PaymentCardDetails paymentCardDetails, Currency currency, decimal amount)
        {
            var paymentDetails = new PaymentDetails(paymentProcessingResult.Reference,
                paymentProcessingResult.Status, paymentCardDetails, currency, amount);
            return _paymentRepository.AddPayment(paymentDetails);
        }

        private void ValidatePaymentCard(PaymentCardDetails paymentCardDetails)
        {
            if (CardIsExpired(paymentCardDetails.CardExpiry))
            {
                _logger.LogWarning("Invalid card expiry: {Year}/{Month}",
                    paymentCardDetails.CardExpiry.Year, paymentCardDetails.CardExpiry.Month);
                throw new PaymentCardExpiredException();
            }
        }

        private bool CardIsExpired(CardExpiry cardExpiry)
        {
            var now = _dateTimeProvider.UtcNow();
            return cardExpiry.Year == now.Year && cardExpiry.Month < now.Month ||
                   cardExpiry.Year < now.Year;
        }
    }
}
