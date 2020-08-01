using Homework_1DAL;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Homework_1BLL
{
    public class SimpleFactory
    {
        public static IDBHelper CreateInstance()
        {
            string reflectionConfig = CustomConfigManager.GetConfig("ReflectionConfig");
            //typeName 命名空间名+类名
            string typeName = reflectionConfig.Split(",")[0];
            string dllName = reflectionConfig.Split(",")[1];

            //Assembly assembly = Assembly.Load(dllName); //Dll名称,不需要后缀 
            Assembly assembly = Assembly.LoadFrom(dllName); //dll名称(需要后缀)
            Type type = assembly.GetType(typeName);
            object obj = Activator.CreateInstance(type);
            return obj as IDBHelper;
        }
        public static class CustomConfigManager
        {
            public static string GetConfig(string key)
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");  //默认读取  当前运行目录
                IConfigurationRoot configuration = builder.Build();
                string configValue = configuration.GetSection(key).Value;
                return configValue;
            }
        }
    }
}
