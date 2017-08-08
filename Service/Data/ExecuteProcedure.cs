using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace Service.Common.Data
{
    /// <summary>
    /// 执行存储过程
    /// </summary>
    public class ExecuteProcedure
    {
        static Database dbInstance;
        /// <summary>
        /// 摘要：执行无返回值存储过程
        /// </summary>
        /// <param name="conString">连接串名称，若默认连接串名称可以为null</param>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameterValues">参数值数组</param>
        /// <returns>影响行数</returns>
        public static int ExeProcedureNonQuery(string conString, string procedureName, params object[] parameterValues)
        {
            dbInstance = DBInstance.CreateInstance(conString);
            if (object.Equals(null, dbInstance))
            {
                return -1;
            }
            int iReturn = dbInstance.ExecuteNonQuery(procedureName, parameterValues);
            return iReturn;
        }
        /// <summary>
        /// 摘要：执行无返回值存储过程
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameterValues">参数值数组</param>
        /// <returns>影响行数</returns>
        public static int ExeProcedureNonQuery(string procedureName, params object[] parameterValues)
        {
            return ExeProcedureNonQuery(null, procedureName, parameterValues);
        }
        /// <summary>
        /// 摘要：执行存储过程获取数据集
        /// </summary>
        /// <param name="conString">连接串名称，若默认连接串名称可以为null</param>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameterValues">参数值数组</param>
        /// <returns>数据集</returns>
        public static DataSet ExeProcedureForDataSet(string conString, string procedureName, params object[] parameterValues)
        {
            dbInstance = DBInstance.CreateInstance(conString);
            if (object.Equals(null, dbInstance))
            {
                return null;
            }
            DbCommand dbcommand = dbInstance.GetStoredProcCommand(procedureName, parameterValues);
            DataSet dsReturn = dbInstance.ExecuteDataSet(dbcommand);
            return dsReturn;
        }
        /// <summary>
        /// 摘要：执行存储过程获取数据集
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameterValues">参数值数组</param>
        /// <returns></returns>
        public static DataSet ExeProcedureForDataSet(string procedureName, params object[] parameterValues)
        {
            return ExeProcedureForDataSet(null, procedureName, parameterValues);
        }
        /// <summary>
        /// 摘要：执行返回参数存储过程
        /// </summary>
        /// <param name="conString">连接串名称，若默认连接串名称可以为null</param>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameterValues">参数值数组</param>
        /// <returns>影响行数</returns>
        public static object ExeProcedureForParameter(string conString, string procedureName, params DbParameter[] commandParameters)
        {
            dbInstance = DBInstance.CreateInstance(conString);
            if (object.Equals(null, dbInstance))
            {
                return null;
            }
            DbCommand cmd = dbInstance.GetStoredProcCommand(procedureName);
            foreach (DbParameter parm in commandParameters)
            {
                if(parm.Value!=null)
                dbInstance.AddInParameter(cmd, parm.ParameterName, parm.DbType, parm.Value);
            }
            dbInstance.AddOutParameter(cmd, commandParameters[commandParameters.Length - 1].ParameterName, commandParameters[commandParameters.Length - 1].DbType, 10);
            dbInstance.ExecuteNonQuery(cmd);
            object iReturn = dbInstance.GetParameterValue(cmd, "return");
            return iReturn;
        }
        /// <summary>
        /// 摘要：执行无返回值存储过程
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameterValues">参数值数组</param>
        /// <returns>影响行数</returns>
        public static object ExeProcedureForParameter(string procedureName, params DbParameter[] commandParameters)
        {
            return ExeProcedureForParameter(null, procedureName, commandParameters);
        }
    }
}
