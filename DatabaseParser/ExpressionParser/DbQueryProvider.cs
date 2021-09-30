﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace DatabaseParser.ExpressionParser
{
    public class DbQueryProvider : IQueryProvider
    {
        private QueryFormatter queryFormatter;
        public DbQueryProvider()
        {
            this.queryFormatter = new QueryFormatter();
        }
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new BaseRepository<TElement>(expression,this);
        }

        public object Execute(Expression expression)
        {
           
            return null;
        }

        [DebuggerStepThrough]
        public TResult Execute<TResult>(Expression expression)
        {
            //这一步将expression转化成我们自己的expression
            var dbExpressionVisitor = new DbExpressionVisitor();
            var middleResult = dbExpressionVisitor.Visit(expression);
            //将我们自己的expression转换成sql
            queryFormatter.Format(middleResult);
            return default;
        }

        public DbQueryResult GetDbQueryDetail()
        {
            return queryFormatter.GetDbQueryDetail();
        }
    }
}