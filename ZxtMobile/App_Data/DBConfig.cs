using System;

namespace ZxtMobile
{
    public class DBConfig
    {
        public static string connectionString = "";
        public static string dbType = "";
        static DBConfig()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
            dbType = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectString"].ProviderName;
        }

        public static IDataBase GetDBObjcet()
        {
            if (dbType == "Oracle")
            {
                return new OracleData(connectionString);
            }
            return new SQLServerData(connectionString);
        }
    }
}
