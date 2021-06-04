using System;

namespace PaymentGateway.Domain
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow() => DateTime.UtcNow;
    }
}
