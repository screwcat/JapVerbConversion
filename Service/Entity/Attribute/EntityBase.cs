using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Service.Common.Log;
using Service.Common.DB;
using System.Configuration;
using System.Data;



namespace Service.Common.Entity
{
    /// <summary>
    /// 实体泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class EntityBase<T> where T : new()
    {
        #region 变量
        /// <summary>
        /// 摘要：数据库类型描述。与此对应在应用程序配置文件<appSettings>节当中应当存在key为"DBType"的<add>节。
        /// 该节值分为如下几种：SQL SERVER数据库用“MSSql”表示；ORACLE数据库用“Oracle”表示；XML数据库用“XML”表示。
        /// </summary>
        public string DATABASE_TYPE = ConfigurationManager.AppSettings.Get("DBType");
        /// <summary>
        /// 摘要：标签内容，当为XML数据库时为路径，当为关系型数据库时为数据库连接串在配置文件中的名称（可以为空）。
        /// </summary>
        private string _TagData = string.Empty;
        public string TagData
        {
            get { return _TagData; }
            set { _TagData = value; }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 摘要：通用新增方法
        /// </summary>
        /// <returns>执行结果若主键为自增长类型，成功时返回值中message为自增长主键值</returns>
        public ReturnMessage Insert()
        {
            ReturnMessage rm = new ReturnMessage(true);
            string stTableName = string.Empty;
            try
            {
                T obj = (T)this.MemberwiseClone();
                stTableName = ((TableAttribute)obj.GetType().GetCustomAttributes(true)[0]).TableName;
                IDBCore iDB = null;
                if (DATABASE_TYPE == "XML")
                {
                    iDB = new XMLCore(_TagData);
                }
                else
                {
                    iDB = DBFactory.CreateDB();
                }
                rm.Message = iDB.Insert(obj);
            }
            catch (Exception ex)
            {
                rm.IsSucessed = false;
                rm.Message = "表【" + stTableName + "】插入数据失败，原因：" + ex.Message;
            }
            return rm;
        }
        /// <summary>
        /// 摘要：更新一条记录
        /// </summary>
        /// <param name="T">实体类对象</param>
        public ReturnMessage Update()
        {
            ReturnMessage rm = new ReturnMessage(true);
            string stTableName = string.Empty;
            try
            {
                T obj = (T)this.MemberwiseClone();
                stTableName = ((TableAttribute)obj.GetType().GetCustomAttributes(true)[0]).TableName;
                IDBCore iDB = null;
                if (DATABASE_TYPE == "XML")
                {
                    iDB = new XMLCore(_TagData);
                }
                else
                {
                    iDB = DBFactory.CreateDB();
                }
                iDB.Update(obj);
            }
            catch (Exception ex)
            {
                rm.IsSucessed = false;
                rm.Message = "表【" + stTableName + "】更新数据失败，原因：" + ex.Message;
            }
            return rm;
        }
        /// <summary>
        /// 摘要：删除记录
        /// </summary>
        /// <param name="value">要删除的主键值数组</param>
        public ReturnMessage Delete(List<string> value)
        {
            ReturnMessage rm = new ReturnMessage(true);
            string stTableName = string.Empty;
            try
            {
                T obj = (T)this.MemberwiseClone();
                stTableName = ((TableAttribute)obj.GetType().GetCustomAttributes(true)[0]).TableName;
                string stPK = string.Empty;
                PropertyInfo[] propertyInfos = obj.GetType().GetProperties();
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    if (propertyInfos[i].GetCustomAttributes(true).Length != 0)
                    {
                        if (propertyInfos[i].GetCustomAttributes(true)[0].GetType().Name == "PrimaryKeyAttribute")
                        {
                            stPK = propertyInfos[i].Name;
                        }
                    }
                }
                IDBCore iDB = null;
                if (DATABASE_TYPE == "XML")
                {
                    iDB = new XMLCore(_TagData);
                }
                else
                {
                    iDB = DBFactory.CreateDB();
                }
                iDB.Delete(stPK, stTableName, value);
            }
            catch (Exception ex)
            {
                rm.IsSucessed = false;
                rm.Message = "表【" + stTableName + "】删除数据失败，原因：" + ex.Message;
            }
            return rm;
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
        public DataTable GetDataSet(string dataSetSql, List<string[]> where, List<string[]> orderByColumn, int rowBegin, int rowEnd)
        {
            DataTable dt = new DataTable();
            string stTableName = string.Empty;
            try
            {
                T obj = (T)this.MemberwiseClone();
                stTableName = ((TableAttribute)obj.GetType().GetCustomAttributes(true)[0]).TableName;
                IDBCore iDB = null;
                if (DATABASE_TYPE == "XML")
                {
                    iDB = new XMLCore(_TagData);
                }
                else
                {
                    iDB = DBFactory.CreateDB();
                }
                if ((rowBegin == -1 || rowEnd == -1) && orderByColumn != null)
                {
                    dt = iDB.GetDataSet(dataSetSql, where, orderByColumn);
                }
                else if ((rowBegin == -1 || rowEnd == -1) && orderByColumn == null)
                {
                    dt = iDB.GetDataSet(dataSetSql, where);
                }
                else
                {
                    dt = iDB.GetDataSet(dataSetSql, where, orderByColumn, rowBegin, rowEnd);
                }
            }
            catch (Exception ex)
            {
                dt = null;
            }
            return dt;
        }
        /// <summary>
        /// 摘要：根据条件查询数据集
        /// </summary>
        /// <param name="dataSetSql">查询语句（包括列名、表等信息，支持表关联）</param>
        /// <param name="where">条件组，二维数据【0】为包括字段名的语句段模板，【1】为取值</param>
        /// <param name="orderByColumn">排序列，二维数据【0】为字段名，【1】为ASC（升序）或DESC（降序）</param>
        /// <returns></returns>
        public DataTable GetDataSet(string dataSetSql, List<string[]> where, List<string[]> orderByColumn)
        {
            return GetDataSet(dataSetSql, where, orderByColumn, -1, -1);
        }
        /// <summary>
        /// 摘要：根据条件查询数据集
        /// </summary>
        /// <param name="dataSetSql">查询语句（包括列名、表等信息，支持表关联）</param>
        /// <param name="where">条件组，二维数据【0】为包括字段名的语句段模板，【1】为取值</param>
        /// <returns></returns>
        public DataTable GetDataSet(string dataSetSql, List<string[]> where)
        {
            return GetDataSet(dataSetSql, where, null, -1, -1);
        }
        /// <summary>
        /// 摘要：根据条件查询数据集记录条数
        /// </summary>
        /// <param name="dataSetSql">查询语句（包括列名、表等信息，支持表关联）</param>
        /// <param name="where">条件组，二维数据【0】为包括字段名的语句段模板，【1】为取值</param>
        /// <returns></returns>
        public int GetDataSetCount(string dataSetSql, List<string[]> where)
        {
            int iCount = -1;
            string stTableName = string.Empty;
            try
            {
                T obj = (T)this.MemberwiseClone();
                stTableName = ((TableAttribute)obj.GetType().GetCustomAttributes(true)[0]).TableName;
                IDBCore iDB = null;
                if (DATABASE_TYPE == "XML")
                {
                    iDB = new XMLCore(_TagData);
                }
                else
                {
                    iDB = DBFactory.CreateDB();
                }
                iCount = iDB.GetDataSetCount(dataSetSql, where);
            }
            catch (Exception ex)
            {
                ;
            }
            return iCount;
        }
        /// <summary>
        /// 摘要：根据主键查询行对象
        /// </summary>
        /// <param name="PK">主键值</param>
        public void FindbyPK(string PK)
        {
            string stTableName = ((TableAttribute)this.GetType().GetCustomAttributes(true)[0]).TableName;
            string stPK = string.Empty;
            string stType = string.Empty;
            string stTemp = string.Empty;
            PropertyInfo[] propertyInfos = this.GetType().GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                if (propertyInfos[i].GetCustomAttributes(true).Length != 0)
                {
                    if (propertyInfos[i].GetCustomAttributes(true)[0].GetType().Name == "PrimaryKeyAttribute")
                    {
                        stPK = propertyInfos[i].Name;
                        stType = propertyInfos[i].GetType().ToString();
                        stType = propertyInfos[i].PropertyType.ToString();
                    }
                }
            }
            List<string[]> where = new List<string[]>();
            if (DATABASE_TYPE == "MSSql")
            {
                stTemp = "{0}";
            }
            else if (stType.ToString() == "System.String")
            {
                stTemp = "'{0}'";
            }
            else
            {
                stTemp = "{0}";
            }
            where.Add(new string[] { stPK + "=" + stTemp, PK });
            DataTable dt = GetDataSet("SELECT * FROM " + stTableName, where);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    if (propertyInfos[i].Name == "TagData") continue;
                    if (propertyInfos[i].GetCustomAttributes(true)[0].GetType().Name != "PrimaryKeyAttribute" && (!dt.Columns.Contains(((PropertyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).ColumnName)) && !dt.Columns.Contains(propertyInfos[i].Name)) continue;
                    if (propertyInfos[i].GetCustomAttributes(true)[0].GetType().Name == "PrimaryKeyAttribute" && !dt.Columns.Contains(propertyInfos[i].Name)) continue;
                    if (propertyInfos[i].GetCustomAttributes(true)[0].GetType().Name != "PrimaryKeyAttribute")
                    {
                        if (((PropertyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).ColumnName == string.Empty)
                        {
                            if (propertyInfos[i].PropertyType.Name == "Int32")
                            {
                                propertyInfos[i].SetValue(this, Convert.ToInt32(dt.Rows[0][propertyInfos[i].Name]), null);
                            }
                            else if (propertyInfos[i].PropertyType.Name == "DateTime")
                            {
                                propertyInfos[i].SetValue(this, DateTime.Parse(dt.Rows[0][propertyInfos[i].Name].ToString()), null);
                            }
                            else
                            {
                                propertyInfos[i].SetValue(this, dt.Rows[0][propertyInfos[i].Name].ToString(), null);
                            }
                        }
                        else
                        {
                            if (propertyInfos[i].PropertyType.Name == "Int32")
                            {
                                propertyInfos[i].SetValue(this, Convert.ToInt32(dt.Rows[0][((PropertyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).ColumnName]), null);
                            }
                            else
                            {
                                propertyInfos[i].SetValue(this, dt.Rows[0][((PropertyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).ColumnName], null);

                            }
                        }
                    }
                    else
                    {
                        propertyInfos[i].SetValue(this, dt.Rows[0][((PrimaryKeyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).ColumnName], null);
                    }
                }
            }
        }
        #endregion
    }
}
