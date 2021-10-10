using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SqlParse
{
    public class SqlParser 
    {
        public SqlParser(string parameterPrefix, string leftQuote, string rightQuote)
        {
            this.parameterPrefix = parameterPrefix;
            this.leftQuote = leftQuote;
            this.rightQuote = rightQuote;
        }

        private string parameterPrefix;
        private string leftQuote;
        private string rightQuote;
        /// <summary>
        /// 切割单词的分隔符，主要是空格
        /// </summary>
        public List<char> SeparatorList { get; set; } = new List<char>() { ' ',','};

        public List<SqlToken> Parse(string sql)
        {
            var result = new List<SqlToken>();

            var reader = new StringReader(sql);
            //当前指针
            var i = 0;
            //文本的开始位置
            var startPosition = -1;
            var text = "";
            while (true)
            {
                var content = reader.Read();
                if (content == -1)
                {
                    break;
                }

                var newChar = (char)content;
                //如果是分隔符，则直接跳过
                if (SeparatorList.Contains(newChar))
                {
                    var sqlTokenType = DetermineTokenType(text);
                    var token = new SqlToken(sqlTokenType, text, startPosition, i - 1);
                    startPosition = -1;
                    result.Add(token);
                    //添加空白
                    var separatorText = newChar.ToString();
                    var separatorSqlTokenType = DetermineTokenType(separatorText);
                    var separatorToken = new SqlToken(separatorSqlTokenType, separatorText, i, i);
             
                    result.Add(separatorToken);
                    //continue;
                }
                else
                {
                    if (startPosition == -1)
                    {
                        startPosition = i;
                    }
                }

                text = startPosition == -1 ? "" : text + newChar;
                Console.WriteLine("值为" + newChar);
                i++;
            }

            return result;
        }

        /// <summary>
        /// 判断token类型
        /// </summary>
        /// <returns></returns>
        private  SqlTokenType DetermineTokenType(string text)
        {
            //if (string.IsNullOrWhiteSpace(text))
            //{
            //    throw new Exception("sql为空");
            //}

            //var firstLetter = text.Substring(0, 1);
            //if (firstLetter == parameterPrefix)
            //{
            //    return SqlTokenType.Variable;
            //}

            //if (base.SpecialCharactersList.Contains(text))
            //{
            //    return SqlTokenType.SpecialCharacters;
            //}

            //if (base.OperatorList.Contains(text))
            //{
            //    return SqlTokenType.Operator;
            //}

            //if (base.FunctionList.Contains(firstLetter))
            //{
            //    return SqlTokenType.Function;
            //}

            //if (base.KeyWordList.Contains(firstLetter))
            //{
            //    return SqlTokenType.Function;
            //}

            return SqlTokenType.Identifiers;
        }
    }
}
