using System.Linq.Expressions;

namespace DatabaseParser.ExpressionParser
{
    /// <summary>
    /// where表达式里的条件表达式，比如a=2,c>3等等
    /// </summary>
    public class WhereConditionExpression : Expression
    {
        public WhereConditionExpression(ColumnExpression columnExpression, string comparisonCondition, object value)
        {
            ColumnExpression = columnExpression;
            ComparisonCondition = comparisonCondition;
            Value = value;
            NodeType= (ExpressionType)DbExpressionType.WhereCondition;
        }

        public WhereConditionExpression(ColumnExpression columnExpression, string comparisonCondition, object value,string functionName):this(columnExpression, comparisonCondition, value)
        {
            this.FunctionName = functionName;
        }
        public override ExpressionType NodeType { get; }
        /// <summary>
        /// 列
        /// </summary>
        public ColumnExpression ColumnExpression { get; set; }
        /// <summary>
        /// 比较条件，比如=,>等
        /// </summary>
        public string ComparisonCondition { get; set; }
        /// <summary>
        /// 具体的值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 与下一个条件的连接符
        /// </summary>
        public string ConnectorToTheNextObject { get; set; }
        /// <summary>
        /// 下一个where连接条件
        /// </summary>
        public WhereConditionExpression NextWhereConditionExpression { get; set; }
        /// <summary>
        /// 包围条件的函数，比如not
        /// </summary>
        public string FunctionName { get; set; }
    }
}