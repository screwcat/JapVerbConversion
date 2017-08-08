using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Common.DB.Core
{
    /// <summary>
    /// 用列值方式描述的一条记录类
    /// </summary>
    public class Record
    {
        /// <summary>
        /// 摘要:列名
        /// </summary>
        protected string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// 摘要:类型
        /// </summary>
        protected string _Type = string.Empty;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        /// <summary>
        /// 摘要:是否自增长
        /// </summary>
        protected int _IsAuto = 0;
        public int IsAuto
        {
            get { return _IsAuto; }
            set { _IsAuto = value; }
        }
        /// <summary>
        /// 摘要:是否主键
        /// </summary>
        protected int _IsPK = 0;
        public int IsPK
        {
            get { return _IsPK; }
            set { _IsPK = value; }
        }
        /// <summary>
        /// 摘要:取值
        /// </summary>
        protected object _Value = null;
        public object Value
        {
            get { return _Value; }
            set { _Value = value;}
        }
    }
}
