using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using Service.Common.Data;

namespace Service.Common.DB
{
    /// <summary>
    /// MySql数据库操作相关类
    /// </summary>
    public class MySqlCore : DBCoreBase
    {
        #region 构造函数
        /// <summary>
        /// 摘要：MySql 构造函数
        /// </summary>
        public MySqlCore()
        {
            this._Transfer_B = "";
            this._Transfer_E = "";
            this._VariableC = "@";
            this._FrontDataRow = "select * from {0} limit {1}";
            this._SelectUserTables = "show tables";
            this._SelectUserColumns = "desc {0}";

        }

        #endregion

        #region 方法
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
                MySqlConnection oConn = new MySqlConnection(_ConnectionString);
                oConn.Open();
                MySqlCommand dbCommand = new MySqlCommand(sql, oConn);
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
        public override DataTable GetPreviewData(string sql)
        {
            DataTable dt = null;
            if (_DBprovider.ToUpper().Contains("OLEDB"))
            {
                dt = base.GetPreviewData(sql);
            }
            else
            {
                RemoveProvider();
                MySqlConnection oConn = new MySqlConnection(_ConnectionString);
                MySqlDataAdapter oddaAdapter = new MySqlDataAdapter(string.Format(_FrontDataRow, sql, _RowCount.ToString()), oConn);
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
                MySqlConnection oConn = new MySqlConnection(_ConnectionString);
                MySqlDataAdapter oddaAdapter = new MySqlDataAdapter(_SelectUserTables, oConn);
                oddaAdapter.Fill(schemaTable);
                schemaTable.Columns[0].ColumnName = "Table_Name";
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
                dtCloumnNames = oConn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new Object[] { null, null, tableName, null });
                oConn.Close();
            }
            else
            {
                RemoveProvider();
                MySqlConnection oConn = new MySqlConnection(_ConnectionString);
                MySqlDataAdapter oddaAdapter = new MySqlDataAdapter(string.Format(_SelectUserColumns, tableName), oConn);
                oddaAdapter.Fill(dtCloumnNames);
                dtCloumnNames.Columns[0].ColumnName = "Column_Name";
                dtCloumnNames.Columns[1].ColumnName = "type";
            }
            return dtCloumnNames;
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
                MySqlConnection oConn = new MySqlConnection(_ConnectionString);
                try
                {
                    RemoveProvider();
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
            sbTotalSpSql.AppendLine(string.Format("drop PROCEDURE if EXISTS {0}", "P_" + TaskName));//删除存储过程
            //头
            sbTotalSpSql.AppendLine(string.Format("Create procedure {0} ( in_Cloumn_Batch Varchar(10) ,OUT resultflag Varchar(1) )"));
            sbTotalSpSql.AppendLine("Begin");
            sbTotalSpSql.AppendLine("START TRANSACTION;");//开启事务
            //循环表
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
                sbTotalSpSql.AppendLine(string.Format("insert into {0} ({1}) select {1} from {2} where Cloumn_Batch=in_Cloumn_Batch and ColumnFlag='1' ", Table.Split(',')[i], stCloumns, "T_" + TaskName + "_" + Table.Split(',')[i]));


                //删除数据
                sbTotalSpSql.AppendLine(string.Format("delete {0} from {1} t where {2} and Cloumn_Batch=in_Cloumn_Batch and ColumnFlag='3'", Table.Split(',')[i], "T_" + TaskName + "_" + Table.Split(',')[i], stWhereCondition));

                //修改数据
                sbTotalSpSql.AppendLine(string.Format("update {0} set {1} from {0},{2} t where {3} and Cloumn_Batch=in_Cloumn_Batch and ColumnFlag='2'", Table.Split(',')[i], stUpdateCondition, "T_" + TaskName + "_" + Table.Split(',')[i], stWhereCondition));
            }
            sbTotalSpSql.AppendLine("COMMIT;");//事务提交
            sbTotalSpSql.AppendLine("End");
            ExecuteSql.ExeComSqlForNonQuery(sbTotalSpSql.ToString());
            sbTotalSpSql.Clear();
        }
        #endregion
    }
}
