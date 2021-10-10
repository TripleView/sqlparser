namespace SqlParse.Dialect
{
    public class MysqlParser:SqlParser
    {
        public MysqlParser() : base("@", "`", "`")
        {

        }

    }
}