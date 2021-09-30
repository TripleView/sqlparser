using System;
using System.Collections.Generic;
using System.Text;

namespace SqlParse.DatabaseAdapter
{
    public class DatabaseAdapter:IDatabaseAdapter
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        private readonly DatabaseType databaseType;
        private readonly Dictionary<DatabaseType, AbstractDatabaseAdapter> databaseTypeDic =
            new Dictionary<DatabaseType, AbstractDatabaseAdapter>()
        {
            {DatabaseType.Oracle,new OracleDatabaseAdapter()}
        };
        public DatabaseAdapter(DatabaseType databaseType)
        {
            this.databaseType = databaseType;
        }

        public string ConvertParameterName => databaseTypeDic[databaseType].ConvertParameterName;

        public string ConvertTableNameLeftPart => databaseTypeDic[databaseType].ConvertTableNameLeftPart;

        public string ConvertTableNameRightPart => databaseTypeDic[databaseType].ConvertTableNameRightPart;

        public string ConvertColumnNameLeftPart => databaseTypeDic[databaseType].ConvertColumnNameLeftPart;

        public string ConvertColumnNameRightPart => databaseTypeDic[databaseType].ConvertColumnNameRightPart;

        public List<string> OperatorList => databaseTypeDic[databaseType].OperatorList;

        public List<string> KeyWordList => databaseTypeDic[databaseType].KeyWordList;

        public List<string> FunctionList => databaseTypeDic[databaseType].FunctionList;

        public List<string> SpecialCharactersList => databaseTypeDic[databaseType].SpecialCharactersList;
    }
}
