using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Common.Log
{
    /// <summary>
    /// 操作执行返回信息
    /// </summary>
    [Serializable]
    public class ReturnMessage
    {
        /// <summary>
        /// 摘要：操作是否成功
        /// </summary>
        bool _IsSucessed = true;
        public bool IsSucessed
        {
            get { return _IsSucessed; }
            set { _IsSucessed = value; }
        }
        /// <summary>
        /// 摘要：操作返回详细信息
        /// </summary>
        string _Message=string.Empty;
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
        /// <summary>
        /// 摘要：构造
        /// </summary>
        public ReturnMessage()
        { }
        public ReturnMessage(bool isSucessed)
        {
            _IsSucessed = isSucessed;
        }
        public ReturnMessage(bool isSucessed, string message)
        {
            _IsSucessed = isSucessed;
            _Message = message;
        }
    }
}
