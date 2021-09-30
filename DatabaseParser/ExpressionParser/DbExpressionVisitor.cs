using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DatabaseParser.Util;

namespace DatabaseParser.ExpressionParser
{
    public class DbExpressionVisitor : ExpressionVisitor
    {
        private Dictionary<string, ColumnExpression> _lastColumns =
            new Dictionary<string, ColumnExpression>();

        private static readonly IDictionary<ExpressionType, string> nodeTypeMappings = new Dictionary<ExpressionType, string>
        {
            {ExpressionType.Add, "+"},
            {ExpressionType.And, "AND"},
            {ExpressionType.AndAlso, "AND"},
            {ExpressionType.Divide, "/"},
            {ExpressionType.Equal, "="},
            {ExpressionType.ExclusiveOr, "^"},
            {ExpressionType.GreaterThan, ">"},
            {ExpressionType.GreaterThanOrEqual, ">="},
            {ExpressionType.LessThan, "<"},
            {ExpressionType.LessThanOrEqual, "<="},
            {ExpressionType.Modulo, "%"},
            {ExpressionType.Multiply, "*"},
            {ExpressionType.Negate, "-"},
            {ExpressionType.Not, "NOT"},
            {ExpressionType.NotEqual, "<>"},
            {ExpressionType.Or, "OR"},
            {ExpressionType.OrElse, "OR"},
            {ExpressionType.Subtract, "-"}
        };

        /// <summary>
        /// 当前处理的方法名称，比如select，where
        /// </summary>
        private string MethodName;

        #region 表名生成管理

        private int _tableIndex = 0;

        /// <summary>
        /// 获取新的查询别名
        /// </summary>
        public string NewAlias => "p" + _tableIndex;

        #endregion
        [DebuggerStepThrough]
        public override Expression Visit(Expression exp)
        {
            if (exp == null) return null;

            switch ((DbExpressionType)exp.NodeType)
            {
                case DbExpressionType.Select:
                case DbExpressionType.Table:
                case DbExpressionType.Join:
                case DbExpressionType.Query:
                    return this.VisitQuery((QueryExpression)exp);
                case DbExpressionType.Column:
                    return this.VisitColumn((ColumnExpression)exp);
            }

            return base.Visit(exp);
        }


        public virtual Expression VisitQuery(QueryExpression queryExpression)
        {
            return queryExpression;
        }

        public virtual Expression VisitWhere(WhereExpression whereExpression)
        {
            return whereExpression;
        }
        public virtual Expression VisitColumn(ColumnExpression columnExpression)
        {
            return columnExpression;
        }

        public virtual Expression VisitTable(TableExpression tableExpression)
        {
            return tableExpression;
        }

        public virtual Expression VisitSelect(SelectExpression selectExpression)
        {
            return selectExpression;
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = node.Method;
            MethodName = method.Name;

            switch (method.Name)
            {
                case nameof(Queryable.Select):
                    return this.VisitSelectCall(node);
                case nameof(Queryable.Where):
                    return this.VisitWhereCall(node);
            }

            return node;
        }

        /// <summary>
        /// 去除表达式中的参数引用包装
        /// </summary>
        public Expression StripQuotes(Expression e)
        {
            //如果为参数应用表达式
            while (e.NodeType == ExpressionType.Quote)
            {
                //将其转为一元表达式即可获取真正的值
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            if (MethodName == nameof(Queryable.Select))
            {

            }
            else if (MethodName == nameof(Queryable.Where))
            {
                //多个binary连接，每次都只处理最右边。
                var rightExpression = this.Visit(binaryExpression.Right);
                var comparisonCondition = nodeTypeMappings[binaryExpression.NodeType];

                if (string.IsNullOrWhiteSpace(comparisonCondition))
                {
                    throw new NotSupportedException(nameof(binaryExpression.NodeType));
                }

                var leftExpression = this.Visit(binaryExpression.Left);

                if (rightExpression is WhereConditionExpression rightWhereConditionExpression)
                {
                    if (leftExpression is WhereConditionExpression leftExpressionExpression)
                    {
                        rightWhereConditionExpression.NextWhereConditionExpression = leftExpressionExpression;
                        rightWhereConditionExpression.ConnectorToTheNextObject = comparisonCondition;
                        return rightWhereConditionExpression;
                    }
                    //兼容it.isHandsome这种单true的条件
                    else if (leftExpression is ColumnExpression columnExpression && columnExpression.Type == typeof(bool))
                    {
                        var leftWhereConditionExpression = new WhereConditionExpression(columnExpression, "=", 1);
                        rightWhereConditionExpression.NextWhereConditionExpression = leftWhereConditionExpression;
                        rightWhereConditionExpression.ConnectorToTheNextObject = comparisonCondition;

                        return rightWhereConditionExpression;
                    }
                    else
                    {
                        throw new NotSupportedException("binary left process failure");
                    }
                }
                if (rightExpression is ColumnExpression tempRightColumnExpression)
                {
                    var tempRightWhereConditionExpression = new WhereConditionExpression(tempRightColumnExpression, "=", 1);

                    if (leftExpression is WhereConditionExpression leftExpressionExpression)
                    {
                        tempRightWhereConditionExpression.NextWhereConditionExpression = leftExpressionExpression;
                        tempRightWhereConditionExpression.ConnectorToTheNextObject = comparisonCondition;
                        return tempRightWhereConditionExpression;
                    }
                    //兼容it.isHandsome这种单true的条件
                    else if (leftExpression is ColumnExpression columnExpression && columnExpression.Type == typeof(bool))
                    {
                        var leftWhereConditionExpression = new WhereConditionExpression(columnExpression, "=", 1);
                        tempRightWhereConditionExpression.NextWhereConditionExpression = leftWhereConditionExpression;
                        tempRightWhereConditionExpression.ConnectorToTheNextObject = comparisonCondition;

                        return tempRightWhereConditionExpression;
                    }
                    else
                    {
                        throw new NotSupportedException("binary left process failure");
                    }
                }
                else if (leftExpression is ColumnExpression leftColumnExpression && rightExpression is ConstantExpression rightConstantExpression)
                {
                    return new WhereConditionExpression(leftColumnExpression, comparisonCondition,
                        rightConstantExpression.Value);
                }
                else if (rightExpression is ColumnExpression rightColumnExpression && leftExpression is ConstantExpression leftConstantExpression)
                {
                    return new WhereConditionExpression(rightColumnExpression, comparisonCondition,
                        leftConstantExpression.Value);
                }
            }
            //throw new NotSupportedException(MethodName);
            return base.VisitBinary(binaryExpression);
        }

        public virtual Expression VisitWhereCall(MethodCallExpression whereCall)
        {
            var result = new WhereExpression();

            var source = (QueryExpression)this.Visit(whereCall.Arguments[0]);
            var lambda = (LambdaExpression)this.StripQuotes(whereCall.Arguments[1]);
            var bodyExpression = this.Visit(lambda.Body);

            //兼容it.isHandsome这种单true的条件
            if (bodyExpression is ColumnExpression columnExpression && columnExpression.Type == typeof(bool))
            {
                var whereConditionExpression = new WhereConditionExpression(columnExpression, "=", 1);
                result.WhereConditionExpressions = whereConditionExpression;
            }
            else if (bodyExpression is WhereConditionExpression whereConditionExpression)
            {
                result.WhereConditionExpressions = whereConditionExpression;
            }
            else
            {
                throw new NotSupportedException(nameof(bodyExpression));
            }
            //将结果集做一下倒叙；
            var whereList = new List<WhereConditionExpression>();
            var lastWhereConditionExpression = result.WhereConditionExpressions;
            whereList.Add(lastWhereConditionExpression);

            while (lastWhereConditionExpression.NextWhereConditionExpression != null)
            {
                var tempWhereConditionExpression = lastWhereConditionExpression.NextWhereConditionExpression;
                whereList.Add(tempWhereConditionExpression);
                lastWhereConditionExpression = lastWhereConditionExpression.NextWhereConditionExpression;
            }

            whereList.Reverse();
            result.WhereConditionExpressions = null;
            for (int i = 0; i < whereList.Count; i++)
            {
                var whereConditionExpression = whereList[i];
                whereConditionExpression.NextWhereConditionExpression = null;
                if (i + 1 <= whereList.Count - 1)
                {
                    whereConditionExpression.ConnectorToTheNextObject = whereList[i + 1].ConnectorToTheNextObject;
                }
                else
                {
                    whereConditionExpression.ConnectorToTheNextObject = "";
                }

                if (i == 0)
                {
                    result.WhereConditionExpressions = whereConditionExpression;
                }
                else
                {
                    var tempWhereConditionExpression = result.WhereConditionExpressions;
                    if (tempWhereConditionExpression == null)
                    {
                        throw new Exception("reverse failure");
                    }
                    while (tempWhereConditionExpression.NextWhereConditionExpression != null)
                    {
                        tempWhereConditionExpression = tempWhereConditionExpression.NextWhereConditionExpression;
                    }

                    tempWhereConditionExpression.NextWhereConditionExpression = whereConditionExpression;
                }
            }

            result.From = source;

            return result;
        }
        public virtual Expression VisitSelectCall(MethodCallExpression selectCall)
        {
            var source = (QueryExpression)this.Visit(selectCall.Arguments[0]);
            var lambda = (LambdaExpression)this.StripQuotes(selectCall.Arguments[1]);
            var bodyExpression = this.Visit(lambda.Body);
            if (bodyExpression != null)
            {
                switch (bodyExpression)
                {
                    case SelectExpression selectExpression:
                        selectExpression.From = source;
                        return selectExpression;
                        break;
                    case MemberExpression memberExpression:

                        break;
                }

            }


            return selectCall;
        }

        protected override Expression VisitUnary(UnaryExpression unaryExpression)
        {
            var operatorString = nodeTypeMappings[unaryExpression.NodeType];

            var operand = unaryExpression.Operand;
            var middleResult = this.Visit(operand);
            if (middleResult is ColumnExpression columnExpression && columnExpression.Type == typeof(bool))
            {
                var result = new WhereConditionExpression(columnExpression, "=", 0, operatorString);
                return result;
            }
            else
            {
                throw new NotSupportedException(nameof(unaryExpression));
            }

            return base.VisitUnary(unaryExpression);
        }

        protected override Expression VisitConstant(ConstantExpression constant)
        {
            if (constant.Value is IQueryable queryable)
            {
                //查找tableAttribute特性,看下有没有自定义表明
                var table = (TableAttribute)queryable.ElementType.GetCustomAttribute(typeof(TableAttribute), false);
                //如果没有该特性，直接使用类名作为表名
                var tableName = table == null ? queryable.ElementType.Name : table.Name;
                var alias = "";
                if (MethodName == nameof(Queryable.Where))
                {
                    alias = this.NewAlias;
                }

                //生成TableExpression,并将其Columns属性缓存
                var tableExpression = new TableExpression(queryable.ElementType, alias, tableName);
                _lastColumns = tableExpression.Columns.ToDictionary(x => x.ColumnName);

                return tableExpression;
            }

            return base.VisitConstant(constant);
        }

        protected override Expression VisitParameter(ParameterExpression param)
        {

            //如果缓存中没有任何列
            if (_lastColumns.Count == 0) return base.VisitParameter(param);

            var alias = this.NewAlias;

            //根据_lastColumns中生成newColumns,Value = Expression.Constant(oldColumn)也就是对oldColumn的一个引用
            var newColumns = _lastColumns.Values.Select(oldColumn =>
                new ColumnExpression(oldColumn.Type,

                    alias,
                    oldColumn.MemberInfo,
                    oldColumn.Index)).ToList();

            //将生成的新列赋值给缓存
            _lastColumns = newColumns.ToDictionary(x => x.ColumnName);

            return new SelectExpression(param.Type, alias, newColumns, null);

        }

        private object GetValue(Expression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            if (MethodName == nameof(Queryable.Select))
            {
                //如果是可以直接获取值得
                if (memberExpression.Expression is MemberExpression parentExpression)
                {
                    var value = GetValue(memberExpression);
                    return Expression.Constant(value);
                }
                else
                {
                    //如果缓存中没有任何列
                    if (_lastColumns.Count == 0) return base.VisitMember(memberExpression);
                    var propertyInfo = memberExpression.Member;
                    var columnName = DbQueryUtil.GetColumnName(propertyInfo);

                    var alias = this.NewAlias;

                    //根据_lastColumns中生成newColumns,Value = Expression.Constant(oldColumn)也就是对oldColumn的一个引用
                    var newColumns = _lastColumns.Values.Where(it => it.ColumnName == columnName).Select(oldColumn =>
                        new ColumnExpression(oldColumn.Type,

                            alias,
                            oldColumn.MemberInfo,
                            oldColumn.Index)).ToList();

                    return new SelectExpression(memberExpression.Expression.Type, alias, newColumns, null);
                }
            }
            else if (MethodName == nameof(Queryable.Where))
            {
                //如果是可以直接获取值得
                if (memberExpression.Expression is MemberExpression parentExpression)
                {
                    //兼容it.name.length>5
                    if (memberExpression.Member is PropertyInfo propertyInfo && propertyInfo.GetMethod?.Name == "get_Length")
                    {
                        //获取所有列
                        var middlExpression = this.Visit(parentExpression);
                        if (middlExpression is ColumnExpression column)
                        {
                            column.FunctionName = "LEN";
                            return column;
                        }
                        else
                        {
                            throw new NotSupportedException(nameof(middlExpression));
                        }
                    }
                    else
                    {
                        var value = GetValue(memberExpression);
                        return Expression.Constant(value);
                    }

                }
                //如果是it.name这种形式
                else if (memberExpression.Expression is ParameterExpression parameterExpression)
                {
                    //获取所有列
                    this.VisitParameter(parameterExpression);
                    //找到要获取的那一列
                    var column = _lastColumns.Values.FirstOrDefault(it => it.MemberInfo == memberExpression.Member);
                    if (column == null)
                    {
                        throw new NotSupportedException(memberExpression.Member.Name);
                    }
                    return column;
                }
                //如果是constant
                else if (memberExpression.Expression is ConstantExpression constantExpression)
                {
                    var value = GetValue(memberExpression);
                    return Expression.Constant(value);
                    //return constantExpression;
                }
                else
                {
                    var result = new ColumnExpression(null, "", null, 0);
                }
            }

            throw new NotSupportedException(this.MethodName);

        }

        protected override Expression VisitNew(NewExpression newExpression)
        {
            var newColumns = new List<ColumnExpression>();
            SelectExpression result = null;
            for (int i = 0; i < newExpression.Members.Count; i++)
            {

                var memberInfo = newExpression.Members[i];
                if (newExpression.Arguments[i] is MemberExpression memberExpression)
                {
                    var middleResult = this.Visit(memberExpression);
                    if (middleResult == null)
                    {
                        continue;
                    }
                    if (middleResult is SelectExpression selectExpression)
                    {
                        result ??= selectExpression;
                    }
                    else if (middleResult is ConstantExpression constantExpression)
                    {
                        var value = constantExpression.Value;
                        var newColumn = new ColumnExpression(constantExpression.Type, "", memberInfo, i, value);
                        newColumns.Add(newColumn);
                    }
                }
                else if (newExpression.Arguments[i] is ConstantExpression constantExpression)
                {
                    var value = constantExpression.Value;
                    var newColumn = new ColumnExpression(constantExpression.Type, "", memberInfo, i, value);
                    newColumns.Add(newColumn);
                }

            }

            if (result != null)
            {
                result.Columns.AddRange(newColumns);
            }
            else
            {
                result = new SelectExpression(null, "", newColumns, null);
            }

            return result;
        }

    }
}