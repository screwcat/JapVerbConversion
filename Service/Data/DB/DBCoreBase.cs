using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using Service.Common.Entity;
using System.Reflection;
using Service.Common.DB.Core;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Service.Common.DB
{
    /// <summary>
    /// 关系型数据库操作基类
    /// </summary>
    public class DBCoreBase : IDBCore
    {
        #region 数据库关键字

        #region 基本参数
        /// <summary>
        /// 摘要：开始转义字符
        /// </summary>
        protected string _Transfer_B = string.Empty;
        public string Transfer_B
        {
            get { return _Transfer_B; }
        }
        /// <summary>
        /// 摘要：结束转义字符
        /// </summary>
        protected string _Transfer_E = string.Empty;
        public string Transfer_E
        {
            get { return _Transfer_E; }
        }
        /// <summary>
        /// 摘要：程序参数SQL语句中的变量前置符号
        /// </summary>
        protected string _VariableC = string.Empty;
        public string VariableC
        {
            get { return _VariableC; }
        }
        /// <summary>
        /// 摘要：获取前几行的sql语句(两个参数，第一个是表名，第二个是前几行的数值)
        /// </summary>
        protected string _FrontDataRow = string.Empty;
        public string FrontDataRow
        {
            get { return _FrontDataRow; }
        }
        /// <summary>
        /// 摘要：获取数据库用户表和视图集合的sql语句
        /// </summary>
        protected string _SelectUserTables = string.Empty;
        public string SelectUserTables
        {
            get { return _SelectUserTables; }
        }
        /// <summary>
        /// 摘要：获取数据库用户表或视图列名集合的sql语句（一个参数，表名）
        /// </summary>
        protected string _SelectUserColumns = string.Empty;
        public string SelectUserColumns
        {
            get { return _SelectUserColumns; }
        }

        #endregion

        #endregion

        #region 字段
        /// <summary>
        /// 传递给子类的行信息
        /// </summary>
        protected List<Record> _Records = new List<Record>();
        protected string _TableName = string.Empty;
        protected string _FrontTableName = string.Empty;
        #endregion

        #region 属性
        /// <summary>
        /// 事务绑定
        /// </summary>
        protected DbTransaction _Transaction = null;
        public DbTransaction Transaction
        {
            get { return _Transaction; }
            set { _Transaction = value; }
        }
        /// <summary>
        /// 摘要：连接串
        /// </summary>
        protected string _ConnectionString = string.Empty;
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }
        /// <summary>
        /// 摘要：驱动
        /// </summary>
        protected string _DBprovider = string.Empty;
        public string DBprovider
        {
            get { return _DBprovider; }
            set { _DBprovider = value; }
        }
        /// <summary>
        /// 摘要：获取表默认行数
        /// </summary>
        protected int _RowCount = 1;
        public int RowCount
        {
            get { return _RowCount; }
            set { _RowCount = value; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 摘要：插入一条记录
        /// </summary>
        /// <param name="T">实体类对象</param>
        /// <returns>自增长主键时返回主键值</returns>
        public virtual string Insert(object T)
        {
            _TableName = ((TableAttribute)T.GetType().GetCustomAttributes(true)[0]).TableName;
            PropertyInfo[] propertyInfos = T.GetType().GetProperties();
            string stTempColumnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                Record rRow = new Record();
                if (propertyInfo.GetCustomAttributes(true).Length != 0)
                {
                    if (propertyInfo.GetCustomAttributes(true)[0].GetType().Name != "PrimaryKeyAttribute")
                    {
                        if (!((PropertyAttribute)(propertyInfo.GetCustomAttributes(true)[0])).IsStorage) continue;
                        if (((PropertyAttribute)(propertyInfo.GetCustomAttributes(true)[0])).ColumnName == string.Empty)
                        {
                            stTempColumnName = propertyInfo.Name;
                        }
                        else
                        {
                            stTempColumnName = ((PropertyAttribute)(propertyInfo.GetCustomAttributes(true)[0])).ColumnName;
                        }
                    }
                    else
                    {
                        rRow.IsPK = 1;
                        if (!((PrimaryKeyAttribute)propertyInfo.GetCustomAttributes(true)[0]).IsAuto)
                        {
                            stTempColumnName = ((PrimaryKeyAttribute)(propertyInfo.GetCustomAttributes(true)[0])).ColumnName;
                        }
                        else
                        {
                            rRow.IsAuto = 1;
                        }
                    }
                    rRow.Name = stTempColumnName;
                    rRow.Type = propertyInfo.GetType().Name;
                    rRow.Value = propertyInfo.GetValue(T, null);
                    _Records.Add(rRow);
                }
            }

            return string.Empty;
        }
        /// <summary>
        /// 摘要：更新一条记录
        /// </summary>
        /// <param name="T">实体类对象</param>
        public virtual void Update(object T)
        {
            _TableName = ((TableAttribute)T.GetType().GetCustomAttributes(true)[0]).TableName;
            PropertyInfo[] propertyInfos = T.GetType().GetProperties();
            string stTempColumnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                Record rRow = new Record();
                if (propertyInfo.GetCustomAttributes(true).Length != 0)
                {
                    if (propertyInfo.GetCustomAttributes(true)[0].GetType().Name != "PrimaryKeyAttribute")
                    {
                        if (!((PropertyAttribute)(propertyInfo.GetCustomAttributes(true)[0])).IsStorage) continue;
                        if (((PropertyAttribute)(propertyInfo.GetCustomAttributes(true)[0])).ColumnName == string.Empty)
                        {
                            stTempColumnName = propertyInfo.Name;
                        }
                        else
                        {
                            stTempColumnName = ((PropertyAttribute)(propertyInfo.GetCustomAttributes(true)[0])).ColumnName;
                        }
                    }
                    else
                    {
                        rRow.IsPK = 1;
                        if (!((PrimaryKeyAttribute)propertyInfo.GetCustomAttributes(true)[0]).IsAuto)
                        {
                            stTempColumnName = ((PrimaryKeyAttribute)(propertyInfo.GetCustomAttributes(true)[0])).ColumnName;
                        }
                        else
                        {
                            rRow.IsAuto = 1;
                        }
                    }
                    rRow.Name = stTempColumnName;
                    rRow.Type = propertyInfo.GetType().Name;
                    rRow.Value = propertyInfo.GetValue(T, null);
                    _Records.Add(rRow);
                }
            }
        }
        /// <summary>
        /// 摘要：删除记录
        /// </summary>
        /// <param name="PK">主键</param>
        /// <param name="tableName">表名</param>
        /// <param name="value">要删除的主键值数组</param>
        public virtual void Delete(string PK, string tableName, List<string> value)
        {
            _TableName = tableName;
        }
        /// <summary>
        /// 摘要：对数据进行分页
        /// </summary>
        /// <param name="dataSetSql">查询语句（包括列名、表等信息，支持表关联）</param>
        /// <param name="where">条件组，二维数据【0】为包括字段名的语句段模板，【1】为取值</param>
        /// <param name="orderByColumn">排序列，二维数据【0】为字段名，【1】为ASC（升序）或DESC（降序）</param>
        /// <param name="rowBegin">排序后数据集的行开始索引</param>
        /// <param name="rowEnd">排序后数据集的行结束索引</param>
        /// <returns></returns>
        public virtual DataTable GetDataSet(string dataSetSql, List<string[]> where, List<string[]> orderByColumn, int rowBegin, int rowEnd)
        {
            return new DataTable();
        }
        /// <summary>
        /// 摘要：根据条件查询数据集
        /// </summary>
        /// <param name="dataSetSql">查询语句（包括列名、表等信息，支持表关联）</param>
        /// <param name="where">条件组，二维数据【0】为包括字段名的语句段模板，【1】为取值</param>
        /// <param name="orderByColumn">排序列，二维数据【0】为字段名，【1】为ASC（升序）或DESC（降序）</param>
        /// <returns></returns>
        public virtual DataTable GetDataSet(string dataSetSql, List<string[]> where, List<string[]> orderByColumn)
        {
            return new DataTable();
        }
        /// <summary>
        /// 摘要：根据条件查询数据集
        /// </summary>
        /// <param name="dataSetSql">查询语句（包括列名、表等信息，支持表关联）</param>
        /// <param name="where">条件组，二维数据【0】为包括字段名的语句段模板，【1】为取值</param>
        /// <returns></returns>
        public virtual DataTable GetDataSet(string dataSetSql, List<string[]> where)
        {
            return new DataTable();
        }
        /// <summary>
        /// 摘要：根据条件查询数据集记录条数
        /// </summary>
        /// <param name="dataSetSql">查询语句（包括列名、表等信息，支持表关联）</param>
        /// <param name="where">条件组，二维数据【0】为包括字段名的语句段模板，【1】为取值</param>
        /// <param name="orderByColumn">排序列，二维数据【0】为字段名，【1】为A（升序）或D（降序）</param>
        /// <returns></returns>
        public virtual int GetDataSetCount(string dataSetSql, List<string[]> where)
        {
            return 0;
        }
        /// <summary>
        /// 摘要：数据库读取
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public virtual DbDataReader GetReader(string sql)
        {
            DbDataReader datareader = null;
            OleDbConnection oConn = new OleDbConnection(_ConnectionString);
            oConn.Open();
            OleDbCommand dbCommand = new OleDbCommand(sql, oConn);
            datareader = dbCommand.ExecuteReader();
            return datareader;
        }
        /// <summary>
        /// 摘要：数据库预览表数据
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public virtual DataTable GetPreviewData(string tableName)
        {
            DataTable dt = new DataTable();
            OleDbConnection oConn = new OleDbConnection(_ConnectionString);
            OleDbDataAdapter oddaAdapter = new OleDbDataAdapter(string.Format(_FrontDataRow, tableName, _RowCount.ToString()), oConn);
            oddaAdapter.Fill(dt);
            return dt;
        }
        /// <summary>
        /// 摘要：导出xsd表结构文件
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="targetPath">导出的目标字段</param>
        public virtual void GetSchema(string tableName, string targetPath)
        {
            DataSet dsData = new DataSet();
            DataTable dttemp = GetPreviewData(tableName);
            dttemp.TableName = tableName;
            dsData.Tables.Add(dttemp);
            if (targetPath.Substring(targetPath.Length - 1, 1) == "\\")
            {
                dsData.WriteXmlSchema(targetPath + tableName + ".xsd");
            }
            else
            {
                dsData.WriteXmlSchema(targetPath + "\\" + tableName + ".xsd");
            }
        }
        /// <summary>
        /// 摘要：获取数据库表名（包括视图）
        /// </summary>
        public virtual DataTable GetTableNames()
        {
            DataTable schemaTable = new DataTable();
            OleDbConnection oConn = new OleDbConnection(_ConnectionString);
            oConn.Open();
            schemaTable = oConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            schemaTable.Merge(oConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "VIEW" }));
            oConn.Close();
            return schemaTable;
        }
        /// <summary>
        /// 摘要：获取数据库表包含的列名
        /// </summary>
        public virtual DataTable GetColumnNames(string tableName)
        {
            return new DataTable();
        }

        /// <summary>
        /// 摘要：获取数据库表包含的所有列名串
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public virtual String GetColumns(string tableName, string isReturnTableName)
        {
            return string.Empty;
        }

        /// <summary>
        /// 摘要：获取数据操作符
        /// </summary>
        /// <returns></returns>
        public virtual DataTable GetDataOperator(string dataType)
        {
            return new DataTable();
        }

        /// <summary>
        /// 摘要：获取标准类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public virtual string GetStandardDataType(string dataType)
        {
            return string.Empty;
        }

        /// <summary>
        /// 摘要：条件拼写
        /// </summary>
        /// <returns></returns>
        public virtual string ConditionSpelling(string tableName, string columnName, string condition, string value, string dataType)
        {
            return string.Empty;
        }
        /// <summary>
        /// 去掉Provider节
        /// </summary>
        public void RemoveProvider()
        {
            string[] rems = _ConnectionString.Split(';');
            foreach (string rem in rems)
            {
                if (rem.Contains("Provider"))
                {
                    _ConnectionString = _ConnectionString.Remove(_ConnectionString.IndexOf(rem), rem.Length + 1);
                }
            }
        }
        /// <summary>
        /// 摘要：拼接读取数据sql语句
        /// </summary>
        /// <param name="columns">查询列</param>
        /// <param name="tableName">表名</param>
        /// <param name="relations">涉及到的表关联</param>
        /// <param name="thisConditions">本表条件</param>
        /// <param name="otherConditions">涉及到的其他表条件</param>
        /// <returns>SQL语句</returns>
        public virtual string GetConstructSelectSql(string columns, string tableName, List<string> relations, List<string> thisConditions, List<string> otherConditions)
        {
            return string.Empty;
        }

        /// <summary>
        /// 摘要：数据库连接测试
        /// </summary>
        public virtual bool ConnectTest()
        {
            return false;
        }

        /// <summary>
        /// 摘要：数据库初始化
        /// </summary>
        /// <param name="TaskName">任务名</param>
        /// <param name="Table">表名</param>
        /// <param name="Columns">列</param>
        /// <param name="TableKey">主健</param>
        public virtual void InitDataBase(string TaskName, string Table, string Columns, string TableKey, string TaskType)
        {

        }

        /// <summary>
        /// 摘要：任务删除时，删除其下的所有表、视图、存储过程
        /// </summary>
        /// <param name="TaskName"></param>
        public virtual void ClearDataBase(string TaskName, string Table)
        {

        }

        public virtual int ExecProcedure(string procedureName, params object[] parameterValues)
        {
            return 0;
        }
        public virtual int ExecProcedure(string procedureName, string[] parameterNames, params object[] parameterValues)
        {
            return 0;
        }
        #endregion
    }
}
