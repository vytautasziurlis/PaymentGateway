using System;

namespace PaymentGateway.Domain
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow();
    }
}
