using System;
using PaymentServices.Types;

namespace PaymentServices.Services
{
    public class PaymentValidator : IPaymentValidator
    {
        public bool GetResult(Account account, MakePaymentRequest request)
        {
            PaymentScheme paymentScheme = request.PaymentScheme;

            string paymentSchemeName = Enum.GetName(typeof(PaymentScheme), paymentScheme);

            AllowedPaymentSchemes allowedPaymentScheme = (AllowedPaymentSchemes)Enum
                .Parse(typeof(AllowedPaymentSchemes), paymentSchemeName);

            if (account == null || !account.AllowedPaymentSchemes.HasFlag(allowedPaymentScheme))
            {
                return false;
            }

            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        return false;
                    }
                    break;
                case PaymentScheme.FasterPayments:
                    if (account.Balance < request.Amount)
                    {
                        return false;
                    }
                    break;

                case PaymentScheme.Chaps:
                    if (account.Status != AccountStatus.Live)
                    {
                        return false;
                    }
                    break;
            }
            
            

            return true;
        }
    }
}

