using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Common.DB.Core
{
    /// <summary>
    /// 表达式格式化
    /// </summary>
    public class ExpressFormat
    {
        /// <summary>
        /// 摘要：格式化字符串添加圆括号
        /// </summary>
        /// <param name="formatString">待格式化的字符串</param>
        /// <returns>格式化后的字符串</returns>
        public static string AddParentheses(string formatString)
        {
            return String.Format("({0})", formatString);
        }
        /// <summary>
        /// 摘要：格式化字符串添加单引号
        /// </summary>
        /// <param name="formatString">待格式化的字符串</param>
        /// <returns>格式化后的字符串</returns>
        public static string AddSingleQuotationMarks(string formatString)
        {
            return String.Format("'{0}'", formatString);
        }
    }
}
