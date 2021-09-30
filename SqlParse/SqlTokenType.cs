using System;
using System.Collections.Generic;
using System.Text;

namespace SqlParse
{
    /// <summary>
    /// 词类型
    /// </summary>
    public enum SqlTokenType
    {
        /// <summary>
        /// 关键词
        /// </summary>
        Keywork = 1,
        /// <summary>
        /// 特殊字符，比如".",",","(",")"
        /// </summary>
        SpecialCharacters = 2,
        /// <summary>
        /// 运算符，如=,+等
        /// </summary>
        Operator=3,
        /// <summary>
        /// 标识符，如表名
        /// </summary>
        Identifiers=4,
        /// <summary>
        /// 变量
        /// </summary>
        Variable=5,
        /// <summary>
        /// 函数
        /// </summary>
        Function=6
    }
}
