using System;
using System.IO;

namespace SqlParse
{
    class Program
    {
        static void Main(string[] args)
        {
            var sql = @"select * from equipment";
            //sql = "12";
            //sql = "select * from equipment e LEFT JOIN(SELECT * FROM ATL_EQUIPMENT ae) t ON e.EQUIPMENT =t.equipment";

           //var result=new SqlParser(DatabaseType.Oracle).Parse(sql);
            Console.WriteLine("Hello World!");
        }
    }
}
