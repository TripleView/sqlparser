﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DatabaseParser.Base;
using DatabaseParser.Util;

namespace DatabaseParser.ExpressionParser
{
    /// <summary>
    /// 代表一张表的表达式
    /// </summary>
    public class TableExpression : QueryExpression
    {
        /// <summary>
        /// 代表一张表的表达式
        /// </summary>
        /// <param name="type">表内元素的类型(对应实体类)</param>
        /// <param name="alias">表的别名</param>
        /// <param name="name">表的名称</param>
        public TableExpression(Type type, string alias)
            : base((ExpressionType)DbExpressionType.Table, type)
        {
            ElementType = type;
            Alias = alias;
        }

        /// <summary>
        /// 表的名称
        /// </summary>
        public string Name 
        {
            get
            {
                //查找tableAttribute特性,看下有没有自定义表明
                var tableAttribute =ElementType.GetCustomAttribute<TableAttribute>();
                //如果没有该特性，直接使用类名作为表名
                var tableName = tableAttribute == null ? ElementType.Name : tableAttribute.Name;
                return tableName;
            }

        }



        /// <summary>
        /// 表的列
        /// </summary>
        public override List<ColumnExpression> Columns
        {
            get
            {
                if (_columns == null)
                {
                    int i = 0;
                    _columns = new List<ColumnExpression>();
                 
                    //排除掉不映射的列
                    var properties = Type.GetProperties().Where(it => !it.GetCustomAttributes().OfType<NotMappedAttribute>().Any()).ToList();

                    var obj = Expression.Constant(Activator.CreateInstance(Type));

                    foreach (var propertyInfo in properties)
                    {
                        _columns.Add(new ColumnExpression(propertyInfo.PropertyType,
                             Alias, propertyInfo, i++));
                    }
                    
                }

                return _columns;
            }
        }

        private List<ColumnExpression> _columns;
    }
}