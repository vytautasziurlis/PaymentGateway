using FluentValidation;
using PaymentGateway.API.Models;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.API.Validation
{
    public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
    {
        public PaymentRequestValidator()
        {
            RuleFor(x => x.PaymentCardDetails)
                .NotNull()
                .WithMessage("paymentCardDetails is required");

            When(x => x.PaymentCardDetails != null, () =>
            {
                RuleFor(x => x.PaymentCardDetails!.CardNumber)
                    .NotEmpty()
                    .WithMessage("cardNumber is required")
                    .Must(CardNumber.IsValid)
                    .WithMessage("Invalid cardNumber");

                RuleFor(x => x.PaymentCardDetails!.ExpiryYear)
                    .NotEmpty()
                    .WithMessage("expiryYear is required")
                    .InclusiveBetween(1900, 2100)
                    .WithMessage("expiryYear must be between 1900 and 2100");

                RuleFor(x => x.PaymentCardDetails!.ExpiryMonth)
                    .NotEmpty()
                    .WithMessage("expiryMonth is required")
                    .InclusiveBetween(1, 12)
                    .WithMessage("expiryMonth must be between 1 and 12");

                RuleFor(x => x.PaymentCardDetails!.Cvv)
                    .NotEmpty()
                    .WithMessage("cvv is required")
                    .Must(CardCvv.IsValid)
                    .WithMessage("Invalid cvv");
            });

            RuleFor(x => x.Currency)
                .IsInEnum()
                .WithMessage("Invalid currency");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("amount must be greater than 0");
        }
    }
}
