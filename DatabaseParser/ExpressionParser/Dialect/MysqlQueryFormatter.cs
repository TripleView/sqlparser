namespace DatabaseParser.ExpressionParser.Dialect
{
    public class MysqlQueryFormatter : QueryFormatter
    {
        public MysqlQueryFormatter():base("@","`","`")
        {
            
        }

        protected override string GetFunctionAlias(string functionName)
        {
            if (functionName == "LEN")
            {
                return "LENGTH";
            }
            return base.GetFunctionAlias(functionName);
        }
    }
}