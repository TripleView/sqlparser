using System;
using System.Collections.Generic;
using System.Text;

namespace SqlParse.DatabaseAdapter
{
    /// <summary>
    /// 数据库配置适配器
    /// </summary>
    public interface IDatabaseAdapter
    {
        /// <summary>
        /// 转化参数
        /// </summary>
        string ConvertParameterName { get; }
        /// <summary>
        /// 转化表名左边部分
        /// </summary>
        string ConvertTableNameLeftPart { get; }
        /// <summary>
        /// 转化表名右边部分
        /// </summary>
        string ConvertTableNameRightPart { get; }
        /// <summary>
        /// 转化列名左边部分
        /// </summary>
        string ConvertColumnNameLeftPart { get; }
        /// <summary>
        /// 转化列名左边部分
        /// </summary>
        string ConvertColumnNameRightPart { get; }
        /// <summary>
        /// 操作符列表
        /// </summary>
        List<string> OperatorList { get; }
        /// <summary>
        /// 数据库关键字列表
        /// </summary>
        List<string> KeyWordList { get; }
        /// <summary>
        /// 函数列表
        /// </summary>
        List<string> FunctionList { get; }
        /// <summary>
        /// 特殊符号列表
        /// </summary>
        List<string> SpecialCharactersList { get; }
    }
}
