using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Service.Common.Log;

namespace Service.Common.Entity.DataExtraction
{
    /// <summary>
    /// 日常日志
    /// </summary>
    [TableAttribute("CommonLog")]
    public class CommonLog : EntityBase<CommonLog>
    {
        #region 属性
        /// <summary>
        /// 摘要:唯一标识
        /// </summary>
        protected string _ID = Guid.NewGuid().ToString();
        [PrimaryKey("ID", IsAuto = false)]
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        /// <summary>
        /// 摘要:操作人
        /// </summary>
        protected string _Operator = string.Empty;
        [Property]
        public string Operator
        {
            get { return _Operator; }
            set { _Operator = value; }
        }
        /// <summary>
        /// 摘要:时间
        /// </summary>
        protected string _CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        [Property]
        public string CreateTime
        {
            get { return _CreateTime; }
            set { _CreateTime = value; }
        }
        /// <summary>
        /// 摘要:消息
        /// </summary>
        protected string _Message = string.Empty;
        [Property]
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        #endregion

        #region 构造
        public CommonLog()
        {

        }
        public CommonLog(string stOperator, string message)
        {
            _Operator = stOperator;
            _Message = message;
        }
        #endregion
    }
}
