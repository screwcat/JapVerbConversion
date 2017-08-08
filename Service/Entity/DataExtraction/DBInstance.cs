using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Common.Entity.DataExtraction
{
    /// <summary>
    /// 数据库信息表
    /// </summary>
    [TableAttribute("DBInstance")]
    public class DBInstance : EntityBase<DBInstance>
    {
        /// <summary>
        /// 摘要:数据库名称
        /// </summary>
        protected string _Name = string.Empty;
        [PrimaryKey("Name", IsAuto = false)]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// 摘要:驱动
        /// </summary>
        protected string _Provider = string.Empty;
        [Property]
        public string Provider
        {
            get { return _Provider; }
            set { _Provider = value; }
        }
    }
}
