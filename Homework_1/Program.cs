using Homework_1DAL;
using Homework_1Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Homework_1
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlServerHelper sqlServer = new SqlServerHelper();
            //作业1.1
            {
                //作业一：提供两个泛型的数据库访问方法，用 BaseModel约束，一个是用id去查询单个实体，(只有这一个参数)
                //User user = new User();
                //user = SqlServerHelper.Find<User>(1);
                //user = SqlServerHelper.Find<User>(100);
            }

            //作业1.2 查询出数据表的全部数据列表查询(没有参数)
            {
                //List<User> userlist = new List<User>();
                //userlist = sqlServer.Query<User>();
            }

            //作业2 封装一个方法，能控制台输出任意实体的全部属性和属性值；
            {
                //User user = new User(){
                //    Id = 1,
                //    Name = "cc",
                //    Account = "cc"
                //};
                //GetProptyNameAndValue(user);
                //void GetProptyNameAndValue<T>(T t)
                //{
                //    Type type = t.GetType();
                //    foreach (PropertyInfo prop in type.GetProperties())
                //    {
                //        Console.WriteLine($"{prop.Name} = {prop.GetValue(t,null)}");
                //    }
                //}
            }

            //作业3.1 提供泛型的数据库实体插入
            {
                User user = new User()
                {
                    Id = 1,
                    Name = "cc",
                    Account = "cc",
                    Password = "cc",
                    State = 1,
                    UserType = 2,
                    CreateTime = DateTime.Now,
                    CreatorId = 1
                };
                sqlServer.Insert(user);
            }

        }

        
    }
}
