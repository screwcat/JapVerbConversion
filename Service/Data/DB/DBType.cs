using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Common.DB
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBType
    {
        /// <summary>
        /// 摘要：Sql Server 数据库
        /// </summary>
        MSSql,
        /// <summary>
        /// 摘要：Oracle 数据库
        /// </summary>
        Oracle,
        /// <summary>
        /// 摘要：xml文件数据库
        /// </summary>
        XML
    }
}
