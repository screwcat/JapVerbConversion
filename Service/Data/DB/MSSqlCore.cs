using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Service.Common.DB.Core;
using Service.Common.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace Service.Common.DB
{
    /// <summary>
    /// MSSql数据库操作相关类
    /// </summary>
    public class MSSqlCore : DBCoreBase
    {
        #region 构造函数
        /// <summary>
        /// 摘要：SQL SERVER 构造函数
        /// </summary>
        public MSSqlCore()
        {
            this._Transfer_B = "[";
            this._Transfer_E = "]";
            this._VariableC = "@";
            this._FrontDataRow = "select Top {1} * from [{0}]";
            this._SelectUserTables = "select name as Table_Name  from sysobjects where xtype='U' or xtype='V' order by name";
            this._SelectUserColumns = "select syscolumns.name as Column_Name,systypes.name as type from syscolumns, systypes where syscolumns.xusertype = systypes.xusertype AND syscolumns.id=OBJECT_ID('{0}')";
        }

        #endregion

        #region 方法
        /// <summary>
        /// 摘要：插入一条记录
        /// </summary>
        /// <param name="T">实体类对象</param>
        /// <returns>自增长主键时返回主键值</returns>
        public override string Insert(object T)
        {
            base.Insert(T);
            StringBuilder sbColumnName = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            List<string> lisParameter = new List<string>();
            List<object> lisParameterValues = new List<object>();
            foreach (Record drTemp in base._Records)
            {
                if (drTemp.IsAuto == 0)
                {
                    sbColumnName.Append(_Transfer_B);
                    sbColumnName.Append(drTemp.Name);
                    sbColumnName.Append(_Transfer_E);
                    sbColumnName.Append(",");
                    sbValues.Append(_VariableC + drTemp.Name);
                    sbValues.Append(",");
                    lisParameter.Add(_VariableC + drTemp.Name);
                    lisParameterValues.Add(drTemp.Value);
                }
                else
                {
                    //自增长列的处理，如何返回自增长的值。
                }
            }
            Insert insertSql = new Insert(base._TableName, sbColumnName.Remove(sbColumnName.Length - 1, 1), sbValues.Remove(sbValues.Length - 1, 1));
            int iResult = ExecuteSql.ExeParaSqlForNonQuery(insertSql.ConnectInsertString(), lisParameter, lisParameterValues);
            return string.Empty;
        }
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="T">实体类对象</param>
        public override void Update(object T)
        {
            base.Update(T);
            StringBuilder sbColumnName = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            StringBuilder sbUpdateWhere = new StringBuilder();//Where 条件
            List<string> lisParameter = new List<string>();
            List<object> lisParameterValues = new List<object>();
            bool HasWhere = false;//是否有主键
            foreach (Record drTemp in base._Records)
            {
                if (drTemp.IsPK == 0)
                {
                    if (!drTemp.Value.Equals(string.Empty))
                    {
                        if (sbColumnName.Length > 0)
                            sbColumnName.Append(",");
                        sbColumnName.Append(_Transfer_B);
                        sbColumnName.Append(drTemp.Name);
                        sbColumnName.Append(_Transfer_E);
                        sbColumnName.Append("=");
                        sbColumnName.Append(_VariableC + drTemp.Name);


                        lisParameter.Add(_VariableC + drTemp.Name);
                        lisParameterValues.Add(drTemp.Value);
                    }
                }
                else
                {
                    //Where条件 主键的值(必须有主键)。
                    HasWhere = true;
                    sbUpdateWhere.Append(_Transfer_B);
                    sbUpdateWhere.Append(drTemp.Name);
                    sbUpdateWhere.Append(_Transfer_E);
                    sbUpdateWhere.Append("=");
                    sbUpdateWhere.Append(_VariableC + drTemp.Name);

                    lisParameter.Add(_VariableC + drTemp.Name);
                    lisParameterValues.Add(drTemp.Value);
                }
            }

            Update UpdateSql = new Update(HasWhere, base._TableName, sbColumnName, sbUpdateWhere);
            int iResult = ExecuteSql.ExeParaSqlForNonQuery(UpdateSql.ConnectUpdateString(), lisParameter, lisParameterValues);
        }
        /// <summary>
        /// 根据主键值数组删除记录
        /// </summary>
        /// <param name="T">实体类对象</param>
        /// <param name="value">要删除的主键值数组</param>
        public override void Delete(string PK, string tableName, List<string> value)
        {
            base.Delete(PK, tableName, value);
            StringBuilder sbColumnName = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            StringBuilder sbUpdateWhere = new StringBuilder();//Where 条件
            List<string> lisParameter = new List<string>();
            List<object> lisParameterValues = new List<object>();

            //Where条件 主键的值(必须有主键)。
            sbUpdateWhere.Append(_Transfer_B);
            sbUpdateWhere.Append(PK);
            sbUpdateWhere.Append(_Transfer_E);
            sbUpdateWhere.Append("IN");
            sbUpdateWhere.Append("(");
            for (int i = 0; i < value.Count; i++)
            {
                if (i != 0)
                    sbUpdateWhere.Append(",");
                sbUpdateWhere.Append(_VariableC + PK + "_" + i.ToString());
                lisParameter.Add(_VariableC + PK + "_" + i.ToString());
                lisParameterValues.Add(value[i]);
            }
            sbUpdateWhere.Append(")");
            Delete DeleteSql = new Delete(true, base._FrontTableName, base._TableName, sbUpdateWhere);
            int iResult = ExecuteSql.ExeParaSqlForNonQuery(DeleteSql.ConnectDeleteString(), lisParameter, lisParameterValues);
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
        public override DataTable GetDataSet(string dataSetSql, List<string[]> where, List<string[]> orderByColumn, int rowBegin, int rowEnd)
        {
            string stCmd = "SELECT * FROM (SELECT ROW_NUMBER() Over(order by {0}) as MB,y.*  FROM  (" + dataSetSql + " {1}) y )  t WHERE MB>{2} and MB<={3}";
            string stOrderby = ConstructOrderBy(orderByColumn);
            List<string> para = new List<string>();
            List<object> value = new List<object>();
            string stWhere = ConstructWhere(where, out para, out value);
            if (where.Count == 0)
                stCmd = string.Format(stCmd, stOrderby, "", rowBegin, rowEnd);
            else
                stCmd = string.Format(stCmd, stOrderby, " where " + stWhere, rowBegin, rowEnd);

            return ExecuteSql.ExeParaSqlForDataSet(stCmd, para, value).Tables[0];
        }
        /// <summary>
        /// 摘要：根据条件查询数据集
        /// </summary>
        /// <param name="dataSetSql">查询语句（包括列名、表等信息，支持表关联）</param>
        /// <param name="where">条件组，二维数据【0】为包括字段名的语句段模板，【1】为取值</param>
        /// <param name="orderByColumn">排序列，二维数据【0】为字段名，【1】为ASC（升序）或DESC（降序）</param>
        /// <returns></returns>
        public override DataTable GetDataSet(string dataSetSql, List<string[]> where, List<string[]> orderByColumn)
        {
            string stCmd = dataSetSql + "{1} {0}";
            string stOrderby = ConstructOrderBy(orderByColumn);
            List<string> para = new List<string>();
            List<object> value = new List<object>();
            string stWhere = ConstructWhere(where, out para, out value);
            if (where.Count == 0)
                stCmd = string.Format(stCmd, string.IsNullOrEmpty(stOrderby) ? "" : "Order by " + stOrderby, "");
            else
                stCmd = string.Format(stCmd, string.IsNullOrEmpty(stOrderby) ? "" : "Order by " + stOrderby, " where " + stWhere);

            return ExecuteSql.ExeParaSqlForDataSet(stCmd, para, value).Tables[0];
        }
        /// <summary>
        /// 摘要：根据条件查询数据集
        /// </summary>
        /// <param name="dataSetSql">查询语句（包括列名、表等信息，支持表关联）</param>
        /// <param name="where">条件组，二维数据【0】为包括字段名的语句段模板，【1】为取值</param>
        /// <returns></returns>
        public override DataTable GetDataSet(string dataSetSql, List<string[]> where)
        {
            return GetDataSet(dataSetSql, where, new List<string[]>());
        }
        /// <summary>
        /// 摘要：根据条件查询数据集记录条数
        /// </summary>
        /// <param name="dataSetSql">查询语句（包括列名、表等信息，支持表关联）</param>
        /// <param name="where">条件组，二维数据【0】为包括字段名的语句段模板，【1】为取值</param>
        /// <returns></returns>
        public override int GetDataSetCount(string dataSetSql, List<string[]> where)
        {
            string stCmd = dataSetSql + "{0}";
            List<string> para = new List<string>();
            List<object> value = new List<object>();
            string stWhere = ConstructWhere(where, out para, out value);
            if (where.Count == 0)
                stCmd = string.Format(stCmd, "");
            else
                stCmd = string.Format(stCmd, " where " + stWhere);

            return ExecuteSql.ExeParaSqlForScalar(stCmd, para, value);
        }
        /// <summary>
        /// 摘要：数据库读取
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public override DbDataReader GetReader(string sql)
        {
            DbDataReader datareader = null;
            if (_DBprovider.ToUpper().Contains("OLEDB"))
            {
                datareader = base.GetReader(sql);
            }
            else
            {
                RemoveProvider();
                SqlConnection oConn = new SqlConnection(_ConnectionString);
                oConn.Open();
                SqlCommand dbCommand = new SqlCommand(sql, oConn);
                datareader = dbCommand.ExecuteReader();
            }
            return datareader;
        }
        /// <summary>
        /// 摘要：数据库预览表数据
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public override DataTable GetPreviewData(string tableName)
        {
            DataTable dt = new DataTable();
            if (_DBprovider.ToUpper().Contains("OLEDB"))
            {
                dt = base.GetPreviewData(tableName);
            }
            else
            {
                RemoveProvider();
                SqlConnection oConn = new SqlConnection(_ConnectionString);
                SqlDataAdapter oddaAdapter = new SqlDataAdapter(string.Format(_FrontDataRow, tableName, _RowCount.ToString()), oConn);
                oddaAdapter.Fill(dt);
            }
            return dt;
        }
        /// <summary>
        /// 摘要：导出xsd表结构文件
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="targetPath">导出的目标字段</param>
        public override void GetSchema(string tableName, string targetPath)
        {
            base.GetSchema(tableName, targetPath);
        }
        /// <summary>
        /// 摘要：获取数据库表名（包括视图）
        /// </summary>
        public override DataTable GetTableNames()
        {
            DataTable schemaTable = new DataTable();
            if (_DBprovider.ToUpper().Contains("OLEDB"))
            {
                OleDbConnection oConn = new OleDbConnection(_ConnectionString);
                oConn.Open();
                schemaTable = oConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                schemaTable.Merge(oConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "VIEW" }));
                oConn.Close();
            }
            else
            {
                //去掉Provider节
                RemoveProvider();
                SqlConnection oConn = new SqlConnection(_ConnectionString);
                SqlDataAdapter oddaAdapter = new SqlDataAdapter(_SelectUserTables, oConn);
                oddaAdapter.Fill(schemaTable);
            }
            return schemaTable;
        }
        /// <summary>
        /// 摘要：获取数据库表包含的列名
        /// </summary>
        public override DataTable GetColumnNames(string tableName)
        {
            DataTable dtCloumnNames = new DataTable();
            if (_DBprovider.ToUpper().Contains("OLEDB"))
            {
                OleDbConnection oConn = new OleDbConnection(_ConnectionString);
                oConn.Open();
                //dtCloumnNames = oConn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new Object[] { null, null, tableName, null });
                OleDbDataAdapter oldDbDataAdapter = new OleDbDataAdapter(string.Format(_SelectUserColumns, tableName), oConn);
                oldDbDataAdapter.Fill(dtCloumnNames);
                oConn.Close();
            }
            else
            {
                RemoveProvider();
                SqlConnection oConn = new SqlConnection(_ConnectionString);
                SqlDataAdapter oddaAdapter = new SqlDataAdapter(string.Format(_SelectUserColumns, tableName), oConn);
                oddaAdapter.Fill(dtCloumnNames);
            }
            return dtCloumnNames;
        }

        /// <summary>
        /// 摘要：获取数据库表包含的所有列名串
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override string GetColumns(string tableName, string isReturnTableName)
        {
            string stColumnNames = string.Empty;
            DataTable dtColumn = GetColumnNames(tableName);
            for (int i = 0; i < dtColumn.Rows.Count; i++)
            {
                if (isReturnTableName == "1")
                {
                    stColumnNames += tableName + "." + dtColumn.Rows[i]["Column_Name"].ToString() + ",";
                }
                else
                {
                    stColumnNames += dtColumn.Rows[i]["Column_Name"].ToString() + ",";
                }
            }
            if (stColumnNames.Length > 1)
            {
                stColumnNames = stColumnNames.Substring(0, stColumnNames.Length - 1);
            }
            return stColumnNames;
        }
        /// <summary>
        /// 获取操作符集合
        /// </summary>
        /// <param name="stDataType"></param>
        /// <returns></returns>
        public override DataTable GetDataOperator(string stStandardDataType)
        {
            DataTable dtDataOperator = new DataTable();
            dtDataOperator.Columns.Add(new DataColumn("OperatorName"));
            dtDataOperator.Columns.Add(new DataColumn("OperatorCode"));
            switch (stStandardDataType)
            {
                case "varchar":
                    {
                        dtDataOperator.Rows.Add(new string[] { "等于", "=" });
                        dtDataOperator.Rows.Add(new string[] { "不等于", "<>" });
                        dtDataOperator.Rows.Add(new string[] { "相似", "like" });
                        dtDataOperator.Rows.Add(new string[] { "不相似", "not like" });
                        dtDataOperator.Rows.Add(new string[] { "包括", "in" });
                        dtDataOperator.Rows.Add(new string[] { "不包括", "not in" });
                        dtDataOperator.Rows.Add(new string[] { "空", "is null" });
                        dtDataOperator.Rows.Add(new string[] { "非空", "is not null" });
                    }
                    break;
                case "int":
                    {
                        dtDataOperator.Rows.Add(new string[] { "等于", "=" });
                        dtDataOperator.Rows.Add(new string[] { "不等于", "<>" });
                        dtDataOperator.Rows.Add(new string[] { "大于", ">" });
                        dtDataOperator.Rows.Add(new string[] { "大于等于", ">=" });
                        dtDataOperator.Rows.Add(new string[] { "小于", "<" });
                        dtDataOperator.Rows.Add(new string[] { "小于等于", "<=" });
                        dtDataOperator.Rows.Add(new string[] { "空", "is null" });
                        dtDataOperator.Rows.Add(new string[] { "非空", "is not null" });
                    }
                    break;
                case "numeric":
                    {
                        dtDataOperator.Rows.Add(new string[] { "等于", "=" });
                        dtDataOperator.Rows.Add(new string[] { "不等于", "<>" });
                        dtDataOperator.Rows.Add(new string[] { "大于", ">" });
                        dtDataOperator.Rows.Add(new string[] { "大于等于", ">=" });
                        dtDataOperator.Rows.Add(new string[] { "小于", "<" });
                        dtDataOperator.Rows.Add(new string[] { "小于等于", "<=" });
                        dtDataOperator.Rows.Add(new string[] { "空", "is null" });
                        dtDataOperator.Rows.Add(new string[] { "非空", "is not null" });
                    }
                    break;
                case "datetime":
                    {
                        dtDataOperator.Rows.Add(new string[] { "等于", "=" });
                        dtDataOperator.Rows.Add(new string[] { "不等于", "<>" });
                        dtDataOperator.Rows.Add(new string[] { "大于", ">" });
                        dtDataOperator.Rows.Add(new string[] { "大于等于", ">=" });
                        dtDataOperator.Rows.Add(new string[] { "小于", "<" });
                        dtDataOperator.Rows.Add(new string[] { "小于等于", "<=" });
                        dtDataOperator.Rows.Add(new string[] { "空", "is null" });
                        dtDataOperator.Rows.Add(new string[] { "非空", "is not null" });
                    }
                    break;
                case "binary":
                    break;
                case "other":
                    break;
            }
            return dtDataOperator;
        }

        /// <summary>
        /// 获取数据类型的标准类型
        /// </summary>
        /// <param name="stDataType"></param>
        /// <returns></returns>
        public override string GetStandardDataType(string stDataType)
        {
            string stStandardDataType = string.Empty;
            switch (stDataType)
            {
                /*字符型*/
                case "char":
                    stStandardDataType = "varchar";
                    break;
                case "varchar":
                    stStandardDataType = "varchar";
                    break;
                case "text":
                    stStandardDataType = "varchar";
                    break;
                case "nchar":
                    stStandardDataType = "varchar";
                    break;
                case "nvarchar":
                    stStandardDataType = "varchar";
                    break;
                case "ntext":
                    stStandardDataType = "varchar";
                    break;
                /*整型*/
                case "int":
                    stStandardDataType = "int";
                    break;
                case "bigint":
                    stStandardDataType = "int";
                    break;
                case "smallint":
                    stStandardDataType = "int";
                    break;
                case "tinyint":
                    stStandardDataType = "int";
                    break;

                /*小数型*/
                case "decimal":
                    stStandardDataType = "numeric";
                    break;
                case "numeric":
                    stStandardDataType = "numeric";
                    break;
                case "float":
                    stStandardDataType = "numeric";
                    break;
                case "real":
                    stStandardDataType = "numeric";
                    break;
                case "smallmoney":
                    stStandardDataType = "numeric";
                    break;
                case "money":
                    stStandardDataType = "numeric";
                    break;

                /*日期型*/
                case "datetime":
                    stStandardDataType = "datetime";
                    break;
                case "smalldatetime":
                    stStandardDataType = "datetime";
                    break;

                /*二进制*/
                case "binary":
                    stStandardDataType = "binary";
                    break;
                case "varbinary":
                    stStandardDataType = "binary";
                    break;
                case "image":
                    stStandardDataType = "binary";
                    break;

                /*其他*/
                case "bit":
                    stStandardDataType = "other";
                    break;
                case "timestamp":
                    stStandardDataType = "other";
                    break;
                case "uniqueidentifier":
                    stStandardDataType = "other";
                    break;
                case "xml":
                    stStandardDataType = "other";
                    break;
                case "sql_variant":
                    stStandardDataType = "other";
                    break;

                default:
                    stStandardDataType = "other";
                    break;
            }
            return stStandardDataType;
        }

        /// <summary>
        /// 条件拼写
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="condition"></param>
        /// <param name="value"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public override string ConditionSpelling(string tableName, string columnName, string condition, string value, string dataType)
        {

            string stTempString = string.Empty;
            switch (dataType)
            {
                case "varchar":
                    if (condition.Equals("like") || condition.Equals("not like"))
                    {
                        stTempString = tableName + "." + columnName + " " + condition + " " + "'%" + value + "%'";
                    }
                    else
                    {
                        if (condition.Equals("in") || condition.Equals("not in"))
                        {
                            string stTempValue = string.Empty;
                            foreach (string stTemp in value.Split(','))
                            {
                                stTempValue += "'" + stTemp + "'" + ",";
                            }
                            if (stTempValue.Length > 0)
                            {
                                stTempValue = stTempValue.Substring(0, stTempValue.Length - 1);
                            }
                            stTempString = tableName + "." + columnName + " " + condition + " " + "(" + stTempValue + ")";
                        }
                        else
                        {
                            stTempString = tableName + "." + columnName + " " + condition + " " + "'" + value + "'";
                        }
                    }
                    break;
                case "int":
                    stTempString = tableName + "." + columnName + " " + condition + " " + value;
                    break;
                case "numeric":
                    stTempString = tableName + "." + columnName + " " + condition + " " + value;
                    break;
                case "datetime":
                    stTempString = tableName + "." + columnName + " " + condition + " " + "'" + Convert.ToDateTime(value).ToString("yyyy-MM-dd") + "'";
                    break;
            }
            return stTempString;
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
        public override string GetConstructSelectSql(string columns, string tableName, List<string> relations, List<string> thisConditions, List<string> otherConditions)
        {
            string stSQL = " SELECT [" + string.Join("].[", string.Join("],[", columns.Split(',')).Split('.')) + "] FROM [" + tableName + "] ";
            StringBuilder sbthisConditions = new StringBuilder();
            StringBuilder sbotherConditions = new StringBuilder();
            foreach (string stTemp in thisConditions)
            {
                sbthisConditions.Append("[" + stTemp.Substring(0, stTemp.IndexOf('.')) + "].[" + stTemp.Substring(stTemp.IndexOf('.') + 1, stTemp.IndexOf(' ') - stTemp.IndexOf('.') - 1) + "] " + stTemp.Substring(stTemp.IndexOf(' ') + 1));
                sbthisConditions.Append(" and ");
            }
            foreach (string stTemp in relations)
            {
                string stOtherTable = stTemp.Split('=')[0].Split('.')[0] == tableName ? stTemp.Split('=')[1].Split('.')[0] : stTemp.Split('=')[0].Split('.')[0];
                var result = from s in otherConditions
                             where s.Contains(stOtherTable + ".")
                             select s;
                StringBuilder sbOtherCondition = new StringBuilder();
                foreach (string stSubTemp in result)
                {
                    sbOtherCondition.Append("[" + stSubTemp.Substring(0, stSubTemp.IndexOf('.')) + "].[" + stSubTemp.Substring(stSubTemp.IndexOf('.') + 1, stSubTemp.IndexOf(' ') - stSubTemp.IndexOf('.') - 1) + "] " + stSubTemp.Substring(stSubTemp.IndexOf(' ') + 1));
                    sbOtherCondition.Append(" and ");
                }
                string stRelation = string.Format("[{0}].[{1}]=[{2}].[{3}]", stTemp.Split('=')[0].Split('.')[0], stTemp.Split('=')[0].Split('.')[1], stTemp.Split('=')[1].Split('.')[0], stTemp.Split('=')[1].Split('.')[1]);
                sbotherConditions.Append(string.Format(" exists (select null from [{0}] where {1} {2}) ", stOtherTable, stRelation, string.IsNullOrEmpty(sbOtherCondition.ToString()) ? string.Empty : " and " + sbOtherCondition.ToString().Substring(0, sbOtherCondition.ToString().Length - 4)));
                sbotherConditions.Append(" and ");
            }
            if (!string.IsNullOrEmpty(sbthisConditions.ToString()) || !string.IsNullOrEmpty(sbotherConditions.ToString()))
            {
                stSQL += " WHERE ";
                if (!string.IsNullOrEmpty(sbthisConditions.ToString())) stSQL += sbthisConditions.ToString();
                if (!string.IsNullOrEmpty(sbotherConditions.ToString())) stSQL += sbotherConditions.ToString();
                stSQL = stSQL.Substring(0, stSQL.Length - 4);
            }
            return stSQL;
        }

        /// <summary>
        /// 摘要：数据库连接测试
        /// </summary>
        public override bool ConnectTest()
        {
            bool bConnectTestResult = false;
            if (_DBprovider.ToUpper().Contains("OLEDB"))
            {
                OleDbConnection oConn = new OleDbConnection(_ConnectionString);
                try
                {
                    oConn.Open();
                    bConnectTestResult = true;
                }
                catch
                {

                }
                if (bConnectTestResult == true)
                {
                    oConn.Close();
                }
            }
            else
            {
                RemoveProvider();
                SqlConnection oConn = new SqlConnection(_ConnectionString);
                try
                {
                    oConn.Open();
                    bConnectTestResult = true;
                }
                catch
                {

                }
                if (bConnectTestResult == true)
                {
                    oConn.Close();
                }
            }
            return bConnectTestResult;
        }
        /// <summary>
        /// 摘要：构造where条件
        /// </summary>
        /// <param name="whereParam">条件集合</param>
        /// <returns>条件语句</returns>
        private string ConstructWhere(List<string[]> whereParam, out List<string> para, out List<object> value)
        {
            para = new List<string>();
            value = new List<object>();
            string stWhere = string.Empty;
            List<string> Sql = new List<string>();
            for (int i = 0; i < whereParam.Count; i++)
            {
                if (!string.IsNullOrEmpty(whereParam[i][1].Trim()))
                {
                    string Kye = "@" + whereParam[i].GetHashCode();
                    Sql.Add(string.Format(whereParam[i][0].Replace("'%{", "'%'+{").Replace("}%'", "}+'%'").Replace("'{", "{").Replace("}'", "}"), Kye));
                    para.Add(Kye);
                    value.Add(whereParam[i][1]);
                }
            }
            stWhere = string.Join(" and ", Sql.ToArray());
            return stWhere;
        }
        /// <summary>
        /// 摘要：构造order by 排序
        /// </summary>
        /// <param name="orderByParam">排序字段集合</param>
        /// <returns>排序语句</returns>
        private string ConstructOrderBy(List<string[]> orderByParam)
        {
            string stOrderBy = string.Empty;
            List<string> Sql = new List<string>();
            for (int i = 0; i < orderByParam.Count; i++)
            {
                Sql.Add(orderByParam[i][0] + " " + orderByParam[i][1]);
            }
            stOrderBy = string.Join(",", Sql.ToArray());
            return stOrderBy;
        }

        /// <summary>
        /// 摘要：数据库初始化
        /// </summary>
        /// <param name="TaskName">任务名</param>
        /// <param name="Table">表名</param>
        /// <param name="Columns">列</param>
        /// <param name="TableKey">主键</param>
        public override void InitDataBase(string TaskName, string Table, string Columns, string TableKey, string TaskType)
        {
            for (int i = 0; i < Table.Split(',').Length; i++)
            {
                //创建全量表
                CreateTable(TaskName, "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All", Columns.Split(';')[i], TableKey.Split(';')[i]);
                //创建中间表
                CreateTable(TaskName, "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp", Columns.Split(';')[i], TableKey.Split(';')[i]);
                //创建增量表
                CreateTable(TaskName, "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Export", Columns.Split(';')[i], TableKey.Split(';')[i]);
                //创建全量表视图
                CreateView("T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All", "V_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All", Columns.Split(';')[i]);
                //创建中间表视图
                CreateView("T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp", "V_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp", Columns.Split(';')[i]);
            }
            //创建存储过程
            CreateProcedure(TaskName, Table, Columns, TableKey, TaskType);
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="Table"></param>
        /// <param name="Columns"></param>
        /// <param name="TableKey"></param>
        private void CreateTable(string TaskName, string Table, string Columns, string TableKey)
        {
            int iKey = 0;
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(Environment.NewLine);
            sbSql.Append(string.Format("if object_id('{0}') is not null", Table));
            sbSql.Append(Environment.NewLine);
            sbSql.Append(string.Format("drop table [{0}]", Table));
            sbSql.Append(Environment.NewLine);
            sbSql.Append(string.Format("create table [{0}] (", Table));                //表名
            sbSql.Append(Environment.NewLine);
            for (int i = 0; i < Columns.Split(',').Length; i++)                        //循环列名
            {
                iKey = 0;
                for (int j = 0; j < TableKey.Split(',').Length; j++)
                {
                    if (Columns.Split(',')[i].Trim() == TableKey.Split(',')[j].Trim()) //判断列名是否为主键
                    {
                        sbSql.Append(string.Format("[{0}] varchar (200),", Columns.Split(',')[i].Trim()));
                        sbSql.Append(Environment.NewLine);
                        iKey = 1;
                        break;
                    }
                }
                if (iKey == 0)
                {
                    sbSql.Append(string.Format("[{0}] varchar (Max),", Columns.Split(',')[i].Trim()));
                    sbSql.Append(Environment.NewLine);
                }
            }
            if (Table.Substring(Table.Length - 6, 6).Trim() == "Export")           //增量表多增加一个字段ColumnFlag              
            {
                sbSql.Append(string.Format("[{0}] varchar (1),", "ColumnFlag"));
                sbSql.Append(Environment.NewLine);
            }
            sbSql.Append(Environment.NewLine);
            sbSql.Append(string.Format("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED (", Table));     //创建主键
            sbSql.Append(Environment.NewLine);
            string stKey = string.Empty;
            foreach (string stTableKey in TableKey.Split(','))
            {
                stKey += stTableKey + " ASC,";
            }
            sbSql.Append(Environment.NewLine);
            sbSql.Append(stKey.Substring(0, stKey.Length - 1));
            sbSql.Append(") WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]");
            sbSql.Append(");");
            sbSql.Append(Environment.NewLine);
            sbSql.Append(string.Format("CREATE UNIQUE NONCLUSTERED INDEX [IX_{0}] ON [dbo].[{0}] (", Table));     //创建索引
            sbSql.Append(Environment.NewLine);
            sbSql.Append(stKey.Substring(0, stKey.Length - 1));
            sbSql.Append(Environment.NewLine);
            sbSql.Append(") WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]");
            ExecuteSql.ExeComSqlForNonQuery(sbSql.ToString());
        }

        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="Table"></param>
        /// <param name="TableKey"></param>
        private void CreateView(string TableName, string ViewName)
        {
            StringBuilder sbViewSql = new StringBuilder();
            sbViewSql.Append(Environment.NewLine);
            sbViewSql.Append(string.Format("if object_id('{0}') is not null", ViewName));
            sbViewSql.Append(Environment.NewLine);
            sbViewSql.Append(string.Format("drop view [{0}]", ViewName));
            ExecuteSql.ExeComSqlForNonQuery(sbViewSql.ToString());
            sbViewSql.Clear();


            sbViewSql.Append(Environment.NewLine);
            sbViewSql.Append(string.Format("CREATE VIEW [{0}", ViewName));
            sbViewSql.Append("] AS SELECT  *,CHECKSUM(*) AS NewHashValue FROM  [");
            sbViewSql.Append(TableName);
            sbViewSql.Append("];");
            ExecuteSql.ExeComSqlForNonQuery(sbViewSql.ToString());
        }
        private void CreateView(string TableName, string ViewName, string Columns)
        {
            StringBuilder sbViewSql = new StringBuilder();
            sbViewSql.Append(Environment.NewLine);
            sbViewSql.Append(string.Format("if object_id('{0}') is not null", ViewName));
            sbViewSql.Append(Environment.NewLine);
            sbViewSql.Append(string.Format("drop view [{0}]", ViewName));
            ExecuteSql.ExeComSqlForNonQuery(sbViewSql.ToString());
            sbViewSql.Clear();
            sbViewSql.Append(Environment.NewLine);
            sbViewSql.Append(string.Format("CREATE VIEW [{0}", ViewName));
            //sbViewSql.Append("] AS SELECT  *,CHECKSUM(*) AS NewHashValue FROM  [");
            sbViewSql.Append("] AS SELECT  *,HASHBYTES('MD5',");
            for (int i = 0; i < Columns.Split(',').Length; i++)
            {
                if (i == Columns.Split(',').Length - 1)
                    sbViewSql.Append(Columns.Split(',')[i].ToString());
                else
                    sbViewSql.Append(Columns.Split(',')[i].ToString() + "+");
            }
            sbViewSql.Append(") AS NewHashValue FROM  [");
            //
            sbViewSql.Append(TableName);
            sbViewSql.Append("];");
            ExecuteSql.ExeComSqlForNonQuery(sbViewSql.ToString());
        }
        /// <summary>
        /// 摘要：创建存储过程（交换系统）
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="Table"></param>
        /// <param name="Column"></param>
        /// <param name="TableKey"></param>
        private void CreateLoadProcedure(string TaskName, string Table, string Columns, string TableKey)
        {
            StringBuilder sbSpSql = new StringBuilder();//构建存储过程SQL
            StringBuilder sbTotalSpSql = new StringBuilder();//构建整合存储过程SQL
            string stCloumns = string.Empty;//列
            string stKey = string.Empty;
            string stWhereCondition = string.Empty;
            string stUpdateCondition = string.Empty;
            string stColumn = string.Empty;
            //验证是否存在
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(string.Format("if object_id('{0}') is not null", "P_" + TaskName));
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(string.Format("drop Procedure [{0}]", "P_" + TaskName));
            sbTotalSpSql.Append(Environment.NewLine);
            ExecuteSql.ExeComSqlForNonQuery(sbTotalSpSql.ToString());
            sbTotalSpSql.Clear();

            //生成整合存储过程头部
            sbTotalSpSql.Append(string.Format("Create PROCEDURE [{0}]", "P_" + TaskName));
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(" @Cloumn_Batch varchar(5)");//批次号
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(" as Begin ");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(" BEGIN TRANSACTION ");
            sbTotalSpSql.Append(Environment.NewLine);
            for (int i = 0; i < Table.Split(',').Length; i++)
            {
                //列集合 
                stCloumns = Columns.Split(';')[i].ToString();
                //更新条件集合
                stUpdateCondition = string.Empty;
                for (int j = 0; j < Columns.Split(';')[i].Split(',').Length; j++)
                {
                    if (j == 0)
                    {
                        stUpdateCondition += Columns.Split(';')[i].Split(',')[j] + "=t." + Columns.Split(';')[i].Split(',')[j];
                    }
                    else
                    {
                        stUpdateCondition += "," + Columns.Split(';')[i].Split(',')[j] + "=t." + Columns.Split(';')[i].Split(',')[j];

                    }
                }
                //where 条件
                stWhereCondition = string.Empty;
                for (int j = 0; j < TableKey.Split(';')[i].Split(',').Length; j++)
                {
                    if (j == 0)
                        stWhereCondition += string.Format(" {0}.{1}=t.{1} ", Table.Split(',')[i], TableKey.Split(';')[i].Split(',')[j]);
                    else
                        stWhereCondition += string.Format(" and {0}.{1}=t.{1} ", Table.Split(',')[i], TableKey.Split(';')[i].Split(',')[j]);
                }

                //插入新增数据
                sbTotalSpSql.Append(string.Format("insert into {0} ({1}) select {1} from {2} where Cloumn_Batch=@Cloumn_Batch and ColumnFlag='1' ", Table.Split(',')[i], stCloumns, "T_" + TaskName + "_" + Table.Split(',')[i]));
                sbTotalSpSql.Append(Environment.NewLine);

                //删除数据
                sbTotalSpSql.Append(string.Format("delete {0} from {1} t where {2} and Cloumn_Batch=@Cloumn_Batch and ColumnFlag='3'", Table.Split(',')[i], "T_" + TaskName + "_" + Table.Split(',')[i], stWhereCondition));
                sbTotalSpSql.Append(Environment.NewLine);
                //修改数据
                sbTotalSpSql.Append(string.Format("update {0} set {1} from {0},{2} t where {3} and Cloumn_Batch=@Cloumn_Batch and ColumnFlag='2'", Table.Split(',')[i], stUpdateCondition, "T_" + TaskName + "_" + Table.Split(',')[i], stWhereCondition));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(Environment.NewLine);


            }
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("IF @@ERROR<>0");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("BEGIN");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("ROLLBACK TRANSACTION--事务回滚语句");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("return 0");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("End");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("ELSE");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("BEGIN");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("COMMIT TRANSACTION--事务提交语句");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("return 1");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("End");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("End");

            ExecuteSql.ExeComSqlForNonQuery(sbTotalSpSql.ToString());
            sbTotalSpSql.Clear();

        }

        /// <summary>
        /// 摘要：创建存储过程（过滤系统）
        /// </summary>
        /// <param name="TaskName">任务名</param>
        /// <param name="Table">表名</param>
        /// <param name="Type">类型</param>
        private void CreateProcedure(string TaskName, string Table, string Column, string TableKey, string Type)
        {
            StringBuilder sbSpSql = new StringBuilder();//构建存储过程SQL
            StringBuilder sbTotalSpSql = new StringBuilder();//构建整合存储过程SQL
            string stWhereCondition = string.Empty;
            string stWhereCondition_v = string.Empty;
            string stColumn = string.Empty;

            //生成整合存储过程头部
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(string.Format("if object_id('{0}') is not null", "P_" + TaskName));
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(string.Format("drop Procedure [{0}]", "P_" + TaskName));
            sbTotalSpSql.Append(Environment.NewLine);
            ExecuteSql.ExeComSqlForNonQuery(sbTotalSpSql.ToString());
            sbTotalSpSql.Clear();

            sbTotalSpSql.Append(string.Format("Create PROCEDURE [{0}]", "P_" + TaskName));
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(" @TaskType varchar(1)");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(" as Begin ");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(" BEGIN TRANSACTION ");
            sbTotalSpSql.Append(Environment.NewLine);


            for (int i = 0; i < Table.Split(',').Length; i++)
            {
                stWhereCondition = string.Empty;
                stWhereCondition_v = string.Empty;
                stColumn = string.Empty;
                foreach (string stemp in TableKey.Split(';')[i].Split(','))
                {
                    stWhereCondition += stemp + "=v." + stemp + " And ";
                    stWhereCondition_v += "v2." + stemp + "=v1." + stemp + " And ";
                }
                foreach (string stemp in Column.Split(';')[i].Split(','))
                {
                    stColumn += "v2." + stemp + ",";
                }
                stWhereCondition = stWhereCondition.Substring(0, stWhereCondition.Length - 4);
                stColumn = stColumn.Substring(0, stColumn.Length - 1);

                //处理增加语句
                sbTotalSpSql.Append("begin");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("insert into {0} ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Export"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("select v.*,'1' from [{0}] v where ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("not exists ");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("(select * from {0} where ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("{0})", stWhereCondition));
                sbTotalSpSql.Append(Environment.NewLine);
                //处理删除语句
                sbTotalSpSql.Append("if @TaskType='1'");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("begin");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("insert into {0} ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Export"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("select v.*,'3' from [{0}] v where ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("not exists ");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("(select * from {0} where ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("{0})", stWhereCondition));
                sbTotalSpSql.Append(Environment.NewLine);

                //处理修改语句
                sbTotalSpSql.Append(string.Format("insert into {0} ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Export"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("select {0},'2' from [{1}] v1 inner join [{2}] v2", stColumn, "V_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All", "V_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(" on  ");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("{0}", stWhereCondition_v));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(" v2.NewHashValue<>v1.NewHashValue");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("end");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("if @TaskType='2'");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("begin");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("insert into {0} ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Export"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("select v.*,'2' from [{0}] v where ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("exists ");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("(select * from {0} where ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("{0})", stWhereCondition));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("end");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("end");
                sbTotalSpSql.Append(Environment.NewLine);
            }

            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("IF @@ERROR<>0");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("BEGIN");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("ROLLBACK TRANSACTION--事务回滚语句");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("return 0");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("End");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("ELSE");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("BEGIN");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("COMMIT TRANSACTION--事务提交语句");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("return 1");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("End");
            sbTotalSpSql.Append(Environment.NewLine);

            sbTotalSpSql.Append(" END ");
            ExecuteSql.ExeComSqlForNonQuery(sbTotalSpSql.ToString());
            sbTotalSpSql.Clear();

            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(string.Format("if object_id('{0}') is not null", "P_" + TaskName + "_clear"));
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(string.Format("drop Procedure [{0}]", "P_" + TaskName + "_clear"));
            sbTotalSpSql.Append(Environment.NewLine);
            ExecuteSql.ExeComSqlForNonQuery(sbTotalSpSql.ToString());
            sbTotalSpSql.Clear();

            sbTotalSpSql.Append(string.Format("Create PROCEDURE [{0}]", "P_" + TaskName + "_clear"));
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(" @TaskType varchar(1)");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(" as Begin ");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(" BEGIN TRANSACTION ");
            sbTotalSpSql.Append(Environment.NewLine);

            for (int i = 0; i < Table.Split(',').Length; i++)
            {
                stWhereCondition = string.Empty;
                stWhereCondition_v = string.Empty;
                stColumn = string.Empty;
                foreach (string stemp in TableKey.Split(';')[i].Split(','))
                {
                    stWhereCondition_v += "v2." + stemp + "=v1." + stemp + " And ";
                }
                foreach (string stemp in Column.Split(';')[i].Split(','))
                {
                    stColumn += "v2." + stemp + ",";
                }
                stWhereCondition_v = stWhereCondition_v.Substring(0, stWhereCondition_v.Length - 4);
                stColumn = stColumn.Substring(0, stColumn.Length - 1);
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("if @TaskType='1'");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("begin");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("delete from {0} ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("delete from {0} ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Export"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("insert into {0} select * from {1} ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("delete from {0} ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("end");

                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("if @TaskType='2'");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("begin");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("delete from {0} ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("delete [{0}]  from [{0}] v1,[{1}] v2 where " + stWhereCondition_v, "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Export"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("insert into {0} select " + stColumn + " from [{1}] v2 ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Export"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("delete from {0} ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Export"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("end");

                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("if @TaskType='3'");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("begin");
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("delete from {0} ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("delete from {0} ", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Export"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append("end");
            }

            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("IF @@ERROR<>0");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("BEGIN");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("ROLLBACK TRANSACTION--事务回滚语句");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("return 0");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("End");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("ELSE");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("BEGIN");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("COMMIT TRANSACTION--事务提交语句");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("return 1");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append("End");
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(" END ");

            ExecuteSql.ExeComSqlForNonQuery(sbTotalSpSql.ToString());
            sbTotalSpSql.Clear();
        }

        /// <summary>
        /// 摘要：任务删除时，删除其下的所有表、视图、存储过程
        /// </summary>
        /// <param name="TaskName"></param>
        public override void ClearDataBase(string TaskName, string Table)
        {
            StringBuilder sbTotalSpSql = new StringBuilder();//构建整合存储过程SQL
            for (int i = 0; i < Table.Split(',').Length; i++)
            {
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("if object_id('{0}') is not null", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("drop table [{0}]", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All"));

                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("if object_id('{0}') is not null", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("drop table [{0}]", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp"));

                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("if object_id('{0}') is not null", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Export"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("drop table [{0}]", "T_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Export"));

                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("if object_id('{0}') is not null", "V_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("drop view [{0}]", "V_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_All"));


                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("if object_id('{0}') is not null", "V_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp"));
                sbTotalSpSql.Append(Environment.NewLine);
                sbTotalSpSql.Append(string.Format("drop view [{0}]", "V_" + TaskName + "_" + Table.Split(',')[i].ToString() + "_Temp"));
            }

            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(string.Format("if object_id('{0}') is not null", "P_" + TaskName));
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(string.Format("drop Procedure [{0}]", "P_" + TaskName));

            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(string.Format("if object_id('{0}') is not null", "P_" + TaskName + "_clear"));
            sbTotalSpSql.Append(Environment.NewLine);
            sbTotalSpSql.Append(string.Format("drop Procedure [{0}]", "P_" + TaskName + "_clear"));
            ExecuteSql.ExeComSqlForNonQuery(sbTotalSpSql.ToString());
        }
        public override int ExecProcedure(string procedureName, params object[] parameterValues)
        {
            return ExecuteSql.ExeProcedureNonQuery(procedureName, parameterValues);
        }
        public override int ExecProcedure(string procedureName, string[] parameterNames, params object[] parameterValues)
        {
            return ExecuteSql.ExeProcedureNonQuery(procedureName, parameterNames, parameterValues);
        }
        #endregion
    }
}
