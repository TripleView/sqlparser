using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DatabaseParser.ExpressionParser
{
    /// <summary>
    /// where表达式
    /// </summary>
    public class WhereExpression : Expression
    {
        public WhereExpression()
        {
            this.NodeType = (ExpressionType)DbExpressionType.Where;
        }

        public override ExpressionType NodeType { get; }
        public override Type Type { get; }
        /// <summary>
        /// 更新的来源
        /// </summary>
        public virtual Expression From { get; set; }
        /// <summary>
        /// where的子条件,是一个链表的形式
        /// </summary>
        public WhereConditionExpression WhereConditionExpressions { get; set; }
    }
}