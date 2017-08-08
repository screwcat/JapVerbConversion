using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Common.Entity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false), Serializable]
    public class PrimaryKeyAttribute : Attribute
    {
        #region 变量
        string _ColumnName = string.Empty;
        bool _IsAuto = false;
        #endregion

        #region 构造
        public PrimaryKeyAttribute()
        {}
        public PrimaryKeyAttribute(string columnName)
        {
            _ColumnName = columnName;
        }
        public PrimaryKeyAttribute(string columnName, bool isAuto)
        {
            _ColumnName = columnName;
            _IsAuto = isAuto;
        }
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
        public bool IsAuto
        {
            get
            {
                return _IsAuto;
            }
            set
            {
                _IsAuto = value;
            }
        }
        #endregion
    }
}
