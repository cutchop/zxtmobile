using System;

namespace ZxtMobile
{
    public interface IDataBase
    {
        /// <summary>
        /// 执行一条SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>返回数据库操作受影响的行数</returns>
        int ExecuteNonQuery(string sql);
        
        /// <summary>
        /// 执行多条SQL语句(在同一事务下处理)
        /// </summary>
        /// <param name="sqls">SQL语句</param>
        /// <returns>事务提交返回true,事务回滚返回false</returns>
        bool ExecuteNonQuery(string[] sqls);
        
        /// <summary>
        /// 执行查询,返回结果集
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>结果集DataSet</returns>
        System.Data.DataSet ExecuteReturnDataSet(string sql);
    }
}
