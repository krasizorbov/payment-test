using System.Configuration;
using PaymentServices.Types;

namespace PaymentServices.Data
{
    public static class DataStoreHelper
    {
        public static IDataStore GetDataStore()
        {
            var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];

            if (dataStoreType == "Backup")
            {
                return new BackupAccountDataStore();
            }
            else
            {
                return new AccountDataStore();
            }
        }
    }
}

