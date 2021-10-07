namespace DatabaseParser.ExpressionParser.Dialect
{
    public class SqlServerQueryFormatter:QueryFormatter
    {
        public SqlServerQueryFormatter():base("@","[","]")
        {
            
        }

    }
}