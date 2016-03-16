using SYORM.Net.Cache;
using SYORM.Net.Enums;
using SYORM.Net.Queryer;
using SYORM.Net.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace SYORM.Net.Factory
{
    public static class DBFactory
    {
        #region 变量
        private static Conn _Connection = new Conn();
        #endregion

        #region 数据库设置
        public static EDbType DbType
        {
            set { _Connection.DbType = value; }
            get { return _Connection.DbType; }
        }

        public static string DbFilePath
        {
            set { _Connection.DbFilePath = value; }
            get { return _Connection.DbFilePath; }
        }
        public static bool IsSingleDataBase
        {
            set { _Connection.IsSingleConnection = value; }
            get { return _Connection.IsSingleConnection; }
        }
        #endregion

        #region 属性
        public static Conn Connection
        {
            get { return _Connection; }
        }
        #endregion

        #region 方法
        public static void CreateDataBase()
        {
            DataBaseUtil.CreateDataBase();
        }

        public static void CreateTable(Type type)
        {
            DataBaseUtil.CreateTable(type);
        }

        public static IQueryer<T> Query<T>()
        {
            Type type = typeof(T);
            BaseQueryer<T> queryer = (BaseQueryer<T>)BasicCache<Type, object>.Instance.Get(type);
            if (queryer == null)
            {
                queryer = new BasicQueryer<T>() as BaseQueryer<T>;
                queryer.Initialize();
                BasicCache<Type, object>.Instance.Set(type, queryer);
            }
            return queryer as IQueryer<T>;
        }

        internal static DbParameter CreateParameter()
        {
            if (DbType == EDbType.SQLite)
                return new SQLiteParameter();
            else
                return new SqlCeParameter();
        }
        #endregion
    }
}
