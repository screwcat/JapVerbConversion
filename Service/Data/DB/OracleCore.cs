using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.OleDb;
using System.Data;
using Oracle.DataAccess.Client;
using Service.Common.DB.Core;
using Service.Common.Data;

namespace Service.Common.DB
{
    /// <summary>
    /// Oracle数据库操作相关类
    /// </summary>
    public class OracleCore : DBCoreBase
    {
        #region 构造函数
        /// <summary>
        /// 摘要：Oracle 构造函数
        /// </summary>
        public OracleCore()
        {
            this._Transfer_B = "\"";
            this._Transfer_E = "\"";
            this._VariableC = ":";
            this._FrontDataRow = "select * from \"{0}\" where rownum < {1}";
            this._SelectUserTables = "select object_name as Table_Name from user_objects  where object_type='TABLE' or object_type='VIEW' order by object_name";
            this._SelectUserColumns = "select column_name as Column_Name,data_type as type from user_tab_columns where table_name='{0}'";
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
                        sbColumnName.Append(_Transfer_B);
                        sbColumnName.Append(drTemp.Name);
                        sbColumnName.Append(_Transfer_E);
                        sbColumnName.Append("=");
                        sbColumnName.Append(_VariableC + drTemp.Name);
                        sbColumnName.Append(",");

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
                sbUpdateWhere.Append(_VariableC + PK + "_" + i.ToString());
                lisParameter.Add(_VariableC + PK + "_" + i.ToString());
                lisParameterValues.Add(value[i]);
            }
            sbUpdateWhere.Append(")");
            Delete DeleteSql = new Delete(true, base._FrontTableName, base._TableName, sbUpdateWhere);
            int iResult = ExecuteSql.ExeParaSqlForNonQuery(DeleteSql.ConnectDeleteString(), lisParameter, lisParameterValues);
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
                OracleConnection oConn = new OracleConnection(_ConnectionString);
                oConn.Open();
                OracleCommand dbCommand = new OracleCommand(sql, oConn);
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
                OracleConnection oConn = new OracleConnection(_ConnectionString);
                OracleDataAdapter oddaAdapter = new OracleDataAdapter(string.Format(_FrontDataRow, tableName, _RowCount.ToString()), oConn);
                oddaAdapter.Fill(dt);
            }
            return dt;
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
                OracleConnection oConn = new OracleConnection(_ConnectionString);
                OracleDataAdapter oddaAdapter = new OracleDataAdapter(_SelectUserTables, oConn);
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
                OracleConnection oConn = new OracleConnection(_ConnectionString);
                OracleDataAdapter oddaAdapter = new OracleDataAdapter(string.Format(_SelectUserColumns, tableName), oConn);
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
                case "VARCHAR":
                    stStandardDataType = "varchar";
                    break;
                case "VARCHAR2":
                    stStandardDataType = "varchar";
                    break;
                case "nVARCHAR2":
                    stStandardDataType = "varchar";
                    break;
                case "CHAR":
                    stStandardDataType = "varchar";
                    break;
                case "LONG":
                    stStandardDataType = "varchar";
                    break;
                case "BLOB":
                    stStandardDataType = "varchar";
                    break;
                case "CLOB":
                    stStandardDataType = "varchar";
                    break;
                case "NCLOB":
                    stStandardDataType = "varchar";
                    break;

                /*小数型*/
                case "NUMBER":
                    stStandardDataType = "numeric";
                    break;

                /*日期型*/
                case "DATE":
                    stStandardDataType = "datetime";
                    break;

                /*二进制*/
                case "RAW":
                    stStandardDataType = "binary";
                    break;
                case "LONG RAW":
                    stStandardDataType = "binary";
                    break;
                case "BINARY_DOUBLE":
                    stStandardDataType = "binary";
                    break;
                case "BINARY_FLOAT":
                    stStandardDataType = "binary";
                    break;

                /*其他*/
                case "TIMESTAMP(6)":
                    stStandardDataType = "other";
                    break;
                case "TIMESTAMP(6) WITH LOCAL TIME ZONE":
                    stStandardDataType = "other";
                    break;
                case "TIMESTAMP(6) WITH TIME ZONE":
                    stStandardDataType = "other";
                    break;
                case "INTERVAL DAY(2) TO SECOND(6)":
                    stStandardDataType = "other";
                    break;
                case "INTERVAL YEAR(2) TO MONTH":
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
                    stTempString = "to_date(" + tableName + "." + columnName + ",'yyyy-MM-dd')" + " " + condition + " " + "'" + Convert.ToDateTime(value).ToString("yyyy-MM-dd") + "'";
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
            string stSQL = " SELECT \"" + string.Join("\".\"", string.Join("\",\"", columns.Split(',')).Split('.')) + "\" FROM \"" + tableName + "\" ";
            StringBuilder sbthisConditions = new StringBuilder();
            StringBuilder sbotherConditions = new StringBuilder();
            foreach (string stTemp in thisConditions)
            {
                sbthisConditions.Append("\"" + stTemp.Substring(0, stTemp.IndexOf('.')) + "\".\"" + stTemp.Substring(stTemp.IndexOf('.') + 1, stTemp.IndexOf(' ') - stTemp.IndexOf('.') - 1) + "\" " + stTemp.Substring(stTemp.IndexOf(' ') + 1));
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
                    sbOtherCondition.Append("\"" + stSubTemp.Substring(0, stSubTemp.IndexOf('.')) + "\".\"" + stSubTemp.Substring(stSubTemp.IndexOf('.') + 1, stSubTemp.IndexOf(' ') - stSubTemp.IndexOf('.') - 1) + "\" " + stSubTemp.Substring(stSubTemp.IndexOf(' ') + 1));
                    sbOtherCondition.Append(" and ");
                }
                string stRelation = string.Format("\"{0}\".\"{1}\"=\"{2}\".\"{3}\"", stTemp.Split('=')[0].Split('.')[0], stTemp.Split('=')[0].Split('.')[1], stTemp.Split('=')[1].Split('.')[0], stTemp.Split('=')[1].Split('.')[1]);
                sbotherConditions.Append(string.Format(" exists (select null from \"{0}\" where {1} {2}) ", stOtherTable, stRelation, string.IsNullOrEmpty(sbOtherCondition.ToString()) ? string.Empty : " and " + sbOtherCondition.ToString().Substring(0, sbOtherCondition.ToString().Length - 4)));
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
                //RemoveProvider();
                string stTempConnectionString = _ConnectionString;
                string[] rems = stTempConnectionString.Split(';');
                foreach (string rem in rems)
                {
                    if (rem.Contains("Provider"))
                    {
                        stTempConnectionString = stTempConnectionString.Remove(stTempConnectionString.IndexOf(rem), rem.Length + 1);
                    }
                }
                OracleConnection oConn = new OracleConnection(stTempConnectionString);
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
        /// 摘要：创建存储过程（交换系统）
        /// </summary>
        /// <param name="TaskName">任务名称</param>
        /// <param name="Table">表名</param>
        /// <param name="Column">列名</param>
        /// <param name="TableKey">主键名</param>
        public void CreateLoadProcedure(string TaskName, string Table, string Columns, string TableKey)
        {
            StringBuilder sbSpSql = new StringBuilder();//构建存储过程SQL
            StringBuilder sbTotalSpSql = new StringBuilder();//构建整合存储过程SQL
            string stCloumns = string.Empty;//列
            string stKey = string.Empty;
            string stWhereCondition = string.Empty;
            string stUpdateCondition = string.Empty;
            string stColumn = string.Empty;
            //验证是否存在

            //表头
            sbTotalSpSql.AppendLine(string.Format("Create or Replace PROCEDURE {0}", "P_" + TaskName));
            sbTotalSpSql.AppendLine("(");
            sbTotalSpSql.AppendLine("Cloumn_Batch  Varchar , resultflag out Varchar ");
            sbTotalSpSql.AppendLine(")");
            sbTotalSpSql.AppendLine("AS Begin");
            //循环表
            for (int i = 0; i < Table.Split(',').Length; i++)
            {
                //列集合 
                stCloumns = Columns.Split(';')[i].ToString();
                //where 条件
                stWhereCondition = string.Empty;
                for (int j = 0; j < TableKey.Split(';')[i].Split(',').Length; j++)
                {
                    if (j == 0)
                        stWhereCondition += string.Format(" {1}=t.{1} ", Table.Split(',')[i], TableKey.Split(';')[i].Split(',')[j]);
                    else
                        stWhereCondition += string.Format(" and {1}=t.{1} ", Table.Split(',')[i], TableKey.Split(';')[i].Split(',')[j]);
                }

                //插入新增数据
                sbTotalSpSql.AppendLine(string.Format("insert into {0} ({1}) select {1} from {2} where Cloumn_Batch=:Cloumn_Batch and ColumnFlag='1'; ", Table.Split(',')[i], stCloumns, "T_" + TaskName + "_" + Table.Split(',')[i]));
                //删除数据
                sbTotalSpSql.AppendLine(string.Format("delete from {0} t where exists (select 1 from {1} where {2} ) and  Cloumn_Batch=:Cloumn_Batch and ColumnFlag='3' ;", Table.Split(',')[i], "T_" + TaskName + "_" + Table.Split(',')[i], stWhereCondition));
                //修改数据
                sbTotalSpSql.AppendLine(string.Format("update {0} t set ({1})=(select {1} from {2} where {3} and  Cloumn_Batch=:Cloumn_Batch and ColumnFlag='2') where exists (select 1 from {2} where {3} and  Cloumn_Batch=:Cloumn_Batch and ColumnFlag='2') ;"));
            }
            sbTotalSpSql.AppendLine("resultflag:=1;");
            sbTotalSpSql.AppendLine("exception when others then");
            sbTotalSpSql.AppendLine(" resultflag:=0;");
            sbTotalSpSql.AppendLine(" rollback;");
            sbTotalSpSql.AppendLine("End;");
            ExecuteSql.ExeComSqlForNonQuery(sbTotalSpSql.ToString());
            sbTotalSpSql.Clear();
        }
        #endregion
    }
}
