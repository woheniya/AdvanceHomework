using Homework_1Cache;
using Homework_1Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Homework_1DAL
{
    public class SqlServerHelper:IDBHelper
    {
        private const string connectStr = "Data Source=LocalHost;Integrated Security=SSPI;Database = CustomerDB;";
        #region 作业3.用DataReader去访问数据库，将得到的结果通过反射生成实体对象；
        /// <summary>
        /// 用DataReader去访问数据库，将得到的结果通过反射生成实体对象；
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Find<T>(int id) where T : BaseModel
        {
            Type type = typeof(T);
            string sql = SqlServerCache<T>.FindSql;
            object objType = Activator.CreateInstance(type);
            SqlParameter sqlParameter = new SqlParameter() { ParameterName = "@Id",Value = id};
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(sqlParameter);
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
            string sql = SqlServerCache<T>.QuerySql;
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

        #region 作业5.进阶需求：提供泛型的数据库实体插入、实体更新、ID删除数据的数据库访问方法；
        /// <summary>
        /// 提供泛型的数据库实体插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Insert<T>(T t) where T : BaseModel
        {
            List<string> list = new List<string>();
            Type type = typeof(T);
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
        }


        /// <summary>
        /// 提供泛型的数据库实体插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Update<T>(T t) where T : BaseModel
        {
            List<string> list = new List<string>();
            Type type = typeof(T);
            string whereStr = string.Empty;

            var propNameList = type.GetProperties().Select(p => $"[{p.Name}]");
            //string propNames = string.Join(',', propNameList);
            //var propListExecId = type.GetProperties().Where(a => !a.Name.Equals("Id"));
            //var propNamesExecId = propListExecId.Select(a => $"[{a.Name}]");
            //string propNames = string.Join(',', propNamesExecId);


            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.Name != "Id")
                {
                    list.Add($"{ prop.Name} = " + (prop.GetValue(t) != null ? $"'{ prop.GetValue(t)}'" : "NULL"));
                }
                else
                {
                    whereStr = $" WHERE Id = {prop.GetValue(t)}";
                }
            }
            string propValues = string.Join(',', list);

            string sql = $"UPDATE [{type.Name}] SET {propValues} {whereStr}";

            return ExcuteSql(sql) > 0;
        }

        /// <summary>
        /// 提供泛型的数据库实体删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Delete<T>(T t) where T : BaseModel
        {
            Type type = typeof(T);

            var propLast = type.GetProperties().Last();
            propLast.GetValue(t);

            string sql = $"DELETE FROM [{type.Name}] WHERE Id = {type.GetProperties().Last().GetValue(t)}";

            return ExcuteSql(sql) > 0;
        }
        #endregion



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
