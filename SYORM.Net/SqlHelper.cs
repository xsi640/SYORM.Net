using SYORM.Net.Cache;
using SYORM.Net.Factory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SYORM.Net
{
    public static class SqlHelper
    {

        #region 方法
        /// <summary>
        /// 将一条查询记录创建为一个实体类对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static T ExecuteDataReader<T>(IDataReader dr)
        {
            T obj = default(T);
            try
            {
                obj = Activator.CreateInstance<T>();
                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo propertyInfo in properties)
                {
                    string propertyName = propertyInfo.Name;
                    int index = EntityMappingCache.Instance.GetDatabaseFieldIndex(type, propertyName);
                    if (index == -1)
                    {
                        int fieldCount = dr.FieldCount;
                        for (int i = 0; i < fieldCount; i++)
                        {
                            string fieldName = dr.GetName(i);
                            if (string.Compare(propertyName, fieldName, true) == 0)
                            {
                                object value = dr.GetValue(i);
                                if (value != null && value != DBNull.Value)
                                {
                                    propertyInfo.SetValue(obj, value, null);
                                }
                                EntityMappingCache.Instance.SetDatabaseFieldIndex(type, propertyName, i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        object value = dr.GetValue(index);
                        if (value != null && value != DBNull.Value)
                        {
                            propertyInfo.SetValue(obj, value, null);
                        }
                    }
                }
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return obj;
        }
        /// <summary>
        /// 返回符合条件查询结果的泛型集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<T> ExecuteList<T>(string commandText, params DbParameter[] array)
        {
            List<T> lists = new List<T>();
            try
            {
                using (DbCommand cmd = CreateCommand(commandText, array))
                {
                    using (DbDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr != null)
                        {
                            while (dr.Read())
                            {
                                T obj = ExecuteDataReader<T>(dr);
                                if (obj != null)
                                {
                                    lists.Add(obj);
                                }
                            }
                        }
                    }
                }
            }
            catch (DbException ex)
            {
                throw ex;
            }

            return lists;
        }
        /// <summary>
        /// 返回符合条件查询结果的实体类对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T ExecuteEntity<T>(string commandText, params DbParameter[] array)
        {
            T obj = default(T);
            try
            {
                using (DbCommand cmd = CreateCommand(commandText, array))
                {
                    using (DbDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr != null)
                        {
                            while (dr.Read())
                            {
                                obj = ExecuteDataReader<T>(dr);
                                break;
                            }
                        }
                    }
                }
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return obj;
        }
        /// <summary>
        /// 执行不查询操作数据库命令
        /// 不支持事务操作
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string commandText, params DbParameter[] array)
        {
            int rowCount = 0;
            try
            {
                using (DbCommand cmd = CreateCommand(commandText, array))
                {
                    rowCount = cmd.ExecuteNonQuery();
                }
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return rowCount;
        }
        /// <summary>
        /// 执行不查询操作数据库命令
        /// 支持事务操作
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(DbTransaction transaction, string commandText, params DbParameter[] array)
        {
            int rowCount = 0;
            try
            {
                using (DbCommand cmd = CreateCommand(transaction, commandText, array))
                {
                    rowCount = cmd.ExecuteNonQuery();
                }
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return rowCount;

        }
        /// <summary>
        /// 执行聚合函数查询
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int ExecuteScalar(string commandText, params DbParameter[] array)
        {
            int result = 0;
            try
            {
                using (DbCommand cmd = CreateCommand(commandText, array))
                {
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        Int32.TryParse(obj.ToString(), out result);
                    }
                }
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return result;
        }

        private static DbCommand CreateCommand(DbTransaction transaction, string commandText, params DbParameter[] array)
        {
            DbCommand command = DBFactory.Connection.DbConnection.CreateCommand();
            command.CommandText = commandText;
            command.Transaction = transaction;
            if (array != null && array.Length > 0)
            {
                foreach (DbParameter parameter in array)
                    command.Parameters.Add(parameter);
            }
            return command;
        }

        private static DbCommand CreateCommand(string commandText, params DbParameter[] array)
        {
            return CreateCommand(null, commandText, array);
        }
        #endregion
    }
}
