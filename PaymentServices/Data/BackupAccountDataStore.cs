using System.Collections.Generic;
using System.Linq;
using PaymentServices.Types;

namespace PaymentServices.Data
{
    public class BackupAccountDataStore : IDataStore
    {
        public BackupAccountDataStore()
        {
            Accounts = new List<Account>();
        }

        public List<Account> Accounts { get; set; }

        public Account GetAccount(string accountNumber)
        {
            Account account = Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

            return account;
        }

        public void UpdateAccount(Account account)
        {
            
        }
    }
}
