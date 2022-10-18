using System;
using PaymentServices.Types;

namespace PaymentServices.Services
{
    public interface IPaymentValidator
    {
        bool GetResult(Account account, MakePaymentRequest request);
    }
}

