using System;
using System.Collections.Generic;
using System.Text;


namespace Service.Common.Entity
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false), Serializable]
    public class TableAttribute : Attribute
    {
        #region 变量
        static string _TableName = string.Empty;
        #endregion

        #region 构造
        public TableAttribute()
        {}
        public TableAttribute(string tableName)
        { _TableName = tableName; }
        #endregion

        #region 属性
        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                _TableName = value;
            }
        }
        #endregion
    }
}
