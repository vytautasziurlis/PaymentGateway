using System;
using PaymentGateway.API.Models;
using PaymentGateway.Domain.Commands;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.API.Mappings
{
    public class Mapper : IMapper
    {
        public ProcessPaymentCommand Map(PaymentRequest request)
        {
            if (request.PaymentCardDetails == null)
            {
                throw new ArgumentNullException(nameof(request.PaymentCardDetails),
                    "PaymentCardDetails cannot be null");
            }

            return new ProcessPaymentCommand(
                new PaymentCardDetails(
                    new CardNumber(request.PaymentCardDetails.CardNumber),
                    new CardExpiry(request.PaymentCardDetails.ExpiryYear, request.PaymentCardDetails.ExpiryMonth),
                    new CardCvv(request.PaymentCardDetails.Cvv)),
                request.Currency,
                request.Amount);
        }


        public PaymentResponse Map(PaymentProcessingResult processingResult) =>
            new PaymentResponse
            {
                Reference = processingResult.Reference,
                Status = processingResult.Status
            };

        public PaymentDetailsResponse Map(PaymentDetails paymentDetails) =>
            new PaymentDetailsResponse
            {
                Reference = paymentDetails.Reference,
                Status = paymentDetails.Status,
                PaymentCardDetails = new PaymentCardDetailsResponse
                {
                    CardNumber = paymentDetails.CardDetails.CardNumber.MaskedValue.Value,
                    ExpiryYear = paymentDetails.CardDetails.CardExpiry.Year,
                    ExpiryMonth = paymentDetails.CardDetails.CardExpiry.Month
                },
                Currency = paymentDetails.Currency,
                Amount = paymentDetails.Amount
            };
    }
}
