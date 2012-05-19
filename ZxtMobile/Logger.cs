using System;
using System.Collections.Generic;
using System.Web;

namespace ZxtMobile
{
    public class Logger
    {
        public static void WriteLog(string log)
        {
            try
            {
                string sql = string.Format("insert into android_server_log(log_content) values('{0}')", log);
                DBConfig.GetDBObjcet().ExecuteNonQuery(sql);
            }
            catch { }
        }
    }
}