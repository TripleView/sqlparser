using SqlParse;
using Xunit;

namespace TestProject1
{
    public class SqlParserTest
    {
        [Fact]
        public void TestSelect()
        {
            var sql = @"select * from equipment";
            //sql = "12";
            //sql = "select * from equipment e LEFT JOIN(SELECT * FROM ATL_EQUIPMENT ae) t ON e.EQUIPMENT =t.equipment";

            var result = new SqlParser(DatabaseType.Oracle).Parse(sql);
        }
    }
}