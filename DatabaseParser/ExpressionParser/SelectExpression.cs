using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DatabaseParser.ExpressionParser
{
    /// <summary>
    /// Select 表达式
    /// </summary>
    public class SelectExpression : QueryExpression
    {
        public SelectExpression(Type type, string alias, List<ColumnExpression> columns, Expression from, WhereExpression where = null,
            List<GroupByExpression> groupBy = null, List<OrderByExpression> orderBy = null, object translator = null)
            : base((ExpressionType)DbExpressionType.Select, type)
        {
            ElementType = type;
            Alias = alias;
            Columns = columns;
            From = from;
            Where = where;
            GroupBy = groupBy ?? new List<GroupByExpression>();
            OrderBy = orderBy ?? new List<OrderByExpression>();
            Translator = translator;
        }

        #region 属性

        /// <summary>
        /// Where条件
        /// </summary>
        public WhereExpression Where { get; set; }

        /// <summary>
        /// GroupBy
        /// </summary>
        public List<GroupByExpression> GroupBy { get; set; }

        /// <summary>
        /// OrderBy
        /// </summary>
        public List<OrderByExpression> OrderBy { get; set; }

        #endregion


    }
}