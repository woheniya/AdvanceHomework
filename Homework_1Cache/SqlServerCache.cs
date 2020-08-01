using System;
using System.Linq;

namespace Homework_1Cache
{
    public static class SqlServerCache<T>
    {
        public static string FindSql = string.Empty;
        public static string QuerySql = string.Empty;
        public static string InsertSql = string.Empty;
        public static string UpdateSql = string.Empty;
        public static string DeleteSql = string.Empty;
        static SqlServerCache()
        {
            Type type = typeof(T);
            var props = type.GetProperties();
            FindSql = $"Select {string.Join(",", props.Select(p => $"[{p.Name}]"))} from [{type.Name}] where Id = @Id";

            QuerySql = $"Select {string.Join(",", props.Select(p => $"[{p.Name}]"))} from [{type.Name}]";

        }
    }
}
