using System.Text;
using DatabaseParser.Util;

namespace DatabaseParser.ExpressionParser.Dialect
{
    public class SqlServerQueryFormatter:QueryFormatter
    {
        public SqlServerQueryFormatter():base("@","[","]")
        {
            this.HasPaginationIgnoreOrderBy = true;
        }


        protected override void AddPrefixLimit1()
        {
            _sb.Append("TOP(1) ");
            base.AddPrefixLimit1();
        }


        protected override void BoxPagination(SelectExpression select)
        {
            if (!select.Skip.HasValue && !select.Take.HasValue)
            {
                return;
            }

            var orderByStringBuilder = new StringBuilder();
            if (select.OrderBy.IsNotNullAndNotEmpty() && !HasPaginationIgnoreOrderBy)
            {
                orderByStringBuilder.Append(" ORDER BY ");
                for (var i = 0; i < select.OrderBy.Count; i++)
                {
                    var orderBy = select.OrderBy[i];
                    this.VisitColumn(orderBy.ColumnExpression);
                    orderByStringBuilder.Append(orderBy.OrderByType == OrderByType.Desc ? " DESC" : "");
                    if (i < select.OrderBy.Count - 1)
                    {
                        orderByStringBuilder.Append(",");
                    }
                }
            }

            var tempStringBuilder = new StringBuilder();
            tempStringBuilder.AppendFormat(
                @"select * from (select row_number() over (order by (select 1)) page_rn, page_inner.* FROM (SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]) page_inner
            ) page_outer where page_rn > 5 and page_rn <= 10");

            _sb.Append(" LIMIT ");
            var hasSkip = select.Skip.HasValue;
            if (hasSkip)
            {
                _sb.Append(BoxParameter(select.Skip.Value));
            }
            else
            {
                _sb.Append(BoxParameter(0));
            }

            _sb.Append(",");
            var hasTake = select.Take.HasValue;
            if (hasTake)
            {
                _sb.Append(BoxParameter(select.Take.Value));
            }
            else
            {
                _sb.Append(BoxParameter(int.MaxValue));
            }

            base.BoxPagination(select);
        }
    }
}