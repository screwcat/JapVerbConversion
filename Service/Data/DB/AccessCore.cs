using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.OleDb;
using Service.Common.DB.Core;
using Service.Common.Data;

namespace Service.Common.DB
{
    /// <summary>
    /// Access数据库操作相关类
    /// </summary>
    public class AccessCore : DBCoreBase
    {
        /// <summary>
        /// 摘要：Access 构造函数
        /// </summary>
        public AccessCore()
        {
            this._Transfer_B = "[";
            this._Transfer_E = "]";
            this._VariableC = "@";
            this._FrontDataRow = "select Top {1} * from [{0}]";
        }

        #region 方法
        /// <summary>
        /// 摘要：数据库读取
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public override DbDataReader GetReader(string sql)
        {
            return base.GetReader(sql);
        }
        /// <summary>
        /// 摘要：数据库预览表数据
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public override DataTable GetPreviewData(string sql)
        {
            return base.GetPreviewData(sql);
        }
        /// <summary>
        /// 摘要：获取数据库表名（包括视图）
        /// </summary>
        public override DataTable GetTableNames()
        {
            return base.GetTableNames();
        }
        #endregion
    }
}
