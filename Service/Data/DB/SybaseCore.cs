using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Common.DB
{
    /// <summary>
    /// Sybase数据库操作相关类
    /// </summary>
    public class SybaseCore : DBCoreBase
    {
        /// <summary>
        /// 摘要：Sybase 构造函数
        /// </summary>
        public SybaseCore()
        {
            this._Transfer_B = "";
            this._Transfer_E = "";
            this._VariableC = "@";
            this._FrontDataRow = "set   rowcount   {1};select * from {0};set   rowcount   0";
        }

    }
}
