using System;
using System.Collections.Generic;
using System.Text;

namespace SqlParse.DatabaseAdapter
{
    public class AbstractDatabaseAdapter : IDatabaseAdapter
    {
        public virtual string ConvertParameterName => "@";

        public virtual string ConvertTableNameLeftPart => "[";

        public virtual string ConvertTableNameRightPart => "]";

        public virtual string ConvertColumnNameLeftPart => "[";

        public virtual string ConvertColumnNameRightPart => "]";

        public virtual List<string> OperatorList => new List<string>() { "=", "~", "-", "+", "*", "/", "<", ">", "!", "&", "|", "^", "%", ":" };

        public virtual List<string> KeyWordList => new List<string>() { "ALTER", "CREATE", "DROP",
            "SELECT", "INSERT", "UPDATE", "DELETE", "MERGE", "TRUNCATE", "DISABLE", "ENABLE",
            "EXECUTE", "BULK", "GRANT", "DENY", "REVOKE", "GO", "ADD", "BEGIN", "COMMIT",
            "ROLLBACK", "DUMP", "BACKUP", "RESTORE", "LOAD", "CHECKPOINT", "WHILE",
            "IF", "BREAK", "CONTINUE", "GOTO", "SET","DECLARE","PRINT","FETCH",
            "OPEN","CLOSE","DEALLOCATE","WITH","DBCC","KILL","MOVE" ,"GET","RECEIVE"
            ,"SEND","WAITFOR","READTEXT","UPDATETEXT","WRITETEXT","USE","SHUTDOWN","RETURN","REVERT"
        };

        public virtual List<string> FunctionList => new List<string>(){};

        public virtual List<string> SpecialCharactersList => new List<string>() { };
    }
}
