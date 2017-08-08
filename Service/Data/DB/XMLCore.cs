using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using Service.Common.Entity;
using System.Reflection;

namespace Service.Common.DB
{
    /// <summary>
    /// XML型文件数据库操作类
    /// </summary>
    public class XMLCore : IDBCore
    {
        #region 变量
        /// <summary>
        /// 摘要：文件路径
        /// </summary>
        protected string _Path = string.Empty;
        public string Path
        {
            get { return _Path; }
            set { _Path = value; }
        }
        /// <summary>
        /// 摘要：表名
        /// </summary>
        protected string _TableName = string.Empty;
        #endregion

        #region 构造函数

        public XMLCore()
        { }
        public XMLCore(string path)
        {
            this._Path = path;
        }

        #endregion
        /// <summary>
        /// 摘要：插入一条记录
        /// </summary>
        /// <param name="T">实体类对象</param>
        /// <returns>自增长主键时返回主键值</returns>
        public string Insert(object T)
        {
            _TableName = ((TableAttribute)T.GetType().GetCustomAttributes(true)[0]).TableName;
            string stTempColumn = string.Empty;
            string stTempValue = string.Empty;
            XElement root = XElement.Load(_Path);
            XElement user = root.Element(_TableName);
            PropertyInfo[] propertyInfos = T.GetType().GetProperties();
            XElement[] lxColumns = new XElement[propertyInfos.Length - 1];
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                if (propertyInfos[i].Name == "TagData" || (propertyInfos[i].GetCustomAttributes(true)[0].GetType().Name != "PrimaryKeyAttribute" && !((PropertyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).IsStorage)) continue;
                if (propertyInfos[i].GetCustomAttributes(true).Length != 0)
                {
                    if (propertyInfos[i].GetCustomAttributes(true)[0].GetType().Name != "PrimaryKeyAttribute")
                    {
                        if (((PropertyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).ColumnName == string.Empty)
                        {
                            stTempColumn = propertyInfos[i].Name;
                        }
                        else
                        {
                            stTempColumn = ((PropertyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).ColumnName;
                        }
                    }
                    else
                    {
                        stTempColumn = ((PrimaryKeyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).ColumnName;
                    }
                    stTempValue = propertyInfos[i].GetValue(T, null).ToString();
                }
                lxColumns[i] = new XElement(stTempColumn, stTempValue);
            }
            user.AddBeforeSelf(new XElement(_TableName, lxColumns));
            root.Save(_Path);
            return string.Empty;

        }
        /// <summary>
        /// 摘要：更新一条记录
        /// </summary>
        /// <param name="T">实体类对象</param>
        public void Update(object T)
        {
            _TableName = ((TableAttribute)T.GetType().GetCustomAttributes(true)[0]).TableName;
            string stTempColumn = string.Empty;
            string stTempValue = string.Empty;
            string[] stPK = new string[2];
            XElement root = XElement.Load(_Path);
            PropertyInfo[] propertyInfos = T.GetType().GetProperties();
            XElement[] lxColumns = new XElement[propertyInfos.Length - 1];
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                if (propertyInfos[i].Name == "TagData" || (propertyInfos[i].GetCustomAttributes(true)[0].GetType().Name != "PrimaryKeyAttribute" && !((PropertyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).IsStorage)) continue;
                if (propertyInfos[i].GetCustomAttributes(true).Length != 0)
                {
                    if (propertyInfos[i].GetCustomAttributes(true)[0].GetType().Name != "PrimaryKeyAttribute")
                    {
                        if (((PropertyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).ColumnName == string.Empty)
                        {
                            stTempColumn = propertyInfos[i].Name;
                        }
                        else
                        {
                            stTempColumn = ((PropertyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).ColumnName;
                        }
                    }
                    else
                    {
                        stTempColumn = ((PrimaryKeyAttribute)(propertyInfos[i].GetCustomAttributes(true)[0])).ColumnName;
                        stPK[0] = stTempColumn;
                        stPK[1] = propertyInfos[i].GetValue(T, null).ToString();
                    }
                    stTempValue = propertyInfos[i].GetValue(T, null).ToString();
                }
                lxColumns[i] = new XElement(stTempColumn, stTempValue);
            }
            root.Elements(_TableName).First((t) => t.Element(stPK[0]).Value == stPK[1]).ReplaceWith(new XElement(_TableName, lxColumns));
            root.Save(_Path);
        }
        /// <summary>
        /// 摘要：删除记录
        /// </summary>
        /// <param name="PK">主键</param>
        /// <param name="tableName">表名</param>
        /// <param name="value">要删除的主键值数组</param>
        public void Delete(string PK, string tableName, List<string> value)
        {
            XElement root = XElement.Load(_Path);
            foreach (string stValue in value)
            {
                root.Elements(tableName).First((t) => t.Element(PK).Value == stValue).Remove();
            }
            root.Save(_Path);
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
            DataSet dsTemp = new DataSet();
            dsTemp.ReadXml(_Path);
            if (dsTemp.Tables.Count > 0)
            {
                DataRow[] drTemp = dsTemp.Tables[0].Select(ConstructWhere(where), ConstructOrderBy(orderByColumn));
                DataTable dtTemp = dsTemp.Tables[0].Clone();
                if (rowBegin > drTemp.Length || rowEnd == 0) return dtTemp;
                for (int i = rowBegin; i < drTemp.Length; i++)
                {
                    if (i == rowEnd) break;
                    dtTemp.ImportRow(drTemp[i]);
                }
                return dtTemp;
            }
            else
                return null;
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
            DataSet dsTemp = new DataSet();
            dsTemp.ReadXml(_Path);
            DataRow[] drTemp = dsTemp.Tables[0].Select(ConstructWhere(where), ConstructOrderBy(orderByColumn));
            DataTable dtTemp = dsTemp.Tables[0].Clone();
            for (int i = 0; i < drTemp.Length; i++)
            {
                dtTemp.ImportRow(drTemp[i]);
            }
            return dtTemp;
        }
        /// <summary>
        /// 摘要：根据条件查询数据集记录条数
        /// </summary>
        /// <param name="dataSetSql">查询语句（包括列名、表等信息，支持表关联）</param>
        /// <param name="where">条件组，二维数据【0】为包括字段名的语句段模板，【1】为取值</param>
        /// <returns></returns>
        public DataTable GetDataSet(string dataSetSql, List<string[]> where)
        {
            DataSet dsTemp = new DataSet();
            dsTemp.ReadXml(_Path);
            if (dsTemp.Tables.Count > 0)
            {
                DataRow[] drTemp = dsTemp.Tables[0].Select(ConstructWhere(where));
                DataTable dtTemp = dsTemp.Tables[0].Clone();
                foreach (DataRow dr in drTemp)
                {
                    dtTemp.ImportRow(dr);
                }
                return dtTemp;
            }
            else
                return null;

        }
        /// <summary>
        /// 摘要：根据条件查询数据集记录条数
        /// </summary>
        /// <param name="dataSetSql">查询语句（包括列名、表等信息，支持表关联）</param>
        /// <param name="where">条件组，二维数据【0】为包括字段名的语句段模板，【1】为取值</param>
        /// <returns></returns>
        public int GetDataSetCount(string dataSetSql, List<string[]> where)
        {
            DataSet dsTemp = new DataSet();
            //dsTemp.ReadXml(_Path);

            StreamReader stream = new StreamReader(_Path);
            dsTemp.ReadXml(stream);

            if (dsTemp.Tables.Count > 0)
            {
                DataRow[] drTemp = dsTemp.Tables[0].Select(ConstructWhere(where));
                return drTemp.Length;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 摘要：构造where条件
        /// </summary>
        /// <param name="whereParam">条件集合</param>
        /// <returns>条件语句</returns>
        private string ConstructWhere(List<string[]> whereParam)
        {
            string stWhere = string.Empty;
            List<string> Sql = new List<string>();
            for (int i = 0; i < whereParam.Count; i++)
            {
                if (!string.IsNullOrEmpty(whereParam[i][1].Trim()))
                {
                    Sql.Add(string.Format(whereParam[i][0], whereParam[i][1]));
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
    }
}
