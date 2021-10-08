using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using DatabaseParser.Util;

namespace DatabaseParser.ExpressionParser
{
    /// <summary>
    /// 列表达式
    /// </summary>
    public class ColumnExpression : Expression
    {

        public ColumnExpression(Type type, string tableAlias, MemberInfo memberInfo, int index) : base()
        {
            TableAlias = tableAlias;
            Index = index;
            this.Type = type;
            this.NodeType = (ExpressionType)DbExpressionType.Column;
            this.MemberInfo = memberInfo;
        }

        
        public ColumnExpression(Type type, string TableAlias, MemberInfo memberInfo, int index, object value,string functionName) : this(type, TableAlias, memberInfo, index)
        {
            this.Value = value;
            this.FunctionName = functionName;
        }

        public ColumnExpression(Type type, string TableAlias, MemberInfo memberInfo, int index, object value) : this(type, TableAlias, memberInfo, index)
        {
            this.Value = value;
        }

        public ColumnExpression(Type type, string TableAlias, MemberInfo memberInfo, int index, string columnAlias) : this(type, TableAlias, memberInfo, index)
        {
            this.ColumnAlias = columnAlias;
        }

        public ColumnExpression(Type type, string TableAlias, MemberInfo memberInfo, int index, string columnAlias, string functionName) : this(type, TableAlias, memberInfo, index)
        {
            this.ColumnAlias = columnAlias;
            this.FunctionName = functionName;
        }

        public override ExpressionType NodeType { get; }
        public override Type Type { get; }


        public string NodeTypeName => ((DbExpressionType)NodeType).ToString();
        /// <summary>
        /// 元信息
        /// </summary>
        public MemberInfo MemberInfo { get;}
        /// <summary>
        /// 包围列的函数，len
        /// </summary>
        public string FunctionName { get; set; }
        #region 属性
        /// <summary>
        /// 固定值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 判断是否为主键
        /// </summary>
        public bool IsKey
        {
            get
            {
                var keyAttribute = MemberInfo.GetCustomAttribute<KeyAttribute>();
                return keyAttribute != null;
            }
        }

        /// <summary>
        /// 判断是否为可空类型
        /// </summary>
        public bool IsNullable => this.MemberInfo.IsNullable();

        /// <summary>
        /// 表的别名
        /// </summary>
        public string TableAlias { get; set; }
        /// <summary>
        /// 列的别名
        /// </summary>
        public string ColumnAlias { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName => DbQueryUtil.GetColumnName(MemberInfo);

        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }

        #endregion
    }
}