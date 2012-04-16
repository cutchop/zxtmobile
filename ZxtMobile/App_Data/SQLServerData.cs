using System;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace ZxtMobile
{
    public class SQLServerData : IDataBase
    {
        protected SqlCommand _Command;
        protected SqlConnection _Connection;

        /// <summary>
        /// 构造方法,初始化
        /// </summary>
        /// <param name="source">连接字符串</param>
        public SQLServerData(string source)
        {
            this._Connection = new SqlConnection(source);
            this._Command = new SqlCommand();
            this._Command.Connection = this._Connection;
            this._Command.CommandType = CommandType.Text;
        }
        /// <summary>
        /// 执行一条SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>返回数据库操作受影响的行数</returns>
        public int ExecuteNonQuery(string sql)
        {
            int ret;
            try
            {
                int rowCount = 0;
                this._Command.CommandText = sql;
                this._Connection.Open();
                rowCount = this._Command.ExecuteNonQuery();
                ret = rowCount;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (this._Connection.State != ConnectionState.Closed)
                {
                    this._Connection.Close();
                }
            }
            return ret;
        }
        /// <summary>
        /// 执行多条SQL语句(在同一事务下处理)
        /// </summary>
        /// <param name="sqls">SQL语句</param>
        /// <returns>事务提交返回true,事务回滚返回false</returns>
        public bool ExecuteNonQuery(string[] sqls)
        {
            bool ret = false;
            if (sqls.Length == 0)
            {
                return false;
            }
            else if (sqls.Length == 1)
            {
                if (ExecuteNonQuery(sqls[0]) > 0)
                    ret = true;
            }
            else
            {
                string errSql = "";
                this._Connection.Open();
                //事务开始
                SqlTransaction transaction = this._Connection.BeginTransaction(IsolationLevel.ReadCommitted);
                this._Command.Transaction = transaction;
                try
                {
                    //循环执行SQL语句
                    foreach (string sql in sqls)
                    {
                        errSql = sql;
                        this._Command.CommandText = sql;
                        this._Command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    ret = true;
                }
                catch (Exception ex)
                {
                    //发生异常,事务回滚,抛出异常
                    transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    if (this._Connection.State != ConnectionState.Closed)
                    {
                        this._Connection.Close();
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 执行查询,返回结果集
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>结果集DataSet</returns>
        public DataSet ExecuteReturnDataSet(string sql)
        {
            DataSet ret = new DataSet();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, this._Connection);
                adapter.Fill(ret);
                adapter.Dispose();
                adapter = null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (this._Connection.State != ConnectionState.Closed)
                {
                    this._Connection.Close();
                }
            }
            return ret;
        }
    }
}
