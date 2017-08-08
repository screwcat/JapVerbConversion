using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Common.DB.Core
{
    /// <summary>
    /// DELETE 语句对象
    /// </summary>
    public class Delete
    {
        readonly string SPACE_CHARACTER = " ";
        readonly string DELETE_CHARACTER = "DELETE";
        readonly string FROM_CHARACTER = "FROM";
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
        /// 摘要：前置表名，当DELETE 语句中表有别名时，会在DELETE 和 FROM 之间加一个前置表名用于指定删除的表。
        /// </summary>
        string _FrontTableName = string.Empty;
        public string FrontTableName
        {
            get { return _FrontTableName; }
            set { _FrontTableName = value; }
        }
        /// <summary>
        /// 摘要：条件语句
        /// </summary>
        StringBuilder _DeleteWhere = new StringBuilder();
        public StringBuilder DeleteWhere
        {
            get { return _DeleteWhere; }
            set
            {
                if (_DeleteWhere.ToString().Equals(String.Empty))
                {
                    _DeleteWhere.Append("1=1");
                }
                else
                {
                    _DeleteWhere = value;
                }
            }
        }
        /// <summary>
        /// 摘要：构造
        /// </summary>
        public Delete()
        { }
        public Delete(bool hasWhere)
        {
            this._HasWhere = hasWhere;
        }
        public Delete(bool hasWhere, string frontTableName, string tableName, StringBuilder deleteWhere)
        {
            this._HasWhere = hasWhere;
            _FrontTableName = frontTableName;
            _TableName = tableName;
            _DeleteWhere = deleteWhere;
        }
        /// <summary>
        /// 摘要：拼接DELETE语句
        /// </summary>
        /// <returns>DELETE语句</returns>
        public string ConnectDeleteString()
        {
            StringBuilder sbDelete = new StringBuilder();
            sbDelete.Append(DELETE_CHARACTER);
            sbDelete.Append(SPACE_CHARACTER);
            sbDelete.Append(_FrontTableName.ToString());
            sbDelete.Append(SPACE_CHARACTER);
            sbDelete.Append(FROM_CHARACTER.ToString());
            sbDelete.Append(SPACE_CHARACTER);
            sbDelete.Append(_TableName.ToString());
            sbDelete.Append(SPACE_CHARACTER);
            if (_HasWhere)
            {
                sbDelete.Append(WHERE_CHARACTER);
                sbDelete.Append(SPACE_CHARACTER);
                sbDelete.Append(_DeleteWhere.ToString());
            }
            return sbDelete.ToString();
        }
    }
}
