using System;

namespace PaymentGateway.Domain.Entities
{
    public readonly struct CardExpiry
    {
        public int Year { get; }

        public int Month { get; }

        public CardExpiry(int year, int month)
        {
            if (month is < 1 or > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month), month,
                    "Month must be between 1 and 12");
            }
            if (year is < 1900 or > 2100)
            {
                throw new ArgumentOutOfRangeException(nameof(year), year,
                    "Year must be between 1900 and 2100");
            }

            Year = year;
            Month = month;
        }
    }
}
