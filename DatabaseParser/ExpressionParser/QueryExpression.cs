using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DatabaseParser.ExpressionParser
{
    /// <summary>
    /// 代表输出查询的表达式（Select、Table、Join等表达式）
    /// </summary>
    public abstract class QueryExpression : Expression
    {
        protected QueryExpression(ExpressionType expressionType, Type type) : base()
        {
            Type = type;
            NodeType = expressionType;
        }

        public override ExpressionType NodeType { get; }
        public string NodeTypeName => ((DbExpressionType)NodeType).ToString();
        public override Type Type { get; }

        /// <summary>
        /// 查询的别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 查询的所有列表达式
        /// </summary>
        public virtual List<ColumnExpression> Columns { get; set; }

        /// <summary>
        /// 查询的结果类型
        /// </summary>
        public Type ElementType { get; set; }

        /// <summary>
        /// 实际类型
        /// </summary>
        public virtual DbExpressionType ExpressionType { get; set; }

        /// <summary>
        /// 查询的来源
        /// </summary>
        public virtual Expression From { get; set; }

        /// <summary>
        /// 查询表达式的翻译器
        /// </summary>
        public object Translator { get; set; }

        /// <summary>
        /// 扩展(存放翻译器解析表达式时的必要数据)
        /// </summary>
        public object ExData { get; set; }
    }
}