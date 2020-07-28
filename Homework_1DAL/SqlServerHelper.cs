using Homework_1Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Homework_1DAL
{
    public enum DataType
    {
        String,
        DateTime,
        Int32,
        Nullable
    }
    public class SqlServerHelper:IDBHelper
    {
        private const string connectStr = "Data Source=LocalHost;Integrated Security=SSPI;Database = CustomerDB;";
        #region 用DataReader去访问数据库，将得到的结果通过反射生成实体对象；
        /// <summary>
        /// 用DataReader去访问数据库，将得到的结果通过反射生成实体对象；
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Find<T>(int id) where T : BaseModel
        {
            Type type = typeof(T);
            //var propList = type.GetProperties().Select(p => $"[{p.Name}]");
            //string props = string.Join(',', propList);

            string sql = $"Select {string.Join(",", type.GetProperties().Select(p => $"[{p.Name}]"))} from [{type.Name}] where id = {id}";
            object objType = Activator.CreateInstance(type);
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        foreach (PropertyInfo prop in type.GetProperties())
                        {
                            prop.SetValue(objType, reader[prop.Name] is DBNull ? null : reader[prop.Name]);
                        }
                        return (T)objType;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public List<T> Query<T>() where T : BaseModel
        {
            Type type = typeof(T);
            List<T> list = null;
            object objType = null;
            //var propList = type.GetProperties().Select(p => $"[{p.Name}]");
            //string props = string.Join(',', propList);

            string sql = $"Select {string.Join(",", type.GetProperties().Select(p => $"[{p.Name}]"))} from [{type.Name}]";
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        list = new List<T>();
                        while (reader.Read())
                        {
                            objType = Activator.CreateInstance(type);
                            foreach (PropertyInfo prop in type.GetProperties())
                            {
                                prop.SetValue(objType, reader[prop.Name] is DBNull ? null : reader[prop.Name]);
                            }
                            list.Add((T)objType);
                        }
                    }
                    
                }
            }
            return list;
        }
        #endregion


        public bool Insert<T>(T t) where T : BaseModel
        {
            List<string> list = new List<string>();
            Type type = typeof(T);


            //var propNameList = type.GetProperties().Select(p => $"[{p.Name}]");
            //string propNames = string.Join(',', propNameList);


            var propListExecId = type.GetProperties().Where(a => !a.Name.Equals("Id"));

            var propNamesExecId = propListExecId.Select(a => $"[{a.Name}]");

            string propNames = string.Join(',', propNamesExecId);


            foreach (PropertyInfo prop in propListExecId)
            {
               list.Add(prop.GetValue(t) != null ? $"'{ prop.GetValue(t)}'" : "NULL");
            }
            string propValues = string.Join(',', list);

            string sql = $"Insert into [{type.Name}] ({propNames}) Values({propValues})";

            return ExcuteSql(sql) > 0;
            //string props = string.Join(',', propList);
        }


        public int ExcuteSql(string sql)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
