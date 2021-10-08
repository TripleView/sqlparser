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
        /// 列前缀，比如DISTINCT
        /// </summary>
        public string ColumnsPrefix { get; set; }
        /// <summary>
        /// 是否只取一条数据
        /// </summary>
        public bool Limit1 { get; set; }
        /// <summary>
        /// 跳过多少数据
        /// </summary>
        public int? Skip { get; set; }
        /// <summary>
        /// 取多少数据
        /// </summary>
        public int? Take { get; set; }
        /// <summary>
        /// 判断是否存在分页
        /// </summary>
        protected bool HasPagination => Skip.HasValue || Take.HasValue;
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