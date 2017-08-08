using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Common.DB.Core
{
    /// <summary>
    /// SELECT 语句对象
    /// </summary>
    public class Select
    {
        readonly string SPACE_CHARACTER = " ";
        readonly string SELECT_CHARACTER = "SELECT";
        readonly string FROM_CHARACTER = "FROM";
        readonly string WHERE_CHARACTER = "WHERE";
        readonly string ORDER_BY_CHARACTER = "ORDER BY";
        readonly string GROUP_BY_CHARACTER = "GROUP BY";

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
        /// 摘要：是否包含ORDERBY排序
        /// </summary>
        bool _HasOrder = false;
        public bool HasOrder
        {
            get { return _HasOrder; }
            set { _HasOrder = value; }
        }
        /// <summary>
        /// 摘要：是否包含GROUPBY分组
        /// </summary>
        bool _HasGroup = false;
        public bool HasGroup
        {
            get { return _HasGroup; }
            set { _HasGroup = value; }
        }
        /// <summary>
        /// 摘要：要查询的字段
        /// </summary>
        StringBuilder _SelectColumns = new StringBuilder();
        public StringBuilder SelectColumns
        {
            get { return _SelectColumns; }
            set { _SelectColumns = value; }
        }
        /// <summary>
        /// 摘要：表名，包括多个表名用于做表关联
        /// </summary>
        StringBuilder _TableName = new StringBuilder();
        public StringBuilder TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }
        /// <summary>
        /// 摘要：条件语句
        /// </summary>
        StringBuilder _SelectWhere = new StringBuilder();
        public StringBuilder SelectWhere
        {
            get { return _SelectWhere; }
            set { _SelectWhere = value; }
        }
        /// <summary>
        /// 摘要：排序字段
        /// </summary>
        StringBuilder _SelectOrder = new StringBuilder();
        public StringBuilder SelectOrder
        {
            get { return _SelectOrder; }
            set { _SelectOrder = value; }
        }
        /// <summary>
        /// 摘要：排序方式，降序或者升序。
        /// </summary>
        OrderBy _OrderMode = 0;
        public OrderBy OrderMode
        {
            get { return _OrderMode; }
            set { _OrderMode = value; }
        }
        /// <summary>
        /// 摘要：分组字段
        /// </summary>
        StringBuilder _SelectGroup = new StringBuilder();
        public StringBuilder SelectGroup
        {
            get { return _SelectGroup; }
            set { _SelectGroup = value; }
        }
        /// <summary>
        /// 摘要：构造
        /// </summary>
        public Select()
        { }
        public Select(bool hasWhere, bool hasOrder, bool hasGroup)
        {
            this._HasWhere = hasWhere;
            this._HasOrder = hasOrder;
            this._HasGroup = hasGroup;
        }
        public Select(bool hasWhere, bool hasOrder, bool hasGroup, StringBuilder selectColumns, StringBuilder tableName,
            StringBuilder selectWhere, StringBuilder selectOrder, OrderBy orderMode, StringBuilder selectGroup)
        {
            this._HasWhere = hasWhere;
            this._HasOrder = hasOrder;
            this._HasGroup = hasGroup;
            this._SelectColumns = selectColumns;
            this._TableName = tableName;
            this._SelectWhere = selectWhere;
            this._SelectOrder = selectOrder;
            this._OrderMode = orderMode;
            this._SelectGroup = selectGroup;
        }
        /// <summary>
        /// 摘要：拼接SELECT语句
        /// </summary>
        /// <returns>SELECT语句</returns>
        public string ConnectSelectString()
        {
            StringBuilder sbSelect=new StringBuilder ();
            sbSelect.Append(SELECT_CHARACTER);
            sbSelect.Append(SPACE_CHARACTER);
            sbSelect.Append(_SelectColumns.ToString());
            sbSelect.Append(SPACE_CHARACTER);
            sbSelect.Append(FROM_CHARACTER);
            sbSelect.Append(SPACE_CHARACTER);
            sbSelect.Append(_TableName.ToString());
            if (_HasWhere)
            {
                sbSelect.Append(SPACE_CHARACTER);
                sbSelect.Append(WHERE_CHARACTER);
                sbSelect.Append(SPACE_CHARACTER);
                sbSelect.Append(_SelectWhere.ToString());
            }
            if (_HasOrder)
            {
                 sbSelect.Append(SPACE_CHARACTER);
                sbSelect.Append(ORDER_BY_CHARACTER);
                sbSelect.Append(SPACE_CHARACTER);
                sbSelect.Append(_SelectOrder.ToString());
                sbSelect.Append(SPACE_CHARACTER);
                sbSelect.Append(_OrderMode.ToString());
            }
            if (_HasGroup)
            {
                        sbSelect.Append(SPACE_CHARACTER);
                sbSelect.Append(GROUP_BY_CHARACTER);
                sbSelect.Append(SPACE_CHARACTER);
                sbSelect.Append(_SelectGroup.ToString());
            }
            return sbSelect.ToString();
        }
    }
}
