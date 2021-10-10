using SqlParse;
using SqlParse.Dialect;
using Xunit;

namespace TestProject1
{
    public class SqlParserTest
    {
        [Fact]
        public void TestSelect()
        {
            var sql = @"SELECT DISTINCT `p0`.`Name` As `Key`, SUM(`p0`.`Age`) As `Count` FROM `Person` As `p0` GROUP BY `p0`.`Name` LIMIT @y0,@y1";
        

            var result = new MysqlParser().Parse(sql);
        }
    }
}