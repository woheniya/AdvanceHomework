using Homework_1Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Homework_1DAL
{
    public  interface IDBHelper
    {
        T Find<T>(int id) where T : BaseModel;
        List<T> Query<T>() where T : BaseModel;
        bool Insert<T>(T t) where T : BaseModel;
        bool Update<T>(T t) where T : BaseModel;
        bool Delete<T>(T t) where T : BaseModel;
    }
}
