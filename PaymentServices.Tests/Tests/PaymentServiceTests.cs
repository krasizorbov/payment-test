using System;
using NUnit.Framework;
using PaymentServices.Types;
using PaymentServices.Data;
using PaymentServices.Services;

namespace PaymentServices.Tests.Tests
{
    public class PaymentServiceTests
    {
        private const string ACCOUNT_NUMBER_FIRST = "123466779";
        private const string ACCOUNT_NUMBER_SECOND = "122456779";
        private const string ACCOUNT_NUMBER_THIRD = "333456799";

        private const decimal AMMOUNT = 6.0m;
        private const string DEBTOR_ACCOUNT_NUMBER = "1234";

        private const AllowedPaymentSchemes CHAPS = AllowedPaymentSchemes.Chaps;
        private const AllowedPaymentSchemes BACS = AllowedPaymentSchemes.Bacs;
        private const AllowedPaymentSchemes FASTER = AllowedPaymentSchemes.FasterPayments;

        private const PaymentScheme FASTER_SCHEME = PaymentScheme.FasterPayments;
        private const PaymentScheme BACS_SCHEME = PaymentScheme.Bacs;
        private const PaymentScheme CHAPS_SCHEME = PaymentScheme.Chaps;

        private const AccountStatus LIVE_STATUS = AccountStatus.Live;
        private const AccountStatus DISABLED_STATUS = AccountStatus.Disabled;
        private const AccountStatus INBOUND_STATUS = AccountStatus.InboundPaymentsOnly;

        private Account firstAccount;
        private Account secondAccount;
        private Account thirdAccount;

        private IDataStore dataStore;
        private IPaymentValidator paymentValidator;
        private PaymentService service;

        [SetUp]
        public void Setup()
        {
            firstAccount = new Account()
            {
                AccountNumber = ACCOUNT_NUMBER_FIRST,
                Balance = AMMOUNT,
                AllowedPaymentSchemes = CHAPS,
                Status = LIVE_STATUS
            };

            secondAccount = new Account()
            {
                AccountNumber = ACCOUNT_NUMBER_SECOND,
                Balance = AMMOUNT,
                AllowedPaymentSchemes = FASTER,
                Status = LIVE_STATUS
            };

            thirdAccount = new Account()
            {
                AccountNumber = ACCOUNT_NUMBER_THIRD,
                Balance = AMMOUNT,
                AllowedPaymentSchemes = BACS,
                Status = LIVE_STATUS
            };

            var accountDataStore = DataStoreHelper.GetDataStore();
            paymentValidator = new PaymentValidator();

            dataStore = DataStoreHelper.GetDataStore();
            dataStore.Accounts.AddRange(new Account[] { firstAccount, secondAccount, thirdAccount });

            service = new PaymentService(accountDataStore, paymentValidator);

            accountDataStore.Accounts.AddRange(new Account[] { firstAccount, secondAccount, thirdAccount });
        }

        [Test]
        public void PaymentFailsWhenPaymentIsNotAllowed()
        {
            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = firstAccount.AccountNumber,
                DebtorAccountNumber = firstAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = FASTER_SCHEME
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsFalse(result.Success);
        }

        [Test]
        [TestCase(PaymentScheme.Chaps, 5.1d, AccountStatus.Disabled)]
        [TestCase(PaymentScheme.FasterPayments, 55.2d, AccountStatus.Live)]
        public void PaymentFailsWhenExtraConditionsAreFalse(PaymentScheme scheme, decimal amount, AccountStatus status)
        {
            firstAccount.Status = status;

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = amount,
                CreditorAccountNumber = firstAccount.AccountNumber,
                DebtorAccountNumber = firstAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = scheme
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void PaymentFailsWhenUserIsNull()
        {
            string accountNumber = "something";

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = accountNumber,
                DebtorAccountNumber = DEBTOR_ACCOUNT_NUMBER,
                PaymentDate = DateTime.Now,
                PaymentScheme = CHAPS_SCHEME
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void PaymentSuccessfulBacs()
        {
            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = firstAccount.AccountNumber,
                DebtorAccountNumber = thirdAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = BACS_SCHEME
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsTrue(result.Success);
        }

        [Test]
        [TestCase(ACCOUNT_NUMBER_FIRST, CHAPS_SCHEME)]
        [TestCase(ACCOUNT_NUMBER_SECOND, FASTER_SCHEME)]
        [TestCase(ACCOUNT_NUMBER_THIRD, BACS_SCHEME)]
        public void PaymentWithdrawsCorrectly(string accountNumber, PaymentScheme scheme)
        {
            decimal expected = 1.0m;

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = 5m,
                CreditorAccountNumber = thirdAccount.AccountNumber,
                DebtorAccountNumber = accountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = scheme
            };

            MakePaymentResult result = service.MakePayment(request);

            Account account = dataStore.GetAccount(accountNumber);

            Assert.AreEqual(expected, account.Balance);
        }

        [Test]
        public void PaymentSuccessfulFaster()
        {
            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = thirdAccount.AccountNumber,
                DebtorAccountNumber = secondAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = FASTER_SCHEME
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsTrue(result.Success);
        }

        [Test]
        public void PaymentSuccessfulChaps()
        {
            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = thirdAccount.AccountNumber,
                DebtorAccountNumber = firstAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = CHAPS_SCHEME
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsTrue(result.Success);
        }
    }
}

