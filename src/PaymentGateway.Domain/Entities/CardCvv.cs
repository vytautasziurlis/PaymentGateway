using System;
using System.Text.RegularExpressions;

namespace PaymentGateway.Domain.Entities
{
    public class CardCvv
    {
        private const string ValidationRegex = "^\\d{3}$";

        public string Value { get; }

        public CardCvv(string cvv)
        {
            if (!IsValid(cvv))
            {
                throw new ArgumentException($"{cvv}'' is not a valid card CVV", nameof(cvv));
            }
            Value = cvv;
        }

        public static explicit operator CardCvv(string cardNumber) => new(cardNumber);

        public override string ToString() => Value;

        public static bool IsValid(string value) =>
            string.IsNullOrWhiteSpace(value) == false &&
            Regex.Match(value, ValidationRegex).Success;
    }
}
