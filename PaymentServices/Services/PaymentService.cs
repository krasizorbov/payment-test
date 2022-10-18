using PaymentServices.Data;
using PaymentServices.Types;
using System;


namespace PaymentServices.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IDataStore _dataStore;
        private readonly IPaymentValidator _paymentValidator;

        public PaymentService(IDataStore dataStore, IPaymentValidator paymentValidator)
        {
            _dataStore = dataStore;
            _paymentValidator = paymentValidator;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var account = _dataStore.GetAccount(request.DebtorAccountNumber);

            var result = new MakePaymentResult();

            result.Success = _paymentValidator.GetResult(account, request);

            if(result.Success)
            {
                account.Balance -= request.Amount;
                _dataStore.UpdateAccount(account);
            }

            return result;
        }
    }
}
