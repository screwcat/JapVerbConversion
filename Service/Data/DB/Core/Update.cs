using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Common.DB.Core
{
    /// <summary>
    /// UPDATE 语句对象
    /// </summary>
    public class Update
    {
        readonly string SPACE_CHARACTER = " ";
        readonly string UPDATE_CHARACTER = "UPDATE";
        readonly string SET_CHARACTER = "SET";
        readonly string WHERE_CHARACTER = "WHERE";

        /// <summary>
        /// 摘要：是否包含WHERE条件
        /// </summary>
        bool _HasWhere = true;
        public bool HasWhere
        {
            get { return _HasWhere; }
            set { _HasWhere = value; }
        }
        /// <summary>
        /// 摘要：表名，包括多个表名用于做表关联
        /// </summary>
        string _TableName = string.Empty;
        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }
        /// <summary>
        /// 摘要：更新字段表达式数组
        /// </summary>
        StringBuilder _UpdateColumns = new StringBuilder();
        public StringBuilder UpdateColumns
        {
            get { return _UpdateColumns; }
            set { _UpdateColumns = value; }
        }
        /// <summary>
        /// 摘要：条件语句
        /// </summary>
        StringBuilder _UpdateWhere = new StringBuilder();
        public StringBuilder UpdateWhere
        {
            get { return _UpdateWhere; }
            set
            {
                if (_UpdateWhere.ToString().Equals(String.Empty))
                {
                    _UpdateWhere.Append("1=1");
                }
                else
                {
                    _UpdateWhere = value;
                }
            }
        }
        /// <summary>
        /// 摘要：构造
        /// </summary>
        public Update()
        { }
        public Update(bool hasWhere)
        {
            this._HasWhere = hasWhere;
        }
        public Update(bool hasWhere, string tableName, StringBuilder updateColumns, StringBuilder updateWhere)
        {
            this._HasWhere = hasWhere;
            this._UpdateColumns = updateColumns;
            this._TableName = tableName;
            this._UpdateWhere = updateWhere;
        }
        /// <summary>
        /// 摘要：拼接UPDATE语句
        /// </summary>
        /// <returns>UPDATE语句</returns>
        public string ConnectUpdateString()
        {
            StringBuilder sbUpdate = new StringBuilder();
            sbUpdate.Append(UPDATE_CHARACTER);
            sbUpdate.Append(SPACE_CHARACTER);
            sbUpdate.Append(_TableName.ToString());
            sbUpdate.Append(SPACE_CHARACTER);
            sbUpdate.Append(SET_CHARACTER);
            sbUpdate.Append(_UpdateColumns.ToString());
            sbUpdate.Append(SPACE_CHARACTER);
            if (_HasWhere)
            {
                sbUpdate.Append(WHERE_CHARACTER);
                sbUpdate.Append(SPACE_CHARACTER);
                sbUpdate.Append(_UpdateWhere);
                sbUpdate.Append(SPACE_CHARACTER);
            }
            return sbUpdate.ToString();
        }
    }
}
