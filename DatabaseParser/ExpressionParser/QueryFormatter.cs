using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using DatabaseParser.Util;

namespace DatabaseParser.ExpressionParser
{
    public class QueryFormatter : DbExpressionVisitor
    {

        private readonly StringBuilder _sb = new StringBuilder();
        private readonly List<SqlParameter> sqlParameters = new List<SqlParameter>();
        private int parameterIndex = 0;
        private string parameterPrefix = "@";
        public override Expression VisitTable(TableExpression table)
        {
            _sb.Append("SELECT ");

            int index = 0;
            foreach (var column in table.Columns)
            {
                if (index++ > 0) _sb.Append(", ");
                this.VisitColumn(column);
            }
            _sb.AppendFormat(" FROM [{0}]", table.Name);
            if (!table.Alias.IsNullOrWhiteSpace())
                _sb.AppendFormat(" As [{0}] ", table.Alias);

            return table;
        }


        public void Format(Expression expression)
        {
            _sb.Clear();
            if (expression is QueryExpression queryExpression)
            {
                this.VisitQuery(queryExpression);
            }
            else if (expression is WhereExpression whereExpression)
            {
                this.VisitWhere(whereExpression);
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
        private string BoxParameter(object obj, bool returnRealValue = false)
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
            //如果有固定值
            var value = columnExpression.Value;
            if (value != null)
            {

                tempStringBuilder.AppendFormat("{0} As [{1}]", BoxParameter(value, true), columnExpression.ColumnName);
            }
            else
            {
                if (!columnExpression.TableAlias.IsNullOrWhiteSpace())
                {
                    tempStringBuilder.AppendFormat("[{0}].[{1}]", columnExpression.TableAlias, columnExpression.ColumnName);
                }
                else
                {
                    tempStringBuilder.AppendFormat("[{0}]", columnExpression.ColumnName);
                }
            }

            if (!columnExpression.FunctionName.IsNullOrWhiteSpace())
            {
                tempStringBuilder.Insert(0, columnExpression.FunctionName + "(");
                tempStringBuilder.Insert(tempStringBuilder.Length, ")");
            }

            _sb.Append(tempStringBuilder);

            return columnExpression;
        }

        public override Expression VisitSelect(SelectExpression select)
        {
            _sb.Append("SELECT ");
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
                _sb.Append("(");
                this.Visit(select.From);
                _sb.Append(")");
            }
            _sb.AppendFormat(" As {0} ", alias);

            return select;
        }

        public override Expression VisitWhere(WhereExpression whereExpression)
        {
            if (whereExpression.From != null)
            {
                this.Visit(whereExpression.From);
                _sb.Append(" ");
            }

            _sb.Append("WHERE ");
            int index = 0;

            var whereConditionExpression = whereExpression.WhereConditionExpressions;
            if (whereConditionExpression != null)
            {
                this.VisitColumn(whereConditionExpression.ColumnExpression);
                _sb.Append(" ");
                _sb.Append(whereConditionExpression.ComparisonCondition);
                _sb.Append(" ");
                _sb.Append(BoxParameter(whereConditionExpression.Value));
                while (whereConditionExpression.NextWhereConditionExpression != null)
                {
                    _sb.Append(" ");
                    _sb.Append(whereConditionExpression.ConnectorToTheNextObject);
                    _sb.Append(" ");

                    whereConditionExpression = whereConditionExpression.NextWhereConditionExpression;
                    this.VisitColumn(whereConditionExpression.ColumnExpression);
                    _sb.Append(" ");
                    _sb.Append(whereConditionExpression.ComparisonCondition);
                    _sb.Append(" ");
                    _sb.Append(BoxParameter(whereConditionExpression.Value));
                }
            }
            else
            {
                throw new Exception("can not find where condition");
            }

            return whereExpression;
        }
    }
}