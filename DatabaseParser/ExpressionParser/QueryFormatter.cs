using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using DatabaseParser.Base;
using DatabaseParser.Util;

namespace DatabaseParser.ExpressionParser
{
    public class QueryFormatter : DbExpressionVisitor
    {

        public QueryFormatter(string parameterPrefix, string leftQuote, string rightQuote)
        {
            this.parameterPrefix = parameterPrefix;
            this.leftQuote = leftQuote;
            this.rightQuote = rightQuote;
        }

        protected readonly StringBuilder _sb = new StringBuilder();

        private readonly List<SqlParameter> sqlParameters = new List<SqlParameter>();

        private int parameterIndex = 0;
        private string parameterPrefix;
        private string leftQuote;
        private string rightQuote;

        /// <summary>
        /// 如果存在分页，则跳过orderby语句
        /// </summary>
        protected bool HasPaginationIgnoreOrderBy { set; get; } = false;

        public override Expression VisitTable(TableExpression table)
        {
            _sb.Append("SELECT ");

            int index = 0;
            foreach (var column in table.Columns)
            {
                if (index++ > 0) _sb.Append(", ");
                this.VisitColumn(column);
            }
            _sb.AppendFormat(" FROM {0}", BoxTableNameOrColumnName(table.Name));
            if (!table.Alias.IsNullOrWhiteSpace())
                _sb.AppendFormat(" As {0} ", BoxTableNameOrColumnName(table.Alias));

            return table;
        }
        /// <summary>
        /// 包装表名或者列名
        /// </summary>
        /// <param name="tableNameOrColumnName"></param>
        /// <returns></returns>

        private string BoxTableNameOrColumnName(string tableNameOrColumnName)
        {
            if (tableNameOrColumnName == "*")
            {
                return tableNameOrColumnName;
            }

            return leftQuote + tableNameOrColumnName + rightQuote;
        }


        /// <summary>
        /// 获取函数别名，比如sqlserver就是LEN，mysql就是LENGTH
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        protected virtual string GetFunctionAlias(string functionName)
        {
            return functionName;
        }

        public void Format(Expression expression)
        {
            _sb.Clear();
            if (expression is SelectExpression selectExpression)
            {
                var result = this.VisitSelect(selectExpression);
            }
            else
            {
                throw new NotSupportedException(nameof(expression));
            }


        }

        public DbQueryResult GetDbQueryDetail()
        {
            return new DbQueryResult()
            {
                Sql = this._sb.ToString().Trim(),
                SqlParameters = this.sqlParameters
            };
        }

        public override Expression VisitQuery(QueryExpression queryExpression)
        {
            switch (queryExpression)
            {
                case SelectExpression selectExpression: this.VisitSelect(selectExpression); break;
                case TableExpression tableExpression: this.VisitTable(tableExpression); break;
                    //case DbExpressionType.Join: this.VisitSource((JoinExpression)exp); break;
            }

            return queryExpression;
        }

        /// <summary>
        /// 包装参数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="returnRealValue">是否返回实际值</param>
        /// <returns></returns>
        protected string BoxParameter(object obj, bool returnRealValue = false)
        {
            var value = obj.ToString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }
            if (char.IsNumber(value, 0))
            {
                value = value;
            }
            else
            {
                value = $"'{value}'";
            }

            if (returnRealValue)
            {
                return value;
            }

            var finalValue = obj;
            if (obj is bool objBool)
            {
                finalValue = objBool ? 1 : 0;
            }

            var parameterName = this.parameterPrefix + "y" + parameterIndex;
            var sqlParameter = new SqlParameter()
            {
                ParameterName = parameterName,
                ParameterType = obj.GetType(),
                Value = finalValue
            };
            this.sqlParameters.Add(sqlParameter);
            parameterIndex++;

            return parameterName;
        }

        public override Expression VisitColumn(ColumnExpression columnExpression)
        {
            var tempStringBuilder = new StringBuilder();
            //如果有固定值,比如'福建' as address
            var value = columnExpression.Value;
            if (value != null)
            {
                tempStringBuilder.AppendFormat("{0} As {1}", BoxParameter(value, true), BoxTableNameOrColumnName(columnExpression.ColumnName));
            }
            else
            {
                if (!columnExpression.TableAlias.IsNullOrWhiteSpace())
                {
                    tempStringBuilder.AppendFormat("{0}.{1}", BoxTableNameOrColumnName(columnExpression.TableAlias), BoxTableNameOrColumnName(columnExpression.ColumnName));
                }
                else
                {
                    tempStringBuilder.AppendFormat("{0}", BoxTableNameOrColumnName(columnExpression.ColumnName));
                }
            }

            if (!columnExpression.FunctionName.IsNullOrWhiteSpace())
            {
                tempStringBuilder.Insert(0, this.GetFunctionAlias(columnExpression.FunctionName) + "(");
                tempStringBuilder.Insert(tempStringBuilder.Length, ")");
            }

            //添加列别名
            if (!columnExpression.ColumnAlias.IsNullOrWhiteSpace())
            {
                tempStringBuilder.AppendFormat(" As {0}", BoxTableNameOrColumnName(columnExpression.ColumnAlias));
            }

            _sb.Append(tempStringBuilder);

            return columnExpression;
        }

        /// <summary>
        /// 添加前缀限制返回一条数据
        /// </summary>
        protected virtual void AddPrefixLimit1()
        {

        }

        /// <summary>
        /// 添加后缀限制返回一条数据
        /// </summary>
        protected virtual void AddSuffixLimit1()
        {

        }

        /// <summary>
        /// 处理分页
        /// </summary>
        protected virtual void BoxPagination(SelectExpression select)
        {

        }

        public override Expression VisitSelect(SelectExpression select)
        {
            _sb.Append("SELECT ");

            if (!select.ColumnsPrefix.IsNullOrWhiteSpace())
            {
                _sb.AppendFormat("{0} ", select.ColumnsPrefix);
            }

            if (select.Limit1)
            {
                this.AddPrefixLimit1();
            }

            int index = 0;
            foreach (var column in select.Columns)
            {
                if (index++ > 0) _sb.Append(", ");
                this.VisitColumn(column);
            }

            var alias = select.Alias;

            if (select.From != null)
            {
                _sb.Append(" FROM ");
                //_sb.Append("(");
                if (select.From is TableExpression table)
                {
                    _sb.Append(BoxTableNameOrColumnName(table.Name));
                    if (!table.Alias.IsNullOrWhiteSpace())
                    {
                        _sb.AppendFormat(" As {0}", BoxTableNameOrColumnName(table.Alias));
                    }
                }

            }
            else
            {
                throw new ArgumentException("loss from");
            }

            if (select.Where != null)
            {
                _sb.Append(" WHERE ");
                this.VisitWhere(select.Where);
            }

            if (select.GroupBy.IsNotNullAndNotEmpty())
            {
                _sb.Append(" GROUP BY ");
                for (var i = 0; i < select.GroupBy.Count; i++)
                {
                    var groupBy = select.GroupBy[i];
                    this.VisitColumn(groupBy.ColumnExpression);
                    if (i < select.GroupBy.Count - 1)
                    {
                        _sb.Append(",");
                    }
                }
            }

            if (select.OrderBy.IsNotNullAndNotEmpty()&&!HasPaginationIgnoreOrderBy)
            {
                _sb.Append(" ORDER BY ");
                for (var i = 0; i < select.OrderBy.Count; i++)
                {
                    var orderBy = select.OrderBy[i];
                    this.VisitColumn(orderBy.ColumnExpression);
                    _sb.Append(orderBy.OrderByType == OrderByType.Desc ? " DESC" : "");
                    if (i < select.OrderBy.Count - 1)
                    {
                        _sb.Append(",");
                    }
                }
            }

            if (select.Limit1)
            {
                this.AddSuffixLimit1();
            }

            BoxPagination(select);

            return select;
        }

        public override Expression VisitWhere(WhereExpression whereExpression)
        {

            int index = 0;
            _sb.Append(" (");
            if (whereExpression.Left != null && whereExpression.Right != null)
            {
                this.VisitWhere(whereExpression.Left);
                _sb.Append(" ");
                _sb.Append(whereExpression.Operator);
                _sb.Append(" ");
                this.VisitWhere(whereExpression.Right);
                _sb.Append(" ");
            }

            else if (whereExpression is WhereConditionExpression whereConditionExpression)
            {
                this.VisitColumn(whereConditionExpression.ColumnExpression);
                _sb.Append(" ");
                _sb.Append(whereConditionExpression.Operator);
                _sb.Append(" ");
                _sb.Append(BoxParameter(whereConditionExpression.Value));
            }
            else if (whereExpression is FunctionWhereConditionExpression functionWhereConditionExpression)
            {
                _sb.Append(GetFunctionAlias(functionWhereConditionExpression.Operator) + " ");
                _sb.Append(" (");
                this.VisitWhere(functionWhereConditionExpression.WhereExpression);
                _sb.Append(" )");
            }

            _sb.Append(" )");


            return whereExpression;
        }
    }
}