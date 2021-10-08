namespace DatabaseParser.ExpressionParser.Dialect
{
    public class MysqlQueryFormatter : QueryFormatter
    {
        public MysqlQueryFormatter():base("@","`","`")
        {
            
        }

        protected override void AddSuffixLimit1()
        {
            _sb.Append(" limit 1 ");
            base.AddSuffixLimit1();
        }

        protected override string GetFunctionAlias(string functionName)
        {
            if (functionName == "LEN")
            {
                return "LENGTH";
            }
            return base.GetFunctionAlias(functionName);
        }

        protected override void BoxPagination(SelectExpression select)
        {
            if (!select.Skip.HasValue && !select.Take.HasValue)
            {
                return;
            }
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