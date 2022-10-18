using System;
using NUnit.Framework;
using PaymentServices.Services;
using PaymentServices.Types;

namespace Yoox.Tests.Tests
{
    public class PaymentValidatorTests
    {
        private const AllowedPaymentSchemes CHAPS = AllowedPaymentSchemes.Chaps;
        private const AllowedPaymentSchemes BACS = AllowedPaymentSchemes.Bacs;
        private const AllowedPaymentSchemes FASTER = AllowedPaymentSchemes.FasterPayments;

        private const PaymentScheme FASTER_SCHEME = PaymentScheme.FasterPayments;
        private const PaymentScheme BACS_SCHEME = PaymentScheme.Bacs;
        private const PaymentScheme CHAPS_SCHEME = PaymentScheme.Chaps;

        private const AccountStatus LIVE_STATUS = AccountStatus.Live;
        private const AccountStatus DISABLED_STATUS = AccountStatus.Disabled;
        private const AccountStatus INBOUND_STATUS = AccountStatus.InboundPaymentsOnly;

        private const decimal AMMOUNT = 6.0m;

        [Test]
        public void PaymentHasFlagBacs()
        {
            var account = new Account()
            {
                AccountNumber = "123456789",
                Balance = 6.0m,
                AllowedPaymentSchemes = BACS,
                Status = LIVE_STATUS
            };

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = account.AccountNumber,
                DebtorAccountNumber = account.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = BACS_SCHEME
            };

            var paymentValidator = new PaymentValidator();
            var result = paymentValidator.GetResult(account, request);

            Assert.AreEqual(result, true);
        }

        [Test]
        public void PaymentHasNoFlagBacs()
        {
            var account = new Account()
            {
                AccountNumber = "123456789",
                Balance = 6.0m,
                AllowedPaymentSchemes = CHAPS,
                Status = LIVE_STATUS
            };

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = account.AccountNumber,
                DebtorAccountNumber = account.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = BACS_SCHEME
            };

            var paymentValidator = new PaymentValidator();
            var result = paymentValidator.GetResult(account, request);

            Assert.AreEqual(result, false);
        }

        [Test]
        public void AccountBallanceIsLessThanRequestAmount()
        {
            var account = new Account()
            {
                AccountNumber = "123456789",
                Balance = 5.0m,
                AllowedPaymentSchemes = CHAPS,
                Status = LIVE_STATUS
            };

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = account.AccountNumber,
                DebtorAccountNumber = account.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = BACS_SCHEME
            };

            var paymentValidator = new PaymentValidator();
            var result = paymentValidator.GetResult(account, request);

            Assert.IsFalse(result);
        }

        [Test]
        public void AccountStatusIsDifferentThanAccountStatusLive()
        {
            var accountStatusLive = AccountStatus.Live;

            var account = new Account()
            {
                AccountNumber = "123456789",
                Balance = 5.0m,
                AllowedPaymentSchemes = CHAPS,
                Status = DISABLED_STATUS
            };

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = AMMOUNT,
                CreditorAccountNumber = account.AccountNumber,
                DebtorAccountNumber = account.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = BACS_SCHEME
            };

            var paymentValidator = new PaymentValidator();
            var result = paymentValidator.GetResult(account, request);

            Assert.AreNotEqual(accountStatusLive, account.Status);
        }
    }
}

