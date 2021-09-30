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
        public SelectExpression(Type type, string alias, List<ColumnExpression> columns, Expression from, Expression where = null,
            IEnumerable<ColumnExpression> groupBy = null, IEnumerable<Expression> orderBy = null, object translator = null)
            : base((ExpressionType)DbExpressionType.Select, type)
        {
            ElementType = type;
            Alias = alias;
            Columns = columns;
            From = from;
            Where = where;
            GroupBy = groupBy;
            OrderBy = orderBy;
            Translator = translator;
         
        }

        #region 属性

        /// <summary>
        /// Where条件
        /// </summary>
        public Expression Where { get; set; }

        /// <summary>
        /// GroupBy
        /// </summary>
        public IEnumerable<ColumnExpression> GroupBy { get; set; }

        /// <summary>
        /// OrderBy
        /// </summary>
        public IEnumerable<Expression> OrderBy { get; set; }

        #endregion

       
    }
}