using System;
using System.Collections.Generic;
using System.Text;

namespace SqlParse.DatabaseAdapter
{
    public class OracleDatabaseAdapter:AbstractDatabaseAdapter
    {
        /// <summary>
        /// SELECT  listagg(keyword, '","') within group (order by  keyword) FROM v$reserved_words m WHERE m.RESERVED='Y'
        /// </summary>
        public override List<string> KeyWordList => new List<string>()
        {
            "!","&","*",":","@",
            "ALL","ALTER","AND","ANY","AS","ASC","BETWEEN","BY","CHAR",
            "CHECK","CLUSTER","COMPRESS","CONNECT","CREATE","DATE",
            "DECIMAL","DEFAULT","DELETE","DESC","DISTINCT","DROP","ELSE",
            "EXCLUSIVE","EXISTS","FLOAT","FOR","FROM","GRANT","GROUP",
            "HAVING","IDENTIFIED","IN","INDEX","INSERT","INTEGER","INTERSECT",
            "INTO","IS","LIKE","LOCK","LONG","MINUS","MODE","NOCOMPRESS","NOT",
            "NOWAIT","NULL","NUMBER","OF","ON","OPTION","OR","ORDER","PCTFREE",
            "PRIOR","PUBLIC","RAW","RENAME","RESOURCE","REVOKE","SELECT","SET",
            "SHARE","SIZE","SMALLINT","START","SYNONYM","TABLE","THEN","TO",
            "TRIGGER","UNION","UNIQUE","UPDATE","VALUES","VARCHAR","VARCHAR2",
            "VIEW","WHERE","WITH","[","]","^"
        };

        public override List<string> OperatorList =>new List<string>()
        {
            "=", "~", "-", "+", "*", "/", "<", ">", "!", "&", "|", "^", "%", ":"
        };

        public override List<string> FunctionList => new List<string>()
        {
            //数值函数
            "CEIL","FLOOR","MOD","POWER","ROUND","SIGN","SQRT",
            //
            "INITICAP","LOWER","REPLACE","SUBSTR","LENGTH","||",
            //
            "SYSDATE","LAST_DAY","ADD_MONTHS","MONTHS_BETWEEN","NEXT_DAY"
            ,"TO_NUMBER","TO_CHAR","CONCAT","INITCAP","LOWER","LPAD","LTRIM"
            ,"REPLACE","RPAD","RTRIM","SOUNDEX","SUBSTR","TRANSLATE","UPPER","ASCII",
            "INSTR","LENGTH","COALESCE","LNNVL","NVL","AVG","MAX","SUM","COUNT"
        };

        public override List<string> SpecialCharactersList => new List<string>()
        {
            ",",";","(",")","."
        };
    }
}
