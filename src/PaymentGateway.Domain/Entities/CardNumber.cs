using System;
using System.Text.RegularExpressions;
using CreditCardValidator;

namespace PaymentGateway.Domain.Entities
{
    public class CardNumber
    {
        private const string ValidationRegex = "^\\d{9,16}$";

        public string Value { get; }

        public Lazy<string> MaskedValue =>
            new Lazy<string>(new string('*', Value.Length - 4) + Value[^4..]);

        public CardNumber(string cardNumber)
        {
            if (!IsValid(cardNumber))
            {
                throw new ArgumentException($"'{cardNumber}' is not a valid card number",
                    nameof(cardNumber));
            }
            Value = cardNumber;
        }

        public static explicit operator CardNumber(string cardNumber) => new(cardNumber);

        public override string ToString() => Value;

        public static bool IsValid(string value) =>
            string.IsNullOrWhiteSpace(value) == false &&
            Regex.Match(value, ValidationRegex).Success &&
            Luhn.CheckLuhn(value);
    }
}
