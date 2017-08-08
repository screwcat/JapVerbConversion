using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Common.Entity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true), Serializable]
    public class PropertyAttribute : Attribute
    {
        #region 变量
        string _ColumnName = string.Empty;
        bool _IsStorage = true;
        #endregion

        #region 构造
        public PropertyAttribute()
        { }
        public PropertyAttribute(string columnName)
        { _ColumnName = columnName; }
        public PropertyAttribute(bool isStorage)
        { _IsStorage = isStorage; }
        #endregion 

        #region 属性
        public string ColumnName
        {
            get
            {
                return _ColumnName;
            }
            set
            {
                _ColumnName = value;
            }
        }
        public bool IsStorage
        {
            get
            {
                return _IsStorage;
            }
            set
            {
                _IsStorage = value;
            }
        }
        #endregion
    }
}
