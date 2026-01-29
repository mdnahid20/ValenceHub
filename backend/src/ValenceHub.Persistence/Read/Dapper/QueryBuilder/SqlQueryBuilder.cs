using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Persistence.Read.Dapper.QueryBuilder
{
    public class SqlQueryBuilder
    {
        private readonly StringBuilder _sql = new StringBuilder();

        public SqlQueryBuilder Select(string columns) { _sql.Append($"SELECT {columns} "); return this; }
        public SqlQueryBuilder From(string table) { _sql.Append($"FROM {table} "); return this; }
        public SqlQueryBuilder Where(string condition) { _sql.Append($"WHERE {condition} "); return this; }
        public string Build() => _sql.ToString();
    }

}
