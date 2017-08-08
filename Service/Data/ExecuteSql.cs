using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using Service.Common.Log;

namespace Service.Common.Data
{
    /// <summary>
    /// 执行SQL语句
    /// </summary>
    public class ExecuteSql
    {
        static Database dbInstance;
        static Exception NoInstanceException = new Exception("DataBase实例不存在，请确认调用CreateInstance静态方法");
        #region 执行不带参数的SQL语句
        /// <summary>
        /// 摘要：执行无返回值一般SQL语句
        /// </summary>
        /// <param name="conString">连接串名称，若默认连接串名称可以为null</param>
        /// <param name="sql">SQL语句</param>
        /// <returns>影响行数</returns>
        public static int ExeComSqlForNonQuery(string conString, string sql)
        {
            dbInstance = DBInstance.CreateInstance(conString);
            return dbInstance.ExecuteNonQuery(CommandType.Text, sql); ;
        }
        /// <summary>
        /// 摘要：执行无返回值一般SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>影响行数</returns>
        public static int ExeComSqlForNonQuery(string sql)
        {
            return ExeComSqlForNonQuery(null, sql);
        }
        /// <summary>
        /// 摘要：执行一般SQL语句返回数据集 
        /// </summary>
        /// <param name="conString">连接串名称，若默认连接串名称可以为null</param>
        /// <param name="sql">SQL语句</param>
        /// <returns>数据集 </returns>
        public static DataSet ExeComSqlForDataSet(string conString, string sql)
        {
            dbInstance = DBInstance.CreateInstance(conString);
            return dbInstance.ExecuteDataSet(CommandType.Text, sql);
        }
        /// <summary>
        /// 摘要：执行一般SQL语句返回数据集 
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>数据集 </returns>
        public static DataSet ExeComSqlForDataSet(string sql)
        {
            return ExeComSqlForDataSet(null, sql);
        }
        /// <summary>
        /// 摘要：执行一般SQL语句返回聚合函数值（第一行第一列） 
        /// </summary>
        /// <param name="conString">连接串名称，若默认连接串名称可以为null</param>
        /// <param name="sql">SQL语句</param>
        /// <returns>数据集第一行第一列</returns>
        public static object ExeComSqlForScalar(string conString, string sql)
        {
            dbInstance = DBInstance.CreateInstance(conString);
            return dbInstance.ExecuteScalar(CommandType.Text, sql);
        }
        /// <summary>
        /// 摘要：执行一般SQL语句返回聚合函数值（第一行第一列） 
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>数据集第一行第一列</returns>
        public static object ExeComSqlForScalar(string sql)
        {
            return ExeComSqlForScalar(null, sql);
        }
        #endregion

        #region 执行带参数的SQL语句
        /// <summary>
        /// 配置sql语句的参数
        /// </summary>
        /// <param name="stVariables">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <param name="cmd"></param>
        private static void CommandParametersAdd(string[] stVariables, object[] parameterValues, ref DbCommand cmd, bool isbool = false)
        {
            for (int i = 0; i < stVariables.Length; i++)
            {
                DbParameter DbP = cmd.CreateParameter() as DbParameter;
                DbP.DbType = DbType.String;
                DbP.ParameterName = stVariables[i];
                if (isbool && DbP.ParameterName == "@return")
                    DbP.Direction = ParameterDirection.ReturnValue;
                DbP.Value = parameterValues[i];
                cmd.Parameters.Add(DbP);
            }
        }
        /// <summary>
        /// 配置sql语句的参数
        /// </summary>
        /// <param name="stVariables">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <param name="cmd"></param>
        private static void CommandParametersAdd(List<string> stVariables, List<object> parameterValues, ref DbCommand cmd)
        {
            for (int i = 0; i < stVariables.Count; i++)
            {
                DbParameter DbP = cmd.CreateParameter() as DbParameter;
                
                if (parameterValues[i].GetType().FullName=="System.Byte[]")
                {
                    DbP.DbType = DbType.Binary;
                }
                else
                {
                    DbP.DbType = DbType.String;
                }
                DbP.ParameterName = stVariables[i];
                DbP.Value = parameterValues[i];
                cmd.Parameters.Add(DbP);
            }
        }
        /// <summary>
        /// 摘要：执行无返回值固定个数参数SQL语句（不带事务）
        /// </summary>
        /// <param name="conString">连接串名称，若默认连接串名称可以为null</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameterNames">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns>影响行数</returns>
        public static int ExeParaSqlForNonQuery(string conString, string sql, string[] parameterNames, params object[] parameterValues)
        {
            dbInstance = DBInstance.CreateInstance(conString);
            int iReturn = 0;
            DbConnection dbconnection = dbInstance.CreateConnection() as DbConnection;
            DbCommand dbcommand = dbconnection.CreateCommand() as DbCommand;
            dbcommand.Connection = dbconnection;
            dbconnection.Open();
            try
            {
                dbcommand.CommandText = sql;
                CommandParametersAdd(parameterNames, parameterValues, ref dbcommand);
                iReturn = dbcommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                dbconnection.Close();
            }
            return iReturn;
        }
        /// <summary>
        /// 摘要：执行无返回值固定个数参数SQL语句（不带事务）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameterNames">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns>影响行数</returns>
        public static int ExeParaSqlForNonQuery(string sql, string[] parameterNames, params object[] parameterValues)
        {
            return ExeParaSqlForNonQuery(null, sql, parameterNames, parameterValues);
        }
        /// <summary>
        /// 摘要：执行无返回值不固定个数参数SQL语句（不带事务）
        /// </summary>
        /// <param name="conString">连接串名称，若默认连接串名称可以为null</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameterNames">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns>影响行数</returns>
        public static int ExeParaSqlForNonQuery(string conString, string sql, List<string> parameterNames, List<object> parameterValues)
        {
            dbInstance = DBInstance.CreateInstance(conString);
            int iReturn = 0;
            DbConnection dbconnection = dbInstance.CreateConnection() as DbConnection;
            DbCommand dbcommand = dbconnection.CreateCommand() as DbCommand;
            dbcommand.Connection = dbconnection;
            dbconnection.Open();
            try
            {
                dbcommand.CommandText = sql;
                CommandParametersAdd(parameterNames, parameterValues, ref dbcommand);
                iReturn = dbcommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                dbconnection.Close();
            }
            return iReturn;
        }
        /// <summary>
        /// 摘要：执行无返回值固定个数参数SQL语句（不带事务）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameterNames">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns>影响行数</returns>
        public static int ExeParaSqlForNonQuery(string sql, List<string> parameterNames, List<object> parameterValues)
        {
            return ExeParaSqlForNonQuery(null, sql, parameterNames, parameterValues);
        }
        /// <summary>
        /// 摘要：执行无返回值固定个数参数SQL语句(带事务)
        /// </summary> 
        /// <param name="sql">SQL语句</param>
        /// <param name="conString">数据库连接</param>
        /// <param name="transaction">添加事务</param>
        /// <param name="parameterNames">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns>影响行数</returns>
        public static int ExeParaSqlForNonQuery(string sql, DbConnection dbConnection, DbTransaction transaction, string[] parameterNames, params object[] parameterValues)
        {
            DbCommand dbcommand = dbConnection.CreateCommand() as DbCommand;
            dbcommand.Connection = dbConnection;
            dbcommand.Transaction = transaction;
            dbcommand.CommandText = sql;
            CommandParametersAdd(parameterNames, parameterValues, ref dbcommand);
            return dbcommand.ExecuteNonQuery();
        }
        /// <summary>
        /// 摘要：执行无返回值参数SQL语句
        /// </summary>
        /// <param name="conString">连接串名称，若默认连接串名称可以为null</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameterNames">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns>影响行数</returns>
        public static int ExeParaSqlForScalar(string conString, string sql, string[] parameterNames, params object[] parameterValues)
        {
            dbInstance = DBInstance.CreateInstance(conString);
            int iResult = 0;
            DbConnection dbconnection = dbInstance.CreateConnection() as DbConnection;
            DbCommand dbcommand = dbconnection.CreateCommand() as DbCommand;
            dbcommand.Connection = dbconnection;
            dbconnection.Open();
            try
            {
                dbcommand.CommandText = sql;
                CommandParametersAdd(parameterNames, parameterValues, ref dbcommand);
                iResult = (int)dbcommand.ExecuteScalar();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                dbconnection.Close();
            }
            return iResult;
        }
        /// <summary>
        /// 摘要：执行无返回值不固定个数参数SQL语句（不带事务）
        /// </summary>
        /// <param name="conString">连接串名称，若默认连接串名称可以为null</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameterNames">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns>影响行数</returns>
        public static int ExeParaSqlForScalar(string conString, string sql, List<string> parameterNames, List<object> parameterValues)
        {
            dbInstance = DBInstance.CreateInstance(conString);
            int iReturn = 0;
            DbConnection dbconnection = dbInstance.CreateConnection() as DbConnection;
            DbCommand dbcommand = dbconnection.CreateCommand() as DbCommand;
            dbcommand.Connection = dbconnection;
            dbconnection.Open();
            try
            {
                if (!sql.ToUpper().Contains("COUNT"))
                    sql = "Select Count(*) " + sql.Substring(sql.ToUpper().IndexOf("FROM"));
                dbcommand.CommandText = sql;
                CommandParametersAdd(parameterNames, parameterValues, ref dbcommand);
                iReturn = (int)dbcommand.ExecuteScalar();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                dbconnection.Close();
            }
            return iReturn;
        }
        /// <summary>
        /// 摘要：执行无返回值参数SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameterNames">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns>影响行数</returns>
        public static int ExeParaSqlForScalar(string sql, List<string> parameterNames, List<object> parameterValues)
        {
            return ExeParaSqlForScalar(null, sql, parameterNames, parameterValues);
        }
        /// <summary>
        /// 摘要：执行无返回值参数SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameterNames">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns>影响行数</returns>
        public static int ExeParaSqlForScalar(string sql, string[] parameterNames, params object[] parameterValues)
        {
            return ExeParaSqlForScalar(null, sql, parameterNames, parameterValues);
        }
        /// <summary>
        /// 摘要：执行参数SQL语句返回数据集
        /// </summary>
        /// <param name="conString">连接串名称，若默认连接串名称可以为null</param>
        /// <param name="sql">SELECT SQL语句</param>
        /// <param name="parameterNames">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns>数据集</returns>
        public static DataSet ExeParaSqlForDataSet(string conString, string sql, string[] parameterNames, params object[] parameterValues)
        {
            DataSet dsResult = new DataSet();
            dbInstance = DBInstance.CreateInstance(conString);
            DbConnection dbconnection = dbInstance.CreateConnection() as DbConnection;
            DbCommand dbcommand = dbconnection.CreateCommand() as DbCommand;
            dbcommand.Connection = dbconnection;
            dbconnection.Open();
            try
            {
                dbcommand.CommandText = sql;
                CommandParametersAdd(parameterNames, parameterValues, ref dbcommand);
                dsResult = dbInstance.ExecuteDataSet(dbcommand);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                dbconnection.Close();
            }
            return dsResult;
        }
        public static DataSet ExeParaSqlForDataSet(string sql, string[] parameterNames, params object[] parameterValues)
        {
            return ExeParaSqlForDataSet(null, sql, parameterNames, parameterValues);
        }
        /// <summary>
        /// 摘要：执行返回数据集不固定个数参数SQL语句（不带事务）
        /// </summary>
        /// <param name="conString">连接串名称，若默认连接串名称可以为null</param>
        /// <param name="sql">SELECT SQL语句</param>
        /// <param name="parameterNames">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns>数据集</returns>
        public static DataSet ExeParaSqlForDataSet(string conString, string sql, List<string> parameterNames, List<object> parameterValues)
        {
            DataSet dsResult = new DataSet();
            dbInstance = DBInstance.CreateInstance(conString);
            if (object.Equals(null, dbInstance))
            {
                return null;
            }
            DbConnection dbconnection = dbInstance.CreateConnection() as DbConnection;
            DbCommand dbcommand = dbconnection.CreateCommand() as DbCommand;
            dbcommand.Connection = dbconnection;
            dbconnection.Open();
            try
            {
                dbcommand.CommandText = sql;
                CommandParametersAdd(parameterNames, parameterValues, ref dbcommand);
                dsResult = dbInstance.ExecuteDataSet(dbcommand);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                dbconnection.Close();
            }
            return dsResult;
        }
        /// <summary>
        /// 摘要：执行返回数据集不固定个数参数SQL语句（不带事务）
        /// </summary>
        /// <param name="sql">SELECT SQL语句</param>
        /// <param name="parameterNames">参数名称</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns>数据集</returns>
        public static DataSet ExeParaSqlForDataSet(string sql, List<string> parameterNames, List<object> parameterValues)
        {
            return ExeParaSqlForDataSet(null, sql, parameterNames, parameterValues);
        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="parameterValues">参数</param>
        /// <returns>影响行数</returns>
        public static int ExeProcedureNonQuery(string procedureName, params object[] parameterValues)
        {
            ReturnMessage rm = new ReturnMessage(true);
            if (object.Equals(null, dbInstance))
                throw NoInstanceException;
            int iReturn = dbInstance.ExecuteNonQuery(procedureName, parameterValues);
            return iReturn;
        }
        public static int ExeProcedureNonQuery(string procedureName, string[] parameterNames, params object[] parameterValues)
        {
            dbInstance = DBInstance.CreateInstance(null);
            int iResult = 0;
            DbConnection dbconnection = dbInstance.CreateConnection() as DbConnection;
            DbCommand dbcommand = dbconnection.CreateCommand() as DbCommand;
            dbcommand.Connection = dbconnection;
            dbconnection.Open();
            try
            {
                dbcommand.CommandText = procedureName;
                dbcommand.CommandType = CommandType.StoredProcedure;
                CommandParametersAdd(parameterNames, parameterValues, ref dbcommand, true);
                //iResult = (int)dbcommand.ExecuteScalar();
                dbcommand.ExecuteScalar();
                iResult = (int)dbcommand.Parameters["@return"].Value;
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                dbconnection.Close();
            }
            return iResult;
        }
        #endregion


    }
}
