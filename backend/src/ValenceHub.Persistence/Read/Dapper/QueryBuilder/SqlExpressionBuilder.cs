using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Persistence.Read.Dapper.QueryBuilder
{
    public class SqlExpressionBuilder
    {
        public static string Equals(string column, object value) => $"{column} = @{column}";
        public static string Like(string column, string pattern) => $"{column} LIKE @{column}";
    }

}
